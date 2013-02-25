using Omegaluz.SimpleOAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
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

        public override bool HasLocalAccountFromId(object userId)
        {
            var user = Membership.GetUser(userId);
            return user != null;
        }

        public override bool HasLocalAccountFromUserName(string userName)
        {
            var user = Membership.GetUser(userName);
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
            string userName = null;

            using (var db = new ExternalProvidersContext())
            {
                var user = db.Users.FirstOrDefault(e => e.UserId == (Guid)userId);
                if (user != null)
                {
                    userName = user.UserName;
                }
            }

            return userName;


            //var user = Membership.GetUser(userId);

            //return user != null ? user.UserName : null;
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

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
        {

            try
            {

                var existingUserId = GetUserId(userName);

                using (var db = new ExternalProvidersContext())
                {
                    var newUserName = Membership.GeneratePassword(50, 0);
                    var newMembership = Membership.CreateUser(newUserName, password);
                    var newUserId = (Guid)newMembership.ProviderUserKey;

                    // TODO: refactor this
                    var connectionString = ConfigurationManager.ConnectionStrings["IdentityServerConfiguration"].ConnectionString;

                    using (var conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string updateString = "UPDATE Memberships SET UserId = @OldUserID WHERE UserID = @NewUserID";
                        var updateCommand = new SqlCommand(updateString, conn);
                        var oldUserIdParam = updateCommand.Parameters.Add("@OldUserId", System.Data.SqlDbType.UniqueIdentifier);
                        oldUserIdParam.Value = existingUserId;
                        var newUserIdParam = updateCommand.Parameters.Add("@NewUserID", System.Data.SqlDbType.UniqueIdentifier);
                        newUserIdParam.Value = newUserId;
                        updateCommand.ExecuteNonQuery();
                        conn.Close();
                    }


                    // delete the user row that we just created
                    db.Users.Remove(db.Users.First(e => e.UserId == newUserId));
                    db.SaveChanges();

                    // we don't require a confirmation token for the sql membership provider (default)\
                    return null;

                }

            }
            catch (DbEntityValidationException ex)
            {
                var firstError = ex.EntityValidationErrors.First();
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }
            catch (Exception)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }
            

        }

        public override void CreateUserRow(string userName)
        {
            try
            {

                using (var db = new ExternalProvidersContext())
                {

                    if (db.Users.FirstOrDefault(e => e.UserName.ToUpper() == userName.ToUpper()) != null)
                    {
                        throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
                    }

                    // get the applicationID
                    var applicationID = db.Applications.First(e => e.ApplicationName.ToUpper() == Membership.ApplicationName.ToUpper()).ApplicationId;

                    db.Users.Add(new Users { UserId = Guid.NewGuid(), UserName = userName, IsAnonymous = false, LastActivityDate = DateTime.Now, ApplicationId = applicationID });
                    db.SaveChanges();

                }
            }
            catch (Exception)
            {

                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }
        }

        public override void DeleteOAuthAccount(string provider, string providerUserId)
        {
            using (var db = new ExternalProvidersContext())
            {
                // delete account
                try
                {
                    var account = db.OAuthMembership
                        .First(e => e.Provider.ToUpper() == provider.ToUpper() && e.ProviderUserId.ToUpper() == providerUserId.ToUpper());

                    db.OAuthMembership.Remove(account);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
                }
            }
        }

        public object GetUserId(string userName)
        {

            var userID = Guid.Empty;

            using (var db = new ExternalProvidersContext())
            {
                var user = db.Users.FirstOrDefault(e => e.UserName.ToUpper() == userName.ToUpper());
                if (user != null)
                {
                    userID = user.UserId;
                }
            }

            return userID;

        }

    }
}