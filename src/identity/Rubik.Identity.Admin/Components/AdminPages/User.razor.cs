using AntDesign;
using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;
using Rubik.Identity.Share.Extension;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class User: BaseEditorPage<TbUser>
    {
        List<TbOrganization> TreeOrganization = [];
        TbOrganization? _current = null;

        public override async Task Query(QueryModel<TbUser> query)
        {
            var user_exp = query.GetFilterExpression();

            var table_exp = user_exp.ExpressionConvertToMultiGenerics<TbUser,TbRleationOrganizeUser>();

            var datasource = await FreeSql.Select<TbUser,TbRleationOrganizeUser>()
                .LeftJoin((a,b)=>a.ID==b.UserID)
                .WhereIf(user_exp != null, table_exp)
                .WhereIf(_current!=null,(a,b)=>b.OrganizationID==_current!.ID)
                .Where((a,b) => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            Total = (int)total;
        }

        protected override async Task OnInitializedAsync()
        {
            TreeOrganization = await FreeSql.Select<TbOrganization>()
                    .Where(a => a.IsDelete == false&&a.ParentID==null)
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();
        }

        void OnOrgClick(TreeEventArgs<TbOrganization> e)
        {
            _current = e.Node.DataItem;
            Table!.ReloadData();
        }

        async Task OnResetPwd()
        {
            if (SelectedRows.Any())
            {
                await MessageService!.Error("请选择需要重置密码的用户!");
                return;
            }


        }
    }
}
