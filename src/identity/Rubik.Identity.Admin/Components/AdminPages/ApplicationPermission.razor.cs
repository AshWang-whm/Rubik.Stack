using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class ApplicationPermission : BaseTreePage<TbApplicationPermission>
    {
        [Parameter]
        public int OrganizationID { get; set; }

        EventCallback<PermissionType> PermissionTypeChangeCallback { get; set; }

        public override async Task Query(QueryModel<TbApplicationPermission> query)
        {
            var exp = query.GetFilterExpression();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbApplicationPermission>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            // 递归查询子节点
            var ids = source.Select(a => a.ID).ToList();

            DataSource = ids.Count == 0 ? source : await FreeSql.Select<TbApplicationPermission>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;
        }

        void OnPermissionTypeChange(PermissionType permission)
        {
            if(permission!= PermissionType.Page)
            {
                Editor.Url = null;
                Editor.PageType = null;
            }
            else
            {
                Editor.PageType = PageType.Internal;
            }
        }

        protected override void OnParametersSet()
        {
            PermissionTypeChangeCallback = EventCallback.Factory.Create<PermissionType>(this, OnPermissionTypeChange);
            base.OnParametersSet();
        }

        protected override async Task<bool> BeforeSave()
        {
            if(Editor.PermissionType== PermissionType.Page&&string.IsNullOrWhiteSpace(Editor.Url))
            {
                await MessageService.Error("[Url] 不允许为空!");
                return false;
            }

            return await base.BeforeSave();
        }
    }
}
