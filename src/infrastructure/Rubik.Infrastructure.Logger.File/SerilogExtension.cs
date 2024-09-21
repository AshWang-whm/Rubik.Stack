using Serilog;
using Serilog.Configuration;

namespace Rubik.Infrastructure.Logger
{
    public static class SerilogExtension
    {
        /// <summary>
        /// File Sink Logger
        /// </summary>
        /// <param name="sinkConfiuration"></param>
        /// <param name="level"></param>
        public static LoggerConfiguration AddFileSerilog(this LoggerSinkConfiguration sinkConfiuration, Serilog.Events.LogEventLevel level= Serilog.Events.LogEventLevel.Information)
        {
            return sinkConfiuration.File("logs/log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: level);
        }
    }
}
