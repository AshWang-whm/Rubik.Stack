using Rubik.Identity.Admin.Components;
using Avd.Infrastructure.Freesql;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.AddEnvironmentJsonFile();

builder.Services.AddHttpContextAccessor();

var fsql = builder.AddFreesql("identity", FreeSql.DataType.PostgreSQL, cmd =>
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
});

builder.Services.AddHttpClient();

builder.Services.AddRadzenComponents();

var app = builder.Build();

//await FreesqlExtension.DbInitialize(fsql);

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
