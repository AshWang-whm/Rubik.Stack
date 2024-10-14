using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// api 项目通过Authority配置，从DiscoveryEndpoint获取配置，包括token验证公钥，api端自行验证token， 权限，scope等
// 前后端分离系统，前端自行处理refresh token 1：定时刷新token 2：401时再刷新token
// 后端api只接入server做验证

builder.Services
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Authority = "http://localhost:5000";
        o.ClaimsIssuer = "rubik.oidc";
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                // 从Authorize ， Cookies ， Url 3个其中之一获取access token
                ctx.Token = ctx.HttpContext.Request.Cookies["access_token"];
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                // 验证 scope ， 类似 IdentityServer4 的 AddIdentityServerAuthencation 的 ApiName 
                if ((ctx.Principal?.Identity?.IsAuthenticated ?? false))
                {
                    var scope = ctx.Principal.Claims.FirstOrDefault(a => a.Type == "scope")?.Value;
                    if (!(scope?.Contains("scope1") ?? false))
                        ctx.Fail(new Exception("scope is invalid"));
                }
                else
                    ctx.Success();
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.RequireAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
