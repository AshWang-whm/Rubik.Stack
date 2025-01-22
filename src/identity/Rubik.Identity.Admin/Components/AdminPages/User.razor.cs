using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;
using Rubik.Share.Extension;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class User: BaseEditorPage<TbUser>
    {
        [Inject]
        ModalService? ModalService { get; set; }

        List<TbOrganization> TreeOrganization = [];
        /// <summary>
        /// 选择的部门ID
        /// </summary>
        int SelectedOrganization = 0;

        List<TbPosition> TreePosition = [];
        /// <summary>
        /// 选择的职位ID
        /// </summary>
        IEnumerable<int> SelectedPositions = [];

        List<TbOrganizationJob> OrgainzationJobList = [];
        int? SelectedJob = null;
        bool ShowOrgModal { get; set; } = false;

        string? OrgTreeSelectKey { get; set; }

        readonly UserCompanyInfo UserCompanyInfo = new();

        public EventCallback<TbOrganization> UserOrgainzationSelectedEventCallback { get; set; }

        public override async Task Query(QueryModel<TbUser> query)
        {
            var user_exp = query.GetFilterExpressionOrNull();

            var table_exp = user_exp.ExpressionConvertToMultiGenerics<TbUser,TbRelationOrganizeUser, TbOrganization>();

            List<int> all_org_ids = [];
            if (SelectedOrganization != 0)
            {
                all_org_ids = await FreeSql.Select<TbOrganization>()
                    .Where(a=>a.ID==SelectedOrganization)
                    .AsTreeCte()
                    .ToListAsync(a => a.ID);
            }

            DataSource = await FreeSql.Select<TbUser,TbRelationOrganizeUser,TbOrganization>()
                .LeftJoin((a,b,c)=>a.ID==b.UserID)
                .LeftJoin((a,b,c)=>b.OrganizationID==c.ID)
                .WhereIf(user_exp != null, table_exp)
                .WhereIf(SelectedOrganization!=0, (a,b,c)=> all_org_ids.Contains(b.OrganizationID))
                .Where((a,b,c) => a.IsDelete == false)
                .Count(out var total)
                .OrderBy((a,b,c)=>a.Sort)
                .ToListAsync((a,b,c)=>new TbUser
                {
                    ID=a.ID,
                    Name=a.Name,
                    Department=c.Name,
                    Code=a.Code,
                    Email=a.Email,
                    Gender=a.Gender,
                    EntryDate=a.EntryDate,
                    ModifyDate=a.ModifyDate,
                    ModifyUser=a.ModifyUser
                });

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
            if(SelectedOrganization == 0)
            {
                await MessageService.Warning("请选择一个组织架构!");
                return;
            }
            SelectedPositions = [];
            await base.OnNew();
        }

        void OnOrgClick(TreeEventArgs<TbOrganization> e,bool reload_table=true)
        {
            SelectedOrganization = e.Node.DataItem.ID;
            if (reload_table)
            {
                Table!.ReloadData();
            }
        }

        async Task OnResetPwd()
        {
            if (!SelectedRows.Any())
            {
                await MessageService!.Error("请选择需要重置密码的用户!");
                return;
            }

            foreach (var row in SelectedRows)
            {
                var pwd = PasswordEncryptExtension.GeneratePasswordHash(row.Code!, row.Code!);

                await FreeSql.Update<TbUser>()
                    .Set(a => a.Password == pwd)
                    .Where(a => a.ID == row.ID)
                    .ExecuteAffrowsAsync();
            }
            await MessageService!.Success("重置密码完成!",1);
            SelectedRows = [];
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

        async Task OnEdit(TbUser user)
        {
            // 查询用户公司职称相关信息
            var user_org_position = await FreeSql!.Select<TbUser, TbRelationOrganizeUser, TbRelationPositionUser,TbRelationJobUser>()
                .LeftJoin((a, b, c, d) => a.ID == b.UserID)
                .LeftJoin((a, b, c, d) => a.ID == c.UserID)
                .LeftJoin((a, b, c, d) => a.ID == d.UserID)
                .Where(a=>a.t1.ID== user.ID)
                .Distinct()
                .ToListAsync((a, b, c, d) => new { b.OrganizationID, c.PositionID, d.JobID });

            SelectedOrganization = user_org_position.FirstOrDefault()?.OrganizationID??0;
            SelectedPositions = user_org_position.Select(a => a.PositionID);
            SelectedJob = user_org_position.FirstOrDefault()?.JobID ;

            UserCompanyInfo.PositionID = SelectedPositions.Distinct().ToList();
            UserCompanyInfo.OrganizationID = SelectedOrganization;
            UserCompanyInfo.JobID = SelectedJob;

            base.OnEdit(user);
        }

        protected override async Task<bool> BeforeSave()
        {
            if (string.IsNullOrWhiteSpace(Editor.Code))
            {
                await MessageService.Error("[Code] 不允许为空!",2);
                return false;
            }

            // 新增用户检查账号，已存在用户检查不同id有无相同code
            var exist = await FreeSql.Select<TbUser>()
                    .WhereIf(Editor.ID>0,a=>a.ID != Editor.ID)
                    .Where(a => a.Code == Editor.Code&& a.IsDelete == false)
                    .AnyAsync();

            if (exist)
            {
                await MessageService.Error($"[Code]:{Editor.Code} 已存在!",2);
                return false;
            }

            return true;
        }

        protected override async Task OnSave()
        {
            var before = await BeforeSave();

            if (!before)
                return;

            var isnew = Editor.ID == 0;

            var uow = FreeSql!.CreateUnitOfWork();
            try
            {
                // 生成密码,新用户默认密码等于账号
                if (isnew)
                {
                    Editor.Password = PasswordEncryptExtension.GeneratePasswordHash(Editor.Code!, Editor.Code!);
                    Editor.ID = (int)(await uow.Orm.Insert(Editor).ExecuteIdentityAsync());
                }
                else
                    await uow.Orm.Update<TbUser>()
                        .SetSource(Editor)
                        .UpdateColumns(a=>new {a.Name,a.Code,a.Email,a.Gender,a.EntryDate,a.Age,a.Sort})
                        .ExecuteAffrowsAsync();

                if (UserCompanyInfo.OrganizationID != SelectedOrganization)
                {
                    if(!isnew)
                    {
                        await uow.Orm.Delete<TbRelationOrganizeUser>()
                            .Where(a => a.UserID == Editor.ID)
                            .ExecuteAffrowsAsync();

                    }
                    await uow.Orm.Insert(new TbRelationOrganizeUser
                    {
                        OrganizationID= SelectedOrganization,
                        UserID= Editor.ID
                    }).ExecuteAffrowsAsync();
                }


                if (UserCompanyInfo.JobID != SelectedJob)
                {
                    if (!isnew)
                    {
                        await uow.Orm.Delete<TbRelationJobUser>()
                            .Where(a => a.UserID == Editor.ID)
                            .ExecuteAffrowsAsync();
                    }
                    // 绑定岗位
                    if (SelectedJob != null)
                        await uow.Orm.Insert(new TbRelationJobUser { JobID = SelectedJob.Value, UserID = Editor.ID })
                            .ExecuteAffrowsAsync();
                }


                if (UserCompanyInfo.PositionID.Except(SelectedPositions).Any())
                {
                    if (!isnew)
                    {
                        await uow.Orm.Delete<TbRelationPositionUser>()
                            .Where(a => a.UserID == Editor.ID)
                            .ExecuteAffrowsAsync();
                    }
                    // 绑定职位
                    var positions = SelectedPositions.Select(a => new TbRelationPositionUser
                    {
                        PositionID = a,
                        UserID = Editor.ID
                    });
                    await uow.Orm.Insert(positions).ExecuteAffrowsAsync();
                }


                uow.Commit();

                await AfterSave();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                await MessageService.Error(ex.Message,3);
            }
        }

        async Task OnClearUserRole()
        {
            var userids = SelectedRows.Select(a => a.ID);
            await FreeSql.Delete<TbRelationRoleUser>()
                .Where(a => userids.Contains(a.UserID))
                .ExecuteAffrowsAsync();
            await MessageService!.Success("删除用户角色成功!",1);
        }

        async Task OnComfirmChangeOrg()
        {
            var user_ids = SelectedRows.Select(a => a.ID).Distinct().ToArray();

            var uow = FreeSql.CreateUnitOfWork();
            try
            {
                await uow.Orm.Delete<TbRelationOrganizeUser>()
                    .Where(a => user_ids.Contains(a.UserID))
                    .ExecuteAffrowsAsync();

                var new_org_relations = user_ids.Select(a => new TbRelationOrganizeUser
                {
                    UserID=a,
                    OrganizationID = SelectedOrganization
                });
                await uow.Orm.Insert(new_org_relations).ExecuteAffrowsAsync();

                uow.Commit();

                foreach (var item in SelectedRows)
                {
                    DataSource.Remove(item);
                }
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                await MessageService.Error(ex.Message);
            }
        }

        void OnShowRoleSetup()
        {
            var users = SelectedRows.Select(a => a.ID).Distinct().ToList();
            ModalRef? @ref = null;

            void buildModal(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<UserRoleSetup>(0);
                builder.AddAttribute(1, "Users", users);
                builder.AddAttribute(2, "ModalRef", @ref);
                builder.CloseComponent();
            }

            @ref = ModalService!.CreateModal(new ModalOptions
            {
                Content = buildModal,
                Title = $"查看用户权限",
                Keyboard = true,
                Visible = true,
                Centered = true,
                MaskClosable = true,
                Maximizable = true,
                Width = "35vw;",
                DestroyOnClose = true,
                Draggable = true,
            });
        }
    }

    public class UserCompanyInfo
    {
        public int OrganizationID { get; set; }

        public List<int> PositionID { get; set; } = [];

        public int? JobID { get; set; }
    }
}
