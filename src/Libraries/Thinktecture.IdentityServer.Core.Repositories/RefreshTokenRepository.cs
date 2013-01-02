/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;

namespace Thinktecture.IdentityServer.Repositories.Sql
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        public string AddToken(int clientId, string userName, string scope)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var tokenId = Guid.NewGuid().ToString("N");

                var refreshToken = new RefreshToken
                {
                    TokenIdentifier = tokenId,
                    ClientId = clientId,
                    Scope = scope,
                    UserName = userName
                };

                entities.RefreshTokens.Add(refreshToken);
                entities.SaveChanges();

                return tokenId;
            }
        }

        public bool TryGetToken(string tokenIdentifier, out Models.RefreshToken token)
        {
            token = null;

            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var entity = (from t in entities.RefreshTokens
                              where t.TokenIdentifier.Equals(tokenIdentifier, StringComparison.OrdinalIgnoreCase)
                              select t)
                             .FirstOrDefault();

                if (entity != null)
                {
                    token = entity.ToDomainModel();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void DeleteToken(string tokenIdentifier)
        {
            using (var entities = IdentityServerConfigurationContext.Get())
            {
                var item = entities.RefreshTokens.Where(x => x.TokenIdentifier.Equals(tokenIdentifier, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (item != null)
                {
                    entities.RefreshTokens.Remove(item);
                    entities.SaveChanges();
                }
            }
        }
    }
}
