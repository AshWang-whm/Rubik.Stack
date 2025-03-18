using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using Rubik.Infrastructure.Utils.Common.ExpressionTrees;
using Rubik.Infrastructure.Utils.Common.Objects;
using Rubik.Infrastructure.Utils.Common.String;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class ApiResource:BaseEditorPage<TbApiResource>
    {
        [Inject]
        ModalService? ModalService { get; set; }
        List<TbApiScope> DataSource_ApiScope { get; set; } = [];
        IEnumerable<TbApiScope> SelectedRows_ApiScope { get; set; } = [];

        bool EditorModalVisiable_ApiScope { get; set; } = false;

        TbApiScope Editor_ApiScope { get; set; }=new TbApiScope();

        string[] ClaimTypes { get; set; } = ["role","job","pos","dept"];

        IEnumerable<string> Editor_ApiScope_ClaimValues { get; set; } = [];

        public override async Task Query(QueryModel<TbApiResource> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            var filter = exp?.ExpressionConvertToMultiGenerics<TbApiResource,TbApiScope>();

            DataSource = await FreeSql.Select<TbApiResource,TbApiScope>()
                .LeftJoin((a,b)=>a.ID==b.ApiID)
                .WhereIf(exp != null, filter)
                .Where((a,b) => a.IsDelete == false&&b.IsDelete==false)
                .Count(out var total)
                .GroupBy((a, b) => new { a.ID, a.Name, a.Code })
                .ToListAsync(a => new TbApiResource
                {
                    ID = a.Key.ID,
                    Name = a.Value.Item1.Name,
                    Code = a.Value.Item1.Code,
                    ModifyDate = a.Value.Item1.ModifyDate,
                    ModifyUser = a.Value.Item1.ModifyUser,
                    Scopes = a.Value.Item2.Code.StringAgg(",")
                });

            Total = (int)total;
        }


        protected async Task OnEditApiScope(TbApiResource obj)
        {
            base.OnEdit(obj);

            await OnEditApiScope(obj.ID);
        }

        async Task OnEditApiScope(int apiid)
        {
            DataSource_ApiScope = await FreeSql.Select<TbApiScope>()
                .Where(a => apiid == a.ApiID)
                .Where(a=>a.IsDelete==false)
                .ToListAsync();
        }

        async Task OnDeleteApiScope(IEnumerable<TbApiScope> scopes)
        {
            if (!scopes.Any())
                return;

            await FreeSql.Delete<TbApiScope>()
                .Where(a => scopes.Select(s => s.ID).Contains(a.ID))
                .ExecuteAffrowsAsync();

            await OnEditApiScope(scopes.First().ApiID);
        }

        async Task OnEditScope(TbApiScope scope)
        {
            EditorModalVisiable_ApiScope = true;
            Editor_ApiScope = scope.DeepCopy();
            Editor_ApiScope_ClaimValues = Editor_ApiScope.Claims?.Split(' ') ?? [];
        }

        async Task OnSaveScope()
        {
            Editor_ApiScope.Claims = Editor_ApiScope_ClaimValues.StringJoin(' ');
            await FreeSql.InsertOrUpdate<TbApiScope>()
                .SetSource(Editor_ApiScope)
                .ExecuteAffrowsAsync();

            await OnEditApiScope(Editor_ApiScope.ApiID);

            EditorModalVisiable_ApiScope = false;
        }
    }
}
