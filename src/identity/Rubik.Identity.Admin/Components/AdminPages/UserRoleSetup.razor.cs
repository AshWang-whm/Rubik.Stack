using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using AntDesign;
using AntDesign.TableModels;
using Rubik.Share.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class UserRoleSetup:ComponentBase
    {
        [Parameter]
        public int[] Users { get; set; } = [];

        [Inject]
        public required IFreeSql FreeSql { get; set; }

        [Inject]
        public required IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public required IMessageService MessageService { get; set; }

        protected List<AppRoleTreeEntity> DataSource = [];

        string[] SelectedKeys { get; set; } = [];

        protected override async Task OnParametersSetAsync()
        {
            // 获取应用，角色
            var apps = await FreeSql!.Select<TbApplication>()
                .Where(a=>a.IsDelete==false)
                .OrderBy(a=>a.Sort)
                .ToListAsync(a=>new TbApplication
                {
                    ID=a.ID,
                    Name=a.Name
                });



            // 单个用户显示权限

        }

    }

    public class AppRoleTreeEntity: ITreeEntity<AppRoleTreeEntity>
    {
        public int ID { get; set; }

        public AppRoleTreeNodeType Type { get; set; } = AppRoleTreeNodeType.Application;

        public string? Name { get; set; }
        public int? ParentID { get; set; }
        public AppRoleTreeEntity? Parent { get; set; }
        public List<AppRoleTreeEntity> Children { get; set; } = [];
    }

    public enum AppRoleTreeNodeType
    {
        Application,
        Role
    }
}
