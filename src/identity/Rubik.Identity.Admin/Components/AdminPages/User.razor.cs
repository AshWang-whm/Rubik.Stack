using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class User: BaseEditorPage<TbUser>
    {
        public override async Task Query(QueryModel<TbUser> query)
        {
            var exp = query.GetQueryExpression();

            // 顶级的数据作为total数据分页统计
            DataSource = await FreeSql.Select<TbUser>()
                .WhereIf(exp != null, exp)
                //.WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            Total = (int)total;
        }
    }
}
