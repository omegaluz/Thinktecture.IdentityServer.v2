using System.ComponentModel.DataAnnotations;

namespace Thinktecture.IdentityServer.Repositories.Sql
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        public string TokenIdentifier { get; set; }

        public int ClientId { get; set; }

        public string UserName { get; set; }

        public string Scope { get; set; }
    }
}
