using Microsoft.AspNetCore.Components.Forms;
using Rubik.Infrastructure.Entity.BaseEntity;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract partial class BaseEditorPage<T> : BasePage<T>
        where T : BaseFullEntity, IFullEntity, new()
    {
        protected T Editor { get; set; } = new();

        protected bool EditorModalVisiable = false;

        protected virtual Task OnNew()
        {
            Editor = new T();
            EditorModalVisiable = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 保存前校验
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<bool> BeforeSave()
        {
            if (string.IsNullOrWhiteSpace(Editor.Code))
            {
                await MessageService.ErrorAsync("[Code] 不允许为空!");
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
            await MessageService.ErrorAsync("提交失败");
        }

        protected virtual async Task OnSave()
        {
            if (Editor == null)
            {
                await MessageService.ErrorAsync("没有要保存的数据！");
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
    }
}
