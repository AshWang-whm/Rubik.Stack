using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using System.Linq.Expressions;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class ApplicationPermission : BaseTreeEditorPage<TbApplicationPermission>
    {
        [Parameter]
        public int ApplicationID { get; set; }

        EventCallback<PermissionType> PermissionTypeChangeCallback { get; set; }

        protected override void OnInitialized()
        {
            ExtraWhereExpression = (a) => a.ApplicationID == ApplicationID;
            base.OnInitialized();
        }

        public override async Task Query(QueryModel<TbApplicationPermission> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbApplicationPermission>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.ApplicationID == ApplicationID)
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
            Editor.ApplicationID = ApplicationID;
            return await base.BeforeSave();
        }
    }
}
