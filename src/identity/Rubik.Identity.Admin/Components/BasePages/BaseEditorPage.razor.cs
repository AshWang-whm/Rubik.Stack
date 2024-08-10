using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components.Forms;
using Rubik.Share.Entity.BaseEntity;
using Rubik.Share.Utils.Objects;
using Rubik.Share.Entity;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BaseEditorPage<T> : BasePage
        where T : BaseTreeEntity<T>, IFullEntity, new()
    {
        protected string EditFormSectionID = Guid.NewGuid().ToString();

        protected List<T> DataSource = [];
        protected List<T> SelectedRows = [];
        protected int Total;

        protected Table<T>? Table {  get; set; }
        protected T Editor { get; set; } = new();

        protected bool EditorModalVisiable = false;

        public abstract Task Query(QueryModel<T> query);

        protected virtual Func<T, IEnumerable<T>> TreeChildren { get; set; } = item => [];

        protected virtual string[] HideColumns { get; set; } = [];
        
        protected virtual async Task OnRefresh()
        {
            await Query(Table!.BuildQueryModel());
        }
        protected virtual void OnNew()
        {
            Editor = new T();
            EditorModalVisiable = true;
        }

        protected virtual async Task OnDeleteSelectedRow()
        {
            await OnDelete([.. SelectedRows]);
        }

        protected virtual bool BeforeSave()
        {
            return true;
        }

        protected virtual async Task AfterSave()
        {
            await OnRefresh();
        }

        protected virtual async Task OnSubmitForm(EditContext context)
        {
            await OnSave();
            EditorModalVisiable = false;
        }

        protected virtual async Task OnSubmitFailed(EditContext context)
        {
            await MessageService.Error("提交失败");
        }

        protected async Task OnSave()
        {
            if (Editor == null)
            {
                await MessageService.Error("没有要保存的数据！");
            }
            else
            {
                if (!BeforeSave())
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

            EditorModalVisiable = false;
            await AfterSave();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="clone">是否深拷贝</param>
        /// <param name="options"></param>
        protected void OnEdit(T obj,Action<T>? options = null)
        {
            //Editor =clone? obj.TreeCopy():obj;
            // 每次同步引用，若不保存数据需要恢复原始状态,todo:
            Editor=obj;
            options?.Invoke(obj);
            //await InvokeAsync(StateHasChanged);
            EditorModalVisiable = true;
        }

        protected async Task OnDelete(params T[] source)
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
