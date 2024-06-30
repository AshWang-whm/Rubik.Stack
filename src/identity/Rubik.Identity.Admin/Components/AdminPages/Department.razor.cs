using Microsoft.FluentUI.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Admin.Extensions;
using Rubik.Identity.Share.Entity;


namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Department:BaseOrmPage
    {
        List<TreeViewItem> Departments { get; set; } = [];

        List<TreeViewItem> DepartmentPosts { get; set; } = [];

        bool IsShowDelete { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var depts = await FreeSql.Select<TbDepartment>()
                .WhereIf(!IsShowDelete,a => !a.IsDelete)
                .ToListAsync();

            Departments = depts.ToTreeViewItems();

        }

        //async Task RefreshDepartmentPost(TreeViewItem department)
        //{
        //    var deptid = Convert.ToInt32(department.Id);
        //}
    }
}
