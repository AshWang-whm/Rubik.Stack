using FreeSql.DataAnnotations;

namespace Rubik.Identity.Share.Entity.BaseEntity
{
    public abstract class BaseNewEntity : INewEntity
    {
        [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
        public int ID { get; set; }

        [Column(Position = -2, IsNullable = false, DbType = "timestamp")]
        public DateTime? AddDate { get; set; }

        [Column(Position = -1, IsNullable = true)]
        public string? AddUser { get; set; }
    }

    /// <summary>
    /// 包括 INewEntity,IDeleteEtity
    /// </summary>
    public abstract class BaseRecordEntity : IRecordEntity
    {
        [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
        public int ID { get; set; }

        [Column(Position = -2, IsNullable = false, DbType = "timestamp")]
        public DateTime? AddDate { get; set; }

        [Column(Position = -1, IsNullable = true)]
        public string? AddUser { get; set; }

        [Column(Position =-3,IsNullable =false)]
        public bool IsDelete { get; set; } = false;
    }


    /// <summary>
    /// 包括 INewEntity, IModifyEntity,IDeleteEtity
    /// </summary>
    public abstract class BaseFullEntity : IFullEntity
    {
        [Column(IsPrimary = true, IsIdentity = true, Position = 1)]
        public int ID { get; set; }



        [Column(Position = -1, IsNullable = true)]
        public string? AddUser { get; set; }

        [Column(Position = -2, IsNullable = false, DbType = "timestamp")]
        public DateTime? AddDate { get; set; }

        [Column(Position = -3)]
        public bool IsDelete { get; set; } = false;

        [Column(Position = -4, IsNullable = true)]
        public string? ModifyUser { get; set; }

        [Column(Position = -5, IsNullable = false, DbType = "timestamp")]
        public DateTime? ModifyDate { get; set; }

    }
}
