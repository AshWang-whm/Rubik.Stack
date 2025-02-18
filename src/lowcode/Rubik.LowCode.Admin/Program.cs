using Rubik.Identity.FreesqlExtension;
using Rubik.Identity.UserIdentity;
using Rubik.Infrastructure.WebExtension;
using Rubik.LowCode.Admin.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBootstrapBlazor();


builder.AddEnvironmentJsonFile();

builder.AddFreesqlWithIdentityAop("lowcode_admin", FreeSql.DataType.PostgreSQL, cmd =>
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
});

builder.Services.AddUserIdentity();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
