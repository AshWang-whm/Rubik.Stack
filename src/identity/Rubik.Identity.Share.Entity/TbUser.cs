

namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_user")]
    public class TbUser:BaseFullEntity
    {

        [Column(IsNullable =false,Position =4)]
        public string? Password { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [Column(IsNullable =false,Position =5)]
        public int? PositionID { get; set; }

        [Column(IsNullable =true)]
        public string? Email { get; set; }

        [Column(IsNullable =false)]
        public Gender Gender { get; set; }

        [Column(IsNullable =true)]
        public int? Age { get; set; }

        [Column(IsNullable =true)]
        public DateTime? EntryDate { get; set; }


        [Navigate(nameof(PositionID))]
        public TbPosition? TbPosition { get; set; }
    }


    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
