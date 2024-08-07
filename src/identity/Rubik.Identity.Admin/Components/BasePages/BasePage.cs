using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public abstract class BasePage : ComponentBase
    {
        [Inject]
        public required IFreeSql FreeSql { get; set; }

        [Inject]
        public required IHttpContextAccessor HttpContextAccessor { get; set; }

        [Inject]
        public required IMessageService MessageService { get; set; }

    }
}
