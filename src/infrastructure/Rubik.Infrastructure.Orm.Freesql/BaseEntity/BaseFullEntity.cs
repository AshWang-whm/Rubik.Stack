using FreeSql.DataAnnotations;

namespace Rubik.Infrastructure.Entity.BaseEntity
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

        [Column(IsNullable = false,Position =2)]
        public string? Name { get; set; }

        [Column(IsNullable =false,Position =3)]
        public string? Code { get; set; }

        [Column(Position =-1)]
        public int Sort { get; set; }

        [Column(Position = -2, IsNullable = true)]
        public string? AddUser { get; set; }

        [Column(Position = -3, IsNullable = false, DbType = "timestamp")]
        public DateTime? AddDate { get; set; }

        [Column(Position = -4)]
        public bool IsDelete { get; set; } = false;

        [Column(Position = -5, IsNullable = true)]
        public string? ModifyUser { get; set; }

        [Column(Position = -6, IsNullable = false, DbType = "timestamp")]
        public DateTime? ModifyDate { get; set; }

    }

    public abstract class BaseTreeEntity<TEntity>:BaseFullEntity,ITreeEntity<TEntity>
        where TEntity : class
    {
        [Column(IsNullable = true, Position = 99)]
        public int? ParentID { get; set; }


        //[Column(IsIgnore = true)]
        [Navigate(nameof(ParentID))]
        public List<TEntity> Children { get; set; } = [];


        [Navigate(nameof(ParentID))]
        public TEntity? Parent { get; set; }
    }
}
