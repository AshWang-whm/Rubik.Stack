using FreeSql.DataAnnotations;
using Rubik.Share.Entity.BaseEntity;
using System.Reflection;

namespace Rubik.Identity.Share.Extension
{
    public static class FreesqlRecursionExtension
    {
        ///// <summary>
        ///// 根据主键,ParentID 递归查询数据
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="freeSql"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static async Task<List<T>> Recursion<T>(this IFreeSql freeSql,int id)
        //    where T : BaseTreeEntity
        //{
        //    var table = typeof(T).GetCustomAttribute<TableAttribute>()?.Name??nameof(T);
        //    var sql = "";

        //    var list = await freeSql.Ado.QueryAsync<List<T>>(sql,new { id });

        //    return [];
        //}
    }
}
