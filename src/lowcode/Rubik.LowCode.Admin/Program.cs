using Rubik.LowCode.Admin.Components;
using Rubik.Share.Entity.FreesqlExtension;
using Rubik.Share.Extension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBootstrapBlazor();


builder.AddEnvironmentJsonFile();

var fsql = builder.AddFreesql("lowcode_admin", FreeSql.DataType.PostgreSQL, cmd =>
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
    // ²¹³ä aop
});


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
