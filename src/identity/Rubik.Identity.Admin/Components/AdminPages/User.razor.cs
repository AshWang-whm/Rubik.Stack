using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class User: BaseEditorPage<TbUser>
    {
        List<TbOrganization> TreeOrganization = [];
        TbOrganization? ClickedOrganization = null;
        int SelectedOrganization = 0;
        //IEnumerable<int> SelectedOrganization = [];

        List<TbPosition> TreePosition = [];
        IEnumerable<int> SelectedPositions = [];

        List<TbOrganizationJob> OrgainzationJobList = [];
        int? SelectedJob = null;

        public EventCallback<TbOrganization> UserOrgainzationSelectedEventCallback { get; set; }

        public override async Task Query(QueryModel<TbUser> query)
        {
            var user_exp = query.GetFilterExpression();

            var table_exp = user_exp.ExpressionConvertToMultiGenerics<TbUser,TbRelationOrganizeUser>();

            var datasource = await FreeSql.Select<TbUser,TbRelationOrganizeUser>()
                .LeftJoin((a,b)=>a.ID==b.UserID)
                .WhereIf(user_exp != null, table_exp)
                .WhereIf(ClickedOrganization != null,(a,b)=>b.OrganizationID== ClickedOrganization!.ID)
                .Where((a,b) => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            Total = (int)total;
        }

        protected override async Task OnInitializedAsync()
        {
            UserOrgainzationSelectedEventCallback = EventCallback.Factory.Create<TbOrganization>(this,OnUserOrganizationSelectedChange);

            TreeOrganization = await FreeSql.Select<TbOrganization>()
                    .Where(a => a.IsDelete == false&&a.ParentID==null)
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .Where(a=>a.OrganizationType== OrganizationType.Department)
                    .ToTreeListAsync();

            TreePosition = await FreeSql.Select<TbPosition>()
                    .Where(a => a.IsDelete == false && a.ParentID == null)
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();
        }

        protected override async Task OnNew()
        {
            if(ClickedOrganization == null)
            {
                await MessageService.Warning("请选择一个组织架构!");
                return;
            }
            SelectedOrganization= ClickedOrganization.ID;

            SelectedPositions = [];

            //// 刷新部门岗位
            //TreeJob = await FreeSql!.Select<TbOrganizationJob>()
            //        .Where(a => a.IsDelete == false)
            //        .Where(a => a.OrganizationID == ClickedOrganization!.ID)
            //        .OrderBy(a => a.Sort)
            //        .ToListAsync();
            //SelectedJobs = [];

            await base.OnNew();
        }

        void OnOrgClick(TreeEventArgs<TbOrganization> e)
        {
            ClickedOrganization = e.Node.DataItem;
            Table!.ReloadData();
        }

        async Task OnResetPwd()
        {
            if (SelectedRows.Any())
            {
                await MessageService!.Error("请选择需要重置密码的用户!");
                return;
            }


        }

        async Task OnUserOrganizationSelectedChange(TbOrganization val)
        {
            // 这里treeselect 会触发2次： todo：

            OrgainzationJobList = await FreeSql!.Select<TbOrganizationJob>()
                .Where(a => a.IsDelete == false)
                .Where(a => a.OrganizationID == SelectedOrganization)
                .ToListAsync();
            SelectedJob = OrgainzationJobList.FirstOrDefault()?.ID;

            await InvokeAsync(StateHasChanged);
        }
        protected override async Task OnSave()
        {
            var before = await BeforeSave();

            if (!before)
                return;

            // 
            var uow = FreeSql!.CreateUnitOfWork();
            try
            {
                // 生成密码,新用户默认密码等于账号
                if (Editor.ID == 0)
                {
                    Editor.Password = PasswordEncryptExtension.GeneratePasswordHash(Editor.Code!, Editor.Code!);
                    Editor.ID = (int)(await uow.Orm.Insert(Editor).ExecuteIdentityAsync());
                }
                else
                    await uow.Orm.Update<TbUser>()
                        .SetSource(Editor)
                        .ExecuteAffrowsAsync();

                // 绑定组织架构
                await uow.Orm.Insert(new TbRelationOrganizeUser
                {
                    OrganizationID= ClickedOrganization!.ID,
                    UserID= Editor.ID
                }).ExecuteAffrowsAsync();

                // 绑定岗位
                if(SelectedJob != null)
                    await uow.Orm.Insert(new TbRelationJobUser { JobID=SelectedJob.Value,UserID=Editor.ID})
                        .ExecuteAffrowsAsync();

                // 绑定职位
                var positions = SelectedPositions.Select(a => new TbRelationPositionUser
                {
                    PositionID=a,
                    UserID=Editor.ID
                });
                await uow.Orm.Insert(positions).ExecuteAffrowsAsync();

                uow.Commit();
            }
            catch (Exception ex)
            {

                uow.Rollback();
            }
        }
    }
}
