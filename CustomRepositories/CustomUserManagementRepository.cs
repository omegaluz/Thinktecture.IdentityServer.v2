using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Thinktecture.IdentityServer.Repositories;

namespace CustomRepositories
{
    public class CustomUserManagementRepository : IUserManagementRepository
    {
        public void CreateUser(string userName, string password, string email = null)
        {
            try
            {
                Membership.CreateUser(userName, password, email);
            }
            catch (MembershipCreateUserException ex)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public void DeleteUser(string userName)
        {
            Membership.DeleteUser(userName);
        }

        public void SetRolesForUser(string userName, IEnumerable<string> roles)
        {
            var userRoles = System.Web.Security.Roles.GetRolesForUser(userName);

            if (userRoles.Length != 0)
            {
                System.Web.Security.Roles.RemoveUserFromRoles(userName, userRoles);
            }

            System.Web.Security.Roles.AddUserToRoles(userName, roles.ToArray());
        }

        public IEnumerable<string> GetRolesForUser(string userName)
        {
            return System.Web.Security.Roles.GetRolesForUser(userName);
        }

        public IEnumerable<string> GetRoles()
        {
            return System.Web.Security.Roles.GetAllRoles();
        }

        public void CreateRole(string roleName)
        {
            try
            {
                System.Web.Security.Roles.CreateRole(roleName);
            }
            catch (ProviderException)
            { }
        }

        public void DeleteRole(string roleName)
        {
            try
            {
                System.Web.Security.Roles.DeleteRole(roleName);
            }
            catch (ProviderException)
            { }
        }

        public IEnumerable<string> GetUsers()
        {
            using (var db = new UsersContext())
            {
                return db.Users.Select(e => e.UserName)
                                        .ToList();
            }
            //var items = Membership.GetAllUsers().OfType<MembershipUser>();
            //return items.Select(x => x.UserName);
        }

        public IEnumerable<string> GetUsers(string filter)
        {

            using (var db = new UsersContext())
            {
                return db.Users.Select(e => e.UserName)
                    .Where(e => e.Contains(filter))
                    .ToList();
            }

            //var items = Membership.GetAllUsers().OfType<MembershipUser>();
            //var query =
            //    from user in items
            //    where user.UserName.Contains(filter) ||
            //          (user.Email != null && user.Email.Contains(filter))
            //    select user.UserName;
            //return query;
        }
    }
}
