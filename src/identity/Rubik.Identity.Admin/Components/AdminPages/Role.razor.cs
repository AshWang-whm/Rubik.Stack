using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Role : BaseTreeEditorPage<TbRole>
    {

        public override async Task Query(QueryModel<TbRole> query)
        {
            var exp = query.GetFilterExpressionOrNull();
            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbRole>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
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

            Total = (int)total;
        }

        void OnShowAppModal<TPage>(TbRole role, string title)
            where TPage : IComponent
        {
            void buildModal(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<TPage>(0);
                builder.AddAttribute(1, "RoleID", role.ID);
                builder.CloseComponent();
            }

            var @ref = ModalService!.CreateModal(new ModalOptions
            {
                Content = buildModal,
                Title = $"查看 [{role.Name}] {title}",
                Keyboard = true,
                Visible = true,
                Centered = true,
                MaskClosable = true,
                Maximizable = true,
                Width = "75vw;",
                DestroyOnClose = true,
            });
        }
    }
}
