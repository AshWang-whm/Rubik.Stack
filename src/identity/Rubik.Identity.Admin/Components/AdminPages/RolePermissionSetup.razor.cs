using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using Rubik.Infrastructure.Utils.Common.ExpressionTrees;
using System;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class RolePermissionSetup: BaseTreeQueryPage<TbApplicationPermission>
    {
        [Parameter]
        public int RoleID { get; set; }

        List<TbApplication> ApplicationList = [];
        int ApplicationTotal { get; set; }

        int? SelectedApp = null;
        Table<TbApplication>? ApplicationTable { get; set; }

        /// <summary>
        /// 当前角色下，已保存的历史APP的权限
        /// </summary>
        List<int> HistoryRolePermissionIDList = [];

        public override async Task Query(QueryModel<TbApplicationPermission> query)
        {
            if (SelectedApp == null)
                return;

            var exp = query.GetFilterExpressionOrNull();

            var table_exp = exp.ExpressionConvertToMultiGenerics<TbApplicationPermission, TbRelationRolePermission>();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbApplicationPermission, TbRelationRolePermission>()
                .WhereIf(exp != null, table_exp)
                .WhereIf(exp == null, (a,b) => a.ParentID == null)
                .Where((a, b) => a.ApplicationID == SelectedApp&&b.RoleID==RoleID)
                .Where((a, b) => !a.IsDelete)
                .Count(out var total)
                .Page(query.PageIndex,query.PageSize)
                .ToListAsync();

            // 递归查询子节点
            var ids = source.Select(a => a.ID).ToList();

            DataSource = ids.Count == 0 ? source : await FreeSql.Select<TbApplicationPermission>()
                    .Where(a => !a.IsDelete)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;

            HistoryRolePermissionIDList = await FreeSql.Select<TbApplicationPermission, TbRelationRolePermission>()
                .LeftJoin((a, b) => a.ID == b.PermissionID)
                .Where((a, b) => b.RoleID == RoleID && a.ApplicationID == SelectedApp)
                .ToListAsync((a, b) => b.PermissionID);

            SelectedRows = DataSource.Where(a => HistoryRolePermissionIDList.Contains(a.ID));
        }

        void OnExpand(RowData<TbApplicationPermission> row)
        {
            if (!row.Expanded||row.Children==null)
                return;
            foreach (var item in row.Children)
            {
                item.Value.Selected = HistoryRolePermissionIDList.Contains(item.Value.Data.ID);
            }
        }


        void ApplicationTableRowClick(RowData<TbApplication> row)
        {
            SelectedApp = row.Data.ID;
            Table!.ReloadData();
        }

        async Task OnSavePermission()
        {
            var now_ids = SelectedRows.Select(a => a.ID);

            // 更新角色权限
            var uow = FreeSql.CreateUnitOfWork();
            try
            {
                // 删除 取消选中的id
                var cancel_ids = HistoryRolePermissionIDList.Except(now_ids).ToList();
                if (cancel_ids.Count > 0)
                {
                    await uow.Orm.Delete<TbRelationRolePermission>()
                        .Where(a => a.RoleID == RoleID)
                        .Where(a => cancel_ids.Contains(a.PermissionID))
                        .ExecuteAffrowsAsync();
                }
                

                // 新增 新选中的id
                var new_permissions = now_ids.Except(HistoryRolePermissionIDList)
                    .Select(a=>new TbRelationRolePermission
                    {
                        RoleID=RoleID,
                        PermissionID=a
                    })
                    .ToList();

                if(new_permissions.Count>0)
                    await uow.Orm.Insert(new_permissions).ExecuteAffrowsAsync();

                uow.Commit();
                await MessageService.SuccessAsync("保存成功!");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                await MessageService.ErrorAsync(ex.Message);
            }
            


        }


        protected override async Task OnInitializedAsync()
        {
            ApplicationList = await FreeSql.Select<TbApplication>()
                .Where(a => !a.IsDelete)
                .OrderBy(a=>a.AddDate)
                .Count(out var ApplicationTotal)
                .ToListAsync();
        }
    }
}
