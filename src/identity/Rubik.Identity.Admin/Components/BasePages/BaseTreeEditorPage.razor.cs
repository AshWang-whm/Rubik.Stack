
using AntDesign.TableModels;
using Rubik.Identity.Share.Entity;
using Rubik.Infrastructure.Entity.BaseEntity;
using System.Linq.Expressions;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BaseTreeEditorPage<T>: BaseEditorPage<T>
        where T : BaseTreeEntity<T>, IFullEntity, new()
    {
        protected virtual Func<T, IEnumerable<T>> TreeChildren { get; set; } = item => item.Children;

        protected virtual Expression<Func<T,bool>>? ExtraWhereExpression { get; set; }

        public override async Task Query(QueryModel<T> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<T>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .WhereIf(ExtraWhereExpression!=null, ExtraWhereExpression)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            var ids = source.Select(a => a.ID).ToList();
            DataSource = ids.Count == 0 ? source : await FreeSql.Select<T>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;
        }

        protected override async Task<bool> BeforeSave()
        {
            if (string.IsNullOrWhiteSpace(Editor.Code))
            {
                await MessageService.Error("[Code] 不允许为空!");
                return false;
            }

            var exist = await FreeSql.Select<TbApplication>()
                .Where(a => a.Code == Editor.Code && a.IsDelete == false)
                .AnyAsync();
            if (exist)
            {
                await MessageService.Error($"[Code]:{Editor.Code} 已存在!");
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
            await MessageService.Success("保存成功", 1);
        }

        protected override async Task OnDelete(params T[] source)
        {
            if (source.Length == 0)
            {
                await MessageService.Warning("没有要删除的数据!");
                return;
            }

            await FreeSql.Update<T>()
                .Set(a => a.IsDelete == true)
                .SetSource(source)
                .ExecuteAffrowsAsync();

            foreach (var item in source)
            {
                item.Parent?.Children.Remove(item);
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}
