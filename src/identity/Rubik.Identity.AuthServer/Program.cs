using Rubik.Share.Entity.FreesqlExtension;
using Rubik.Share.Extension;
using Rubik.Identity.Oidc.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Oauth2.0 �ͻ����Լ���cookie��֤
builder.AddOidcServer()
    .AddOidcConfig();

builder.AddEnvironmentJsonFile();

builder.Services.AddHttpContextAccessor();

var fsql = builder.AddFreesql("identity", FreeSql.DataType.PostgreSQL, cmd =>
{
#if DEBUG
    System.Diagnostics.Debug.WriteLine(cmd.CommandText);
#endif
    // ���� aop
});


var app = builder.Build();

app.UseOidcServer();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.Run();
