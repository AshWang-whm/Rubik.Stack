﻿using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class OrganizationJob : BaseEditorPage<TbOrganizationJob>
    {

        [Parameter]
        public int OrganizationID { get; set; }

        public override async Task Query(QueryModel<TbOrganizationJob> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            // 顶级的数据作为total数据分页统计
            DataSource = await FreeSql.Select<TbOrganizationJob>()
                .WhereIf(exp != null, exp)
                //.WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
                .Where(a => a.OrganizationID == OrganizationID)
                .Count(out var total)
                .ToListAsync();

            Total = (int)total;
        }

        protected override async Task<bool> BeforeSave()
        {
            Editor.OrganizationID= OrganizationID;
            return true;
        }
    }
}
