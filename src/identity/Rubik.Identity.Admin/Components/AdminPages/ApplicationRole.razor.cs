using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class ApplicationRole : BaseTreePage<TbApplicationRole>
    {
        [Parameter]
        public int OrganizationID { get; set; }

        public override async Task Query(QueryModel<TbApplicationRole> query)
        {
            var exp = query.GetFilterExpression();

            DataSource = await FreeSql.Select<TbApplicationRole>()
                .WhereIf(exp != null, exp)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            Total = (int)total;
        }

    }
}
