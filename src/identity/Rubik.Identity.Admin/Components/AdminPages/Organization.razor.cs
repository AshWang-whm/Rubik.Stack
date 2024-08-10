using AntDesign;
using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Organization : BaseTreePage<TbOrganization>
    {
        public override async Task Query(QueryModel<TbOrganization> query)
        {
            var exp = query.GetQueryExpression();

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

        protected override async Task AfterSave()
        {
            // 新数据直接添加到DataSource
            if (Editor.ID == 0)
            {
                if (Editor.Parent != null)
                {
                    // 如果Children初始为0 ， 初次添加不会生成 + 号
                    Editor.Parent.Children.Add(Editor);
                }
                else
                {
                    DataSource.Add(Editor);
                }
            }
            EditorModalVisiable = false;
            await InvokeAsync(StateHasChanged);
            await MessageService.Success("保存成功",1);
        }


        protected override Func<TbOrganization, IEnumerable<TbOrganization>> TreeChildren => item => item.Children;
    }
}
