

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_user")]
    public class TbUser:BaseFullEntity
    {

        [Column(IsNullable =false,Position =4)]
        public string? Password { get; set; }

        [Column(IsNullable =true)]
        public string? Email { get; set; }

        [Column(IsNullable =false)]
        public Gender Gender { get; set; }

        [Column(IsNullable =true)]
        public int? Age { get; set; }

        [Column(IsNullable =true)]
        public DateTime? EntryDate { get; set; }


        [Column(IsIgnore =true)]
        public string? Department { get; set; }
    }

    [Flags]
    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
