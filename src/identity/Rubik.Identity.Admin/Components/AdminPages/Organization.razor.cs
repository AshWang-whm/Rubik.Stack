using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Organization : BaseTreePage<TbOrganization>
    {
        [Inject]
        ModalService? ModalService { get; set; }

        public override async Task Query(QueryModel<TbOrganization> query)
        {
            var exp = query.GetFilterExpression();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<TbOrganization>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            // 递归查询子节点
            var ids = source.Select(a => a.ID).ToList();

            DataSource = ids.Count==0 ? source: await FreeSql.Select<TbOrganization>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;
        }

        protected override async Task<bool> BeforeSave()
        {
            if(string.IsNullOrWhiteSpace(Editor.Code))
            {
                await MessageService.Error("[Code] 不允许为空!");
                return false;
            }

            var exist = await FreeSql.Select<TbApplication>()
                .Where(a => a.Code == Editor.Code && a.IsDelete == false)
                .AnyAsync();
            if (exist)
            {
                await MessageService.Error($"[Code]:{Editor.Code} 也存在!");
                return false;
            }

            return true;
        }

        void OnShowOrganizePositionModal(TbOrganization org)
        {
            RenderFragment buildModal = (builder) =>
            {
                builder.OpenComponent<OrganizationJob>(0);
                builder.AddAttribute(1, "OrganizationID", org.ID);
                builder.CloseComponent();
            };

            var @ref = ModalService!.CreateModal(new ModalOptions
            {
                Content= buildModal,
                Title=$"查看 [{org.Name}] 岗位",
                Keyboard=true,
                Visible=true,
                Centered=true,
                MaskClosable=true,
                Maximizable=true,
                Width="75vw;"
            });

            @ref.OnClose = async () =>
            {
                System.Diagnostics.Debug.WriteLine("close");
            };
        }
    }
}
