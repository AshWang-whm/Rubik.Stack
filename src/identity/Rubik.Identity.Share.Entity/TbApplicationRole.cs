using Rubik.Identity.Share.Entity.BaseEntity;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_application_role")]
    public class TbApplicationRole:BaseTreeEntity
    {
        public string? Description { get; set; }

        [Column(IsNullable =false)]
        public int ApplicationID { get; set; }
    }
}
