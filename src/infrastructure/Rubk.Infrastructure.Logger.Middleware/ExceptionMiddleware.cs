using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Rubik.Infrastructure.Contract.HttpResponse;
using Serilog;
using System.Text;
using System.Text.Json;
using Rubik.Infrastructure.Logger.Extension;

namespace Avd.Infrastructure.Logger
{
    public class GlobalExceptionMiddlware(RequestDelegate _next, ILogger logger, IHttpContextAccessor httpContextAccessor)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var address = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!;
                var ip = address.IsIPv4MappedToIPv6 ? address.ToString()[7..] : address.ToString();

                var path = httpContextAccessor.HttpContext!.Request.Path.Value ?? "";
                string? parameter = null;

                // restful api 去掉后面的参数
                if (httpContextAccessor.HttpContext!.Request.Method == "GET"&& httpContextAccessor.HttpContext!.Request.RouteValues.Values.Count > 0)
                {
                    foreach (var value in httpContextAccessor.HttpContext!.Request.RouteValues.Values)
                    {
                        if (value != null)
                            path = path.Replace(value.ToString()??"", null);
                    }

                    // 多个restful 参数的处理
                    if (httpContextAccessor.HttpContext!.Request.RouteValues.Values.Count > 1)
                    {
                        path= path.TrimEnd('/');
                        //path+= '/';
                    }
                }
                else
                {
                    if (context.Request.Headers.TryGetValue("Accept", out var header))
                    {
                        // post 接口只保存json参数
                        if (header == "application/json")
                        {
                            context.Request.EnableBuffering();
                            using var ms = new MemoryStream();
                            await context.Request.BodyReader.CopyToAsync(ms);
                            parameter = Encoding.UTF8.GetString(ms.ToArray());
                            // 超长不保存
                            if (parameter.Length > 1024)
                                parameter = "超长Body不记录";
                            context.Request.Body.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
                
                var user = httpContextAccessor.HttpContext!.User.Identity?.Name ?? null;
                var referer = httpContextAccessor.HttpContext!.Request.Headers.Referer.ToString();
                await _next(context);

            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 200;
                var result = HttpOutput.NotOk(ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                logger.LogErrorException(ex);
            }
        }
    }

    public static class GlobalExceptionMiddlewareExtension
    {
        public static void UseGlobalException(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddlware>();
        }
    }
}
