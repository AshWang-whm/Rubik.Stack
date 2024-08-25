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

            DataSource = await FreeSql.Select<TbApplicationPermission>()
                .WhereIf(exp != null, exp)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

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
