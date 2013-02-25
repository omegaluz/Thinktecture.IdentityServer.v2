using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Thinktecture.IdentityServer.Web.Providers
{

    public class ExternalProvidersContext : DbContext
    {
        public ExternalProvidersContext()
            : base("IdentityServerConfiguration")
        {
        }

        public DbSet<ExternalProvidersOAuthMembership> OAuthMembership { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Applications> Applications { get; set; }
       
    }

    public class ExternalProvidersOAuthMembership
    {
        [Key, Column(Order = 0)]
        public string Provider { get; set; }
        [Key, Column(Order = 1)]
        public string ProviderUserId { get; set; }
        public Guid UserId { get; set; }
    }

    public class Users
    {
        [Key]
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }
    }

    public class Applications
    {
        [Key]
        public Guid ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
    }

}