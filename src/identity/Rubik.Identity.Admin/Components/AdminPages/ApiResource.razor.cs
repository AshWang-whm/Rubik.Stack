using AntDesign;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class ApiResource:BaseEditorPage<TbApiResource>
    {
        [Inject]
        ModalService? ModalService { get; set; }


    }
}
