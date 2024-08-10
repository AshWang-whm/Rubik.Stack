using Rubik.Share.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BaseTreePage<T>: BasePage<T>
        where T : BaseTreeEntity<T>, IFullEntity, new()
    {
        protected virtual Func<T, IEnumerable<T>> TreeChildren { get; set; } = item => [];

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
