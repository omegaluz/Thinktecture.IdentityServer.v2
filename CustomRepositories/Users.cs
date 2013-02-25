using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRepositories
{

    class UsersContext : DbContext
    {
        public UsersContext()
            : base("IdentityServerConfiguration")
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<UsersInRoles> UsersInRoles { get; set; }
        public DbSet<Memberships> Memberships { get; set; }

    }


    class Users
    {
        [Key]
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }
    }

    class Roles
    {
        [Key]
        public Guid RoleId { get; set; }
        public Guid ApplicationId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }

    class UsersInRoles
    {
        [Key, Column(Order = 0)]
        public Guid UserId { get; set; }
        [Key, Column(Order = 1)]
        public Guid RoleId { get; set; }
    }

    class Memberships
    {
        [Key]
        public Guid UserId { get; set; }
        public string Email { get; set; }
    }
}
