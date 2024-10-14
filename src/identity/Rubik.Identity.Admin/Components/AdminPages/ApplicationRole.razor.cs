using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class ApplicationRole : BaseTreePage<TbApplicationRole>
    {
        [Parameter]
        public int ApplicationID { get; set; }

        bool EditRolePermissionVisiable { get; set; } = false;

        List<TbApplicationPermission> TreePermissions { get; set; } = [];
        string[] SelectedKeys { get; set; } = [];
        TbApplicationRole? TbApplicationRole { get; set; }

        public override async Task Query(QueryModel<TbApplicationRole> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbApplicationRole>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a=>a.ApplicationID== ApplicationID)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            // 递归查询子节点
            var ids = source.Select(a => a.ID).ToList();

            DataSource = ids.Count == 0 ? source : await FreeSql.Select<TbApplicationRole>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;
        }

        protected override Task<bool> BeforeSave()
        {
            Editor.ApplicationID = ApplicationID;
            return base.BeforeSave();
        }

        protected override async Task AfterSave()
        {
            TreePermissions.Clear();
            SelectedKeys = [];
            await base.AfterSave();
        }

        async Task OnShowRolePermissions(TbApplicationRole context)
        {
            // 
            TbApplicationRole=context;
            if (TreePermissions.Count==0)
            {
                TreePermissions = await FreeSql.Select<TbApplicationPermission>()
                    .Where(a => a.ApplicationID == context.ApplicationID && a.IsDelete == false && a.ParentID == null)
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();
            }

            SelectedKeys = [.. (await FreeSql.Select<TbApplicationRole, TbRelationRolePermission, TbApplicationPermission>()
                .LeftJoin((a, b, c) => a.ID == b.RoleID)
                .LeftJoin((a, b, c) => b.PermissionID == c.ID)
                .Where((a,b,c)=>a.ID==context.ID&&c.IsDelete==false)
                .ToListAsync((a, b, c) => c.ID.ToString()))];

            EditRolePermissionVisiable = true;
            await InvokeAsync(StateHasChanged);
        }


        async Task OnSaveRolePermission()
        {
            var role_permissions = SelectedKeys.Select(int.Parse)
                .Select(a=>new TbRelationRolePermission
                {
                    RoleID=TbApplicationRole!.ID,
                    PermissionID=a,
                    AddDate=DateTime.Now
                })
                .ToList();

            if(role_permissions.Count > 0)
            {
                var uow = FreeSql.CreateUnitOfWork();
                try
                {
                    await uow.Orm.Delete<TbRelationRolePermission>()
                        .Where(a => a.RoleID == TbApplicationRole!.ID)
                        .ExecuteAffrowsAsync();

                    await uow.Orm.Insert(role_permissions)
                        .ExecuteAffrowsAsync();
                    uow.Commit();
                    await MessageService.Info("保存成功!", 0.5);

                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    await MessageService.Info($"保存失败:{ex.Message}", 1);

                }

            }

            EditRolePermissionVisiable = false;
            SelectedKeys = [];
        }
    }
}
