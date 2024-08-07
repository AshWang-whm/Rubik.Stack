using AntDesign;
using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Organization : BaseEditorPage<TbOrganization>
    {
        protected override string[] HideColumns { get; set; } = [nameof(TbOrganization.Jobs)];

        public override async Task Query(QueryModel<TbOrganization> query)
        {
            var exp = query.GetQueryExpression();

            var source = await FreeSql.Select<TbOrganization>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            // 递归查询子节点
            var ids = source.Select(a => a.ID).ToList();
            if (ids.Count != 0)
            {
                var sub = FreeSql.Select<TbOrganization>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.Parent.ID))
                    .ToTreeList();

                // 递归写入
                DataSource = source.Recursion(sub);
            }
            else
                DataSource = source;


            Total = (int)total;
        }

        protected override async Task AfterSave()
        {
            // 直接添加到数据源
            if(Editor.Parent != null)
            {
                Editor.Parent.Children.Add(Editor);
            }
            else
            {
                DataSource.Add(Editor);
            }
            EditorModalVisiable = false;
            await MessageService.Success("保存成功");
        }


        protected override Func<TbOrganization, IEnumerable<TbOrganization>> TreeChildren => item => item.Children;
    }
}
