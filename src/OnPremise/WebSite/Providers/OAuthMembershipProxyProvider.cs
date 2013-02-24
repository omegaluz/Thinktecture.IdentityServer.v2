using Omegaluz.SimpleOAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.WebPages;
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

        public override bool HasLocalAccount(object userId)
        {
            var user = Membership.GetUser(userId);
            return user != null;
        }

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
            Guid userId = (Guid)GetUserId(userName);

            if (userId != Guid.Empty)
            {
                using (var db = new ExternalProvidersContext())
                {
                    var oauthMemberships = db.OAuthMembership
                        .Where(e => e.UserId == userId);

                    return oauthMemberships
                        .ToList()
                        .Select(e => new OAuthAccount(e.Provider, e.ProviderUserId))
                        .ToList();
                }
            }

            return new OAuthAccount[0];
        }

        public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
        {
            if (userName.IsEmpty())
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }

            Guid userId = (Guid)GetUserId(userName);

            if (userId == Guid.Empty)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidUserName);
            }

            var oldUserId = GetUserIdFromOAuth(provider, providerUserId);

            using (var db = new ExternalProvidersContext())
            {
                if (oldUserId == null || (oldUserId != null && (Guid)oldUserId == Guid.Empty))
                {
                    // account doesn't exist. create a new one.
                    var membership = new ExternalProvidersOAuthMembership
                    {
                        Provider = provider,
                        ProviderUserId = providerUserId,
                        UserId = userId
                    };

                    try
                    {
                        db.OAuthMembership.Add(membership);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
                    }
                }
                else
                {
                    // account already exists. update it
                    try
                    {
                        var membership = db.OAuthMembership
                            .First(e => e.Provider.ToUpper() == provider.ToUpper() && e.ProviderUserId.ToUpper() == providerUserId.ToUpper());

                        membership.UserId = userId;
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
                    }
                }
            }
        }

        public object GetUserId(string userName)
        {
            var result = Membership.GetUser(userName);

            if (result != null)
            {
                return result.ProviderUserKey;
            }

            return Guid.Empty;
        }

    }
}