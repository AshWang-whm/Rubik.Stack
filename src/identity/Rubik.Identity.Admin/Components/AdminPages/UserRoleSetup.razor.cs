using Microsoft.AspNetCore.Components;
using Rubik.Identity.Share.Entity;
using AntDesign;
using Rubik.Infrastructure.Entity.BaseEntity;
using static OneOf.Types.TrueFalseOrNull;
using FreeSql.DataAnnotations;

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

        protected List<TbRole> DataSource = [];

        protected TreeNode<TbRole>[] SelectedNodes = [];

        protected string[] CheckedKeys = [];

        /// <summary>
        /// 初始化的所选角色
        /// </summary>
        protected List<string> CopyCheckedKeys = [];

        protected override async Task OnInitializedAsync()
        {
            if(ModalRef != null)
                ModalRef.OnOk = OnSave;

            var source = await FreeSql.Select<TbRole>()
                .Where(a => a.ParentID == null&&a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            // 递归查询子节点
            var ids = source.Select(a => a.ID).ToList();

            DataSource = ids.Count == 0 ? source : await FreeSql.Select<TbRole>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .Distinct()
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            // 单个用户显示权限
            if (Users.Count == 1)
            {
                CheckedKeys = [..(await FreeSql.Select<TbRelationRoleUser>()
                    .Where(a => Users.Contains(a.UserID))
                    .ToListAsync(a => a.RoleID.ToString()))];

                CopyCheckedKeys.AddRange(CheckedKeys);
            }
        }

        async Task OnSave()
        {
            if (AreListsEqual(CheckedKeys, CopyCheckedKeys))
            {
                await ModalRef!.CloseAsync();
                await MessageService.SuccessAsync("没有要保存的数据", 1);
                return;
            }

            var uow = FreeSql!.CreateUnitOfWork();
            try
            {

                // 清除角色绑定数据
                await uow.Orm.Delete<TbRelationRoleUser>()
                    .Where(a => Users.Contains(a.UserID))
                    .ExecuteAffrowsAsync();

                // 绑定用户&角色数据   多人同时设置？判断有无变化？ TODO：
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
                await MessageService.SuccessAsync("保存成功",1);
            }
            catch (Exception ex)
            {
                await MessageService!.ErrorAsync(ex.Message, 2);
                uow.Rollback();
            }
        }

        static bool AreListsEqual(IEnumerable<string> list1, IEnumerable<string> list2)
        {
            // 检查元素数量是否相等
            if (list1.Count() != list2.Count())
            {
                return false;
            }

            // 使用 SequenceEqual 比较排序后的列表
            return list1.OrderBy(x => x).SequenceEqual(list2.OrderBy(x => x));
        }
    }
}
