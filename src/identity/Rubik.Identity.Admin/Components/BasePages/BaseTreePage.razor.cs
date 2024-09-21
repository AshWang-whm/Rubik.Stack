
using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BaseTreePage<T>: BasePage<T>
        where T : BaseTreeEntity<T>, IFullEntity, new()
    {
        protected virtual Func<T, IEnumerable<T>> TreeChildren { get; set; } = item => item.Children;

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
