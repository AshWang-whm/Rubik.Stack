using AntDesign;
using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Position : BaseTreeEditorPage<TbPosition>
    {

        //public override async Task Query(QueryModel<TbPosition> query)
        //{
        //    var exp = query.GetFilterExpressionOrNull();

        //    // 顶级的数据作为total数据分页统计
        //    var source = await FreeSql.Select<TbPosition>()
        //        .WhereIf(exp != null, exp)
        //        .WhereIf(exp == null, a => a.ParentID == null)
        //        .Where(a => a.IsDelete == false)
        //        .Count(out var total)
        //        .ToListAsync();

        //    var ids = source.Select(a => a.ID).ToList();
        //    DataSource = ids.Count == 0 ? source : await FreeSql.Select<TbPosition>()
        //            .Where(a => a.IsDelete == false)
        //            .Where(a => ids.Contains(a.ID))
        //            .AsTreeCte()
        //            .OrderBy(a => a.Sort)
        //            .ToTreeListAsync();

        //    Total = (int)total;
        //}

        /// <summary>
        /// 岗位的Code不做验证
        /// </summary>
        /// <returns></returns>
        protected override Task<bool> BeforeSave()
        {
            return Task.FromResult(true);
        }
    }
}
