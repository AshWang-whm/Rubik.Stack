using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Rubik.Share.Entity.BaseEntity;

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



        protected List<T> DataSource = [];
        protected IEnumerable<T> SelectedRows = [];
        protected int Total;

        protected Table<T>? Table { get; set; }
        protected T Editor { get; set; } = new();

        protected bool EditorModalVisiable = false;

        public abstract Task Query(QueryModel<T> query);

        protected virtual async Task OnRefresh()
        {
            await Query(Table!.BuildQueryModel());
        }
        protected virtual Task OnNew()
        {
            Editor = new T();
            EditorModalVisiable = true;
            return Task.CompletedTask;
        }

        protected virtual async Task OnDeleteSelectedRow()
        {
            await OnDelete([.. SelectedRows]);
        }

        /// <summary>
        /// 保存前校验
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> BeforeSave()
        {
            if (string.IsNullOrWhiteSpace(Editor.Code))
            {
                await MessageService.Error("[Code] 不允许为空!");
                return false;
            }

            return true;
        }

        protected virtual async Task AfterSave()
        {
            EditorModalVisiable = false;
            await OnRefresh();
        }

        protected virtual async Task OnSubmitFailed(EditContext context)
        {
            await MessageService.Error("提交失败");
        }

        protected virtual async Task OnSave()
        {
            if (Editor == null)
            {
                await MessageService.Error("没有要保存的数据！");
            }
            else
            {
                if (!await BeforeSave())
                {
                    return;
                }
                else
                {
                    // free sql aop上需要判断 Insert / Update ， InsertOrUpdate 无法判断是新增或是更改
                    if (Editor.ID == 0)
                        await FreeSql.Insert(Editor).ExecuteAffrowsAsync();
                    else
                    {
                        await FreeSql.Update<T>()
                            .SetSource(Editor)
                            .ExecuteAffrowsAsync();
                    }
                }
            }

            await AfterSave();
        }

        protected virtual void OnEdit(T obj, Action<T>? options = null)
        {
            //Editor =clone? obj.TreeCopy():obj;
            // 每次同步引用，若不保存数据需要恢复原始状态,todo:
            Editor = obj;
            options?.Invoke(obj);
            EditorModalVisiable = true;
        }
        protected virtual async Task OnDelete(params T[] source)
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

            //foreach (var item in source)
            //{
            //    item.Parent?.Children.Remove(item);
            //}
            await InvokeAsync(StateHasChanged);
        }
    }
}
