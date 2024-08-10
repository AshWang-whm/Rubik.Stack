﻿using AntDesign;
using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Organization : BaseEditorPage<TbOrganization>
    {
        protected override string[] HideColumns { get; set; } = [nameof(TbOrganization.Jobs)];

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

        protected override async Task AfterSave()
        {
            // 新数据直接添加到DataSource
            if (Editor.ID == 0)
            {
                if (Editor.Parent != null)
                {
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
