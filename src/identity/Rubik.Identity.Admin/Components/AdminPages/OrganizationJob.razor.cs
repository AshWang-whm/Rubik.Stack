using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class OrganizationJob : BaseTreePage<TbOrganizationJob>
    {

        [Parameter]
        public int OrganizationID { get; set; }

        public override async Task Query(QueryModel<TbOrganizationJob> query)
        {
            var exp = query.GetQueryExpression();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbOrganizationJob>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
                .Where(a => a.OrganizationID == OrganizationID)
                .Count(out var total)
                .ToListAsync();

            var ids = source.Select(a => a.ID).ToList();
            DataSource = ids.Count == 0 ? source : await FreeSql.Select<TbOrganizationJob>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;
        }
    }
}
