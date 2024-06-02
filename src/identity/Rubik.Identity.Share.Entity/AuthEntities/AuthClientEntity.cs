using FreeSql.DataAnnotations;
using Rubik.Identity.Share.Entity.BaseEntities;

namespace Rubik.Identity.Share.Entity.AuthEntities
{
    [Table(Name = "auth_client", DisableSyncStructure = false)]
    public class AuthClientEntity : BaseFullEntity
    {
        [Column(Position = 2, IsNullable = false)]
        public string? ClientId { get; set; }

        [Column(Position = 3)]
        public string? ResponseUrl { get; set; }


    }
}
