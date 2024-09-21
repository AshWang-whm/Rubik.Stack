using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BaseEditorPage<T> : BasePage<T>
        where T : BaseFullEntity, IFullEntity, new()
    {
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

            await InvokeAsync(StateHasChanged);
        }
    }
}
