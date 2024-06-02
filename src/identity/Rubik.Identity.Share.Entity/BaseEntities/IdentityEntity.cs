using FreeSql.DataAnnotations;

namespace Rubik.Identity.Share.Entity.BaseEntities
{
    public class IdentityEntity:BaseFullEntity
    {
        [Column(Position =2,IsNullable =false)]
        public string? Name { get; set; }
    }
}
