using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BasePage<T> : ComponentBase
        where T : BaseFullEntity, IFullEntity, new()
    {
        [Inject]
        public required IFreeSql FreeSql { get; set; }

        [Inject]
        public required IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public required IMessageService MessageService { get; set; }

        [Inject]
        public required ModalService ModalService { get; set; }


        protected List<T> DataSource = [];
        protected IEnumerable<T> SelectedRows = [];
        protected int Total;

        protected Table<T>? Table { get; set; }
        

        public abstract Task Query(QueryModel<T> query);

        protected virtual async Task OnRefresh()
        {
            await Query((Table!.GetQueryModel() as QueryModel<T>)!);
        }
        
        protected virtual async Task OnDeleteSelectedRow()
        {
            await OnDelete([.. SelectedRows]);
        }

        
        protected virtual async Task OnDelete(params T[] source)
        {
            if (source.Length == 0)
            {
                await MessageService.WarningAsync("没有要删除的数据!");
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
