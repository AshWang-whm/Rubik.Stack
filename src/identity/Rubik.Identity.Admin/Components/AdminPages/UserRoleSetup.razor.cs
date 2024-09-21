using Microsoft.AspNetCore.Components;
using Rubik.Identity.Share.Entity;
using AntDesign;
using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class UserRoleSetup:ComponentBase
    {
        [Parameter]
        public List<int> Users { get; set; } = [];

        [Parameter]
        public ModalRef? ModalRef { get; set; }

        [Inject]
        public required IFreeSql FreeSql { get; set; }

        [Inject]
        public required IMessageService MessageService { get; set; }

        protected List<AppRoleTreeEntity> DataSource = [];

        protected TreeNode<AppRoleTreeEntity>[] SelectedNodes = [];

        protected string[] CheckedKeys = [];

        protected override async Task OnInitializedAsync()
        {
            if(ModalRef != null)
                ModalRef.OnOk = OnSave;

            // 获取应用，角色
            var apps = await FreeSql!.Select<TbApplication>()
                .Where(a => a.IsDelete == false)
                .OrderBy(a => a.Sort)
                .ToListAsync(a => new TbApplication
                {
                    ID = a.ID,
                    Name = a.Name
                });

            var roles = await FreeSql.Select<TbApplicationRole>()
                .Where(a => a.IsDelete == false)
                .ToListAsync(a => new TbApplicationRole
                {
                    ID = a.ID,
                    Name = a.Name,
                    ParentID = a.ParentID,
                    ApplicationID = a.ApplicationID,
                });

            DataSource = apps.Select(a => new AppRoleTreeEntity
            {
                ID = a.ID,
                Name = a.Name,
                Check=false
                // 区分app和role
            }).ToList();

            foreach (var app in DataSource)
            {
                var app_top_nodes = roles.Where(a => a.ApplicationID == app.ID && a.ParentID == null)
                    .Select(a => new AppRoleTreeEntity
                    {
                        ID = a.ID,
                        Name = a.Name,
                    }).ToList();

                RoleRecursion(app_top_nodes, roles);

                app.Children = app_top_nodes;
            }

            // 单个用户显示权限
            if (Users.Count == 1)
            {
                CheckedKeys = [.. (await FreeSql.Select<TbRelationRoleUser>()
                    .Where(a => Users.Contains(a.UserID))
                    .ToListAsync(a=>a.RoleID.ToString()))];
            }
        }

        async Task OnSave()
        {
            var uow = FreeSql!.CreateUnitOfWork();
            try
            {
                // 清除角色绑定数据
                await uow.Orm.Delete<TbRelationRoleUser>()
                    .Where(a => Users.Contains(a.UserID))
                    .ExecuteAffrowsAsync();

                // 绑定用户&角色数据   判断有无变化？ TODO：
                if (CheckedKeys.Length != 0)
                {
                    var role_user = CheckedKeys.Select(int.Parse).SelectMany(a => Users.Select(s => new TbRelationRoleUser
                    {
                        RoleID = a,
                        UserID = s
                    })).ToList();

                    await FreeSql.Insert(role_user).ExecuteAffrowsAsync();
                }

                uow.Commit();
                await ModalRef!.CloseAsync();
                await MessageService.Success("保存成功",1);
            }
            catch (Exception ex)
            {
                await MessageService!.Error(ex.Message, 2);
                uow.Rollback();
            }
        }

        static void RoleRecursion(List<AppRoleTreeEntity> parents,IEnumerable<TbApplicationRole> source)
        {
            foreach (var parent in parents)
            {
                var nodes = source.Where(a => a.ParentID == parent.ID)
                    .Select(a => new AppRoleTreeEntity
                    {
                        ID = a.ID,
                        Name = a.Name,
                        ParentID = a.ParentID,
                    }).ToList();

                RoleRecursion(nodes, source);

                parent.Children = nodes;
            }
        }

    }

    public class AppRoleTreeEntity: ITreeEntity<AppRoleTreeEntity>
    {
        public int ID { get; set; }

        public string? Key => ID.ToString();

        public bool Check { get; set; } = true;

        public string? Name { get; set; }
        public int? ParentID { get; set; }
        public AppRoleTreeEntity? Parent { get; set; }
        public List<AppRoleTreeEntity> Children { get; set; } = [];
    }

}
