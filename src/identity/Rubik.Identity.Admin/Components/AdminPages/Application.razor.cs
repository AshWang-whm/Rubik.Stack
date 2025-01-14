using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Application: BaseEditorPage<TbApplication>
    {
        [Inject]
        ModalService? ModalService { get; set; }

        EventCallback<OidcAppType> OidcAppTypeChangeCallback { get; set; }

        List<string?> Scopes = [];
        public override async Task Query(QueryModel<TbApplication> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            DataSource = await FreeSql.Select<TbApplication>()
                .WhereIf(exp != null, exp)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            Total = (int)total;
        }

        void OnShowAppModal<TPage>(TbApplication app,string title)
            where TPage : IComponent
        {
            void buildModal(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<TPage>(0);
                builder.AddAttribute(1, "ApplicationID", app.ID);
                builder.CloseComponent();
            }

            var @ref = ModalService!.CreateModal(new ModalOptions
            {
                Content = buildModal,
                Title = $"查看 [{app.Name}] {title}",
                Keyboard = true,
                Visible = true,
                Centered = true,
                MaskClosable = true,
                Maximizable = true,
                Width = "75vw;",
                DestroyOnClose = true,
            });
        }

        protected override async Task OnInitializedAsync()
        {
            await OnLoadAllCode();
            OidcAppTypeChangeCallback = EventCallback.Factory.Create<OidcAppType>(this, OnOidcAppTypeChange);
        }

        protected override Task OnNew()
        {
            base.OnNew();
            Editor.CallbackPath = "/oidc/callback";
            return Task.CompletedTask;
        }

        protected override async Task AfterSave()
        {
            if (!Scopes.Any(a => Editor.Code == a))
            {
                Scopes.Add(Editor.Code);
            }
            await OnLoadAllCode();
            await base.AfterSave();
        }

        async Task OnLoadAllCode()
        {
            Scopes = await FreeSql.Select<TbApplication>()
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync(a => a.Code);
        }

        void OnOidcAppTypeChange(OidcAppType appType)
        {
            if(appType!= OidcAppType.MVC&&appType!= OidcAppType.FrontEnd)
            {
                Editor.CallbackPath = null;
                Editor.RedirectUri = null;
            }
            else
            {
                Editor.CallbackPath = "/oidc/callback";
            }

            if (appType != OidcAppType.MVC && appType != OidcAppType.Client) 
            { 
                Editor.ClientSecret = null;
            }

        }
    }
}
