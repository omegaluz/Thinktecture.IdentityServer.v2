﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Profile;
using System.Web.Security;
using Thinktecture.IdentityServer;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.TokenService;

namespace CustomRepositories
{
    public class CustomClaimsRepository : IClaimsRepository
    {

        private const string ProfileClaimPrefix = "http://identityserver.thinktecture.com/claims/profileclaims/";

        public IEnumerable<Claim> GetClaims(ClaimsPrincipal principal, RequestDetails requestDetails)
        {
            var userName = principal.Identity.Name;
            var claims = new List<Claim>(from c in principal.Claims select c);

            // email address

            using (var db = new UsersContext())
            {
                string email = null;
                var user = db.Users.First(e => e.UserName.ToUpper() == userName.ToUpper());
                var userid = user.UserId;
                var membership = db.Memberships.FirstOrDefault(e => e.UserId == userid);
                if (membership != null)
                {
                    email = membership.Email;
                }
                if (!String.IsNullOrEmpty(email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, email));
                }
            }


            // roles
            GetRolesForToken(userName).ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

            // profile claims
            if (ProfileManager.Enabled)
            {
                var profile = ProfileBase.Create(userName, true);
                if (profile != null)
                {
                    foreach (SettingsProperty prop in ProfileBase.Properties)
                    {
                        object value = profile.GetPropertyValue(prop.Name);
                        if (value != null)
                        {
                            if (!string.IsNullOrWhiteSpace(value.ToString()))
                            {
                                claims.Add(new Claim(ProfileClaimPrefix + prop.Name.ToLowerInvariant(), value.ToString()));
                            }
                        }
                    }
                }
            }

            return claims;
        }

        public IEnumerable<string> GetSupportedClaimTypes()
        {
            var claimTypes = new List<string>
            {
                ClaimTypes.Name,
                ClaimTypes.Email,
                ClaimTypes.Role
            };

            if (ProfileManager.Enabled)
            {
                foreach (SettingsProperty prop in ProfileBase.Properties)
                {
                    claimTypes.Add(ProfileClaimPrefix + prop.Name.ToLowerInvariant());
                }
            }

            return claimTypes;
        }

        protected virtual IEnumerable<string> GetRolesForToken(string userName)
        {
            var returnedRoles = new List<string>();

            if (System.Web.Security.Roles.Enabled)
            {
                string[] roles;
                using (var db = new UsersContext())
                {
                    var userId = db.Users.First(e => e.UserName.ToUpper() == userName.ToUpper())
                        .UserId;
                    var userroles = db.UsersInRoles.Where(e => e.UserId == userId);
                    roles = db.Roles.Join(userroles, 
                        e => e.RoleId, 
                        f => f.RoleId,
                        (e, f) => e.RoleName)
                        .ToArray();
                }

                returnedRoles = roles.Where(role => !(role.StartsWith(Constants.Roles.InternalRolesPrefix))).ToList();
            }

            return returnedRoles;
        }
    }
}
