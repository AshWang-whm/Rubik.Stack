using FreeSql.DataAnnotations;
using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.LowCode.Share.Entity
{
    public class BaseComponentConfigEntity:BaseFullEntity
    {
        [Column(IsNullable =false)]
        public int ComponentID { get; set; }

        [Column(IsNullable =false)]
        public string? Version { get; set; }

        [Column(IsNullable =true)]
        public string? Remark { get; set; }

        [Navigate(nameof(ComponentID))]
        public TbComponentEntity? TbComponent { get; set; }
    }
}
