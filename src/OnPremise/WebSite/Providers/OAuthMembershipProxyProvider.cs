using Omegaluz.SimpleOAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Security;
using Thinktecture.IdentityServer.Repositories;

namespace Thinktecture.IdentityServer.Web.Providers
{
    public class OAuthMembershipProxyProvider : OAuthMembershipProvider
    {

        //[Import]
        //public IUserManagementRepository UserManagementRepository { get; set; }

        //public OAuthMembershipProxyProvider()
        //{
        //    Container.Current.SatisfyImportsOnce(this);
        //}


        public override string GetUserNameFromOpenAuth(string openAuthProvider, string openAuthId)
        {

            object userId = GetUserIdFromOAuth(openAuthProvider, openAuthId);

            if (userId == null || (userId is Guid && ((Guid)userId) == Guid.Empty))
            {
                return null;
            }

            return GetUserNameFromId(userId);

        }

        public override string GetUserNameFromId(object userId)
        {
            var user = Membership.GetUser(userId); ;

            return user != null ? user.UserName : null;
        }

        public override object GetUserIdFromOAuth(string provider, string providerUserId)
        {
            object userID = null;

            using (var membership = new ExternalProvidersContext())
            {
                var oauthuser = membership.OAuthMembership
                    .FirstOrDefault(e => e.Provider == provider && e.ProviderUserId == providerUserId);

                if (oauthuser != null)
                {
                    userID = oauthuser.UserId;
                }
            }

            return userID;
        }

        public override ICollection<OAuthAccount> GetAccountsForUser(string userName)
        {
            throw new NotImplementedException();
        }

    }
}