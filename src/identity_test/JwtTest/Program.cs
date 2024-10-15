using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// api ��Ŀͨ��Authority���ã���DiscoveryEndpoint��ȡ���ã�����token��֤��Կ��api��������֤token�� Ȩ�ޣ�scope��
// ǰ��˷���ϵͳ��ǰ�����д���refresh token 1����ʱˢ��token 2��401ʱ��ˢ��token
// ���apiֻ����server����֤

builder.Services.AddHttpClient("OidcServer", opt =>
{
    opt.BaseAddress = new Uri("http://localhost:5000");
});

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
                // ��Authorize �� Cookies �� Url 3������֮һ��ȡaccess token
                ctx.Token = ctx.HttpContext.Request.Cookies["access_token"];
                return Task.CompletedTask;
            },
            OnTokenValidated =async ctx =>
            {
                // ����Զ�� VerifyReferenceToken , /oauth/verify
                var client = ctx.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>()
                    .CreateClient("OidcServer");
                if (client == null)
                {
                    ctx.Fail(new Exception("δ����Զ��OidcServer HttpClient!"));
                    return;
                }
                var a = ctx.SecurityToken.UnsafeToString();
                var b = ctx.SecurityToken.GetType();
                var reference = await client!.GetStringAsync($"/oauth/verify?token={ctx.SecurityToken.UnsafeToString()}");
                var result = JsonSerializer.Deserialize<VerifyToken>(reference);
                if (result?.result ?? false)
                    ctx.Success();
                else
                    ctx.Fail(new Exception(result?.exception??"��֤tokenʧ��!"));
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


internal class VerifyToken
{
    public bool result { get; set; }

    public string? exception { get; set; }
}