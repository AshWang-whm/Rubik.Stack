
namespace Rubik.Infrastructure.Entity.BaseEntity
{
    public interface IIdEntity<TId>
    {
        public TId ID { get; set; }
    }

    public interface IIntIdEntity:IIdEntity<int>
    {

    }

    public interface INewEntity : IIntIdEntity
    {
        public string? AddUser { get; set; }

        public DateTime? AddDate { get; set; }
    }

    public interface IModifyEntity: IIntIdEntity
    {
        public string? ModifyUser { get; set; }

        public DateTime? ModifyDate { get; set; }
    }

    public interface IDeleteEtity: IIntIdEntity
    {
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// 包括 INewEntity,IDeleteEtity
    /// </summary>
    public interface IRecordEntity:INewEntity,IDeleteEtity
    {

    }


    /// <summary>
    /// 包括 INewEntity, IModifyEntity,IDeleteEtity
    /// </summary>
    public interface IFullEntity : IRecordEntity,IModifyEntity
    {

    }
}
