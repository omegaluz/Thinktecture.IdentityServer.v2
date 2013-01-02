/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.IdentityServer.Models;
using Thinktecture.IdentityServer.TokenService;

namespace Thinktecture.IdentityServer.Repositories
{
    /// <summary>
    /// Repository for handling refresh tokens
    /// </summary>
    public interface IRefreshTokenRepository
    {
        string AddToken(string clientId, string userName, string scope);
        RefreshToken GetToken(string tokenIdentifier);
        void DeleteToken(string tokenIdentifier);
    }
}
