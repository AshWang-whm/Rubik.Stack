using Rubik.Identity.Share.Entity.BaseEntity;

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_user")]
    public class TbUser:BaseFullEntity
    {
        [Column(IsNullable =false)]
        public string? Account { get; set; }

        [Column(IsNullable =false)]
        public string? Password { get; set; }

        [Column(IsNullable =false)]
        public string? UserName { get; set; }

        [Column(IsNullable =true)]
        public string? Email { get; set; }

        [Column(IsNullable =false)]
        public Gender Gender { get; set; }

        [Column(IsNullable =true)]
        public int? Age { get; set; }

        [Column(IsNullable =true)]
        public DateTime? EntryDate { get; set; }
    }


    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
