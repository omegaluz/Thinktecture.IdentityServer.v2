/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.ComponentModel.Composition;
using System.IdentityModel.Protocols.WSTrust;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Thinktecture.IdentityModel.Authorization;
using Thinktecture.IdentityServer.Models;
using Thinktecture.IdentityServer.Repositories;

namespace Thinktecture.IdentityServer.Protocols.OAuth2
{
    public class OAuth2TokenController : ApiController
    {
        [Import]
        public IUserRepository UserRepository { get; set; }

        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        [Import]
        public IClientsRepository ClientsRepository { get; set; }

        [Import]
        public ICodeTokenRepository RefreshTokenRepository { get; set; }

        public OAuth2TokenController()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public OAuth2TokenController(IUserRepository userRepository, IConfigurationRepository configurationRepository, IClientsRepository clientsRepository, ICodeTokenRepository refreshTokenRepository)
        {
            UserRepository = userRepository;
            ConfigurationRepository = configurationRepository;
            ClientsRepository = clientsRepository;
            RefreshTokenRepository = refreshTokenRepository;
        }

        public HttpResponseMessage Post(TokenRequest tokenRequest)
        {
            Tracing.Information("OAuth2 endpoint called.");

            Client client = null;
            if (!ValidateClient(out client))
            {
                Tracing.Error("Invalid client: " + ClaimsPrincipal.Current.Identity.Name);
                return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidClient);
            }

            Tracing.Information("Client: " + client.Name);

            var tokenType = ConfigurationRepository.Global.DefaultHttpTokenType;

            // validate scope
            EndpointReference appliesTo = null;
            if (tokenRequest.Grant_Type != OAuth2Constants.GrantTypes.AuthorizationCode)
            {
                try
                {
                    appliesTo = new EndpointReference(tokenRequest.Scope);
                    Tracing.Information("OAuth2 endpoint called for scope: " + tokenRequest.Scope);
                }
                catch
                {
                    Tracing.Error("Malformed scope: " + tokenRequest.Scope);
                    return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidScope);
                }
            }

            // check grant type - password == resource owner password flow
            if (string.Equals(tokenRequest.Grant_Type, OAuth2Constants.GrantTypes.Password, System.StringComparison.Ordinal))
            {
                if (ConfigurationRepository.OAuth2.EnableResourceOwnerFlow)
                {
                    if (client.AllowResourceOwnerFlow)
                    {
                        return ProcessResourceOwnerCredentialRequest(tokenRequest.UserName, tokenRequest.Password, appliesTo, tokenType, client);
                    }
                    else
                    {
                        Tracing.Error("Client is not allowed to use the resource owner password flow:" + client.Name);
                    }
                }
            } // or refresh token
            else if (string.Equals(tokenRequest.Grant_Type, OAuth2Constants.GrantTypes.RefreshToken, System.StringComparison.Ordinal))
            {
                // TODO: refresh tokens allowed?
                return ProcessRefreshTokenRequest(client, tokenRequest.Refresh_Token);
            } // or code grant
            else if (string.Equals(tokenRequest.Grant_Type, OAuth2Constants.GrantTypes.AuthorizationCode, System.StringComparison.Ordinal))
            {
                // TODO: refresh tokens allowed?
                return ProcessAuthorizationCodeRequest(client, tokenRequest.Code);
            }

            Tracing.Error("invalid grant type: " + tokenRequest.Grant_Type);
            return OAuthErrorResponseMessage(OAuth2Constants.Errors.UnsupportedGrantType);
        }

        private HttpResponseMessage ProcessAuthorizationCodeRequest(Client client, string code)
        {
            return ProcessRefreshTokenRequest(client, code);
        }

        private HttpResponseMessage ProcessResourceOwnerCredentialRequest(string userName, string password, EndpointReference appliesTo, string tokenType, Client client)
        {
            Tracing.Information("Starting resource owner password credential flow for client: " + client.Name);

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                Tracing.Error("Invalid resource owner credentials for: " + appliesTo.Uri.AbsoluteUri);
                return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidGrant);
            }

            if (UserRepository.ValidateUser(userName, password))
            {
                // TODO: make refresh token configurable
                return CreateTokenResponse(userName, client, appliesTo, includeRefreshToken: true);
            }
            else
            {
                Tracing.Error("Resource owner credential validation failed: " + userName);
                return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidGrant);
            }
        }

        private HttpResponseMessage ProcessRefreshTokenRequest(Client client, string refreshToken)
        {
            Tracing.Information("Processing refresh token request for client: " + client.Name);

            // 1. get refresh token from DB - if not exists: error
            CodeToken token;
            if (RefreshTokenRepository.TryGetCode(refreshToken, out token))
            {
                // 2. make sure the client is the same - if not: error
                if (token.ClientId == client.ID)
                {
                    // 3. call STS 
                    // TODO: make refresh token configurable
                    RefreshTokenRepository.DeleteCode(token.Code);
                    return CreateTokenResponse(token.UserName, client, new EndpointReference(token.Scope), includeRefreshToken: true);
                }

                Tracing.Error("Invalid client for refresh token. " + client.Name + " / " + refreshToken);
                return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidGrant);
            }

            Tracing.Error("Refresh token not found. " + client.Name + " / " + refreshToken);
            return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidGrant);
        }

        private HttpResponseMessage CreateTokenResponse(string userName, Client client, EndpointReference scope, bool includeRefreshToken, string tokenType = null)
        {
            if (string.IsNullOrWhiteSpace(tokenType))
            {
                tokenType = ConfigurationRepository.Global.DefaultHttpTokenType;
            }

            var auth = new AuthenticationHelper();

            var principal = auth.CreatePrincipal(userName, "OAuth2",
                    new Claim[]
                        {
                            new Claim(Constants.Claims.Client, client.Name),
                            new Claim(Constants.Claims.Scope, scope.Uri.AbsoluteUri)
                        });

            if (!ClaimsAuthorization.CheckAccess(principal, Constants.Actions.Issue, Constants.Resources.OAuth2))
            {
                Tracing.Error("OAuth2 endpoint authorization failed for user: " + userName);
                return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidGrant);
            }

            var sts = new STS();
            TokenResponse tokenResponse;
            if (sts.TryIssueToken(scope, principal, tokenType, out tokenResponse))
            {
                if (includeRefreshToken)
                {
                    tokenResponse.RefreshToken = RefreshTokenRepository.AddCode(client.ID, userName, scope.Uri.AbsoluteUri);
                }

                var resp = Request.CreateResponse<TokenResponse>(HttpStatusCode.OK, tokenResponse);
                return resp;
            }
            else
            {
                return OAuthErrorResponseMessage(OAuth2Constants.Errors.InvalidRequest);
            }
        }

        private HttpResponseMessage OAuthErrorResponseMessage(string error)
        {
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                string.Format("{{ \"{0}\": \"{1}\" }}", OAuth2Constants.Errors.Error, error));
        }

        private bool ValidateClient(out Client client)
        {
            client = null;

            if (!ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                Tracing.Error("Anonymous client.");
                return false;
            }

            var passwordClaim = ClaimsPrincipal.Current.FindFirst("password");
            if (passwordClaim == null)
            {
                Tracing.Error("No client secret provided.");
                return false;
            }

            return ClientsRepository.ValidateAndGetClient(
                ClaimsPrincipal.Current.Identity.Name,
                passwordClaim.Value, 
                out client);
        }
    }
}
