using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
}
