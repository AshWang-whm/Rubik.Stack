using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Configuration;

namespace Rubik.Infrastructure.Logger.Middleware.Api
{
    public static class SerilogExtension
    {
        /// <summary>
        /// Logger Sink
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <param name="level"></param>
        public static void AddSerilog(this WebApplicationBuilder builder, Func<LoggerSinkConfiguration,LoggerConfiguration> loggerConfiguration)
        {
            builder.Host.UseSerilog();
            Log.Logger =loggerConfiguration.Invoke(new LoggerConfiguration()
                .WriteTo)
                .CreateLogger();
        }


    }
}
