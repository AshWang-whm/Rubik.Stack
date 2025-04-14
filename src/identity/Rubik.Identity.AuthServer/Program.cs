using Microsoft.Extensions.FileProviders;
using Rubik.Identity.Oidc.Core.Extensions;
using Rubik.Infrastructure.Orm.Freesql;
using Rubik.Infrastructure.WebExtension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Oauth2.0 客户端自己用cookie验证
builder.AddOidcServer()
    .AddOidcConfig();

builder.AddEnvironmentJsonFile();

builder.Services.AddFreesqlOrm(builder.Configuration.GetConnectionString("identity")!);

// javascript test cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("javacript_test", cors =>
    {
        cors.AllowAnyOrigin();
        cors.AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseStaticFiles();

app.UseOidcServer();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseCors("javacript_test");

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.Run();
