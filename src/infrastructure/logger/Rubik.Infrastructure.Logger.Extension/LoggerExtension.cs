using Serilog;
using Serilog.Events;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Rubik.Infrastructure.Logger.Extension
{
    public static partial class LoggerExtension
    {
        static readonly JsonSerializerOptions Options = new()
        {
            Encoder= JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static void LogInformationObj<T>(this ILogger logger,T obj, [CallerMemberName] string caller="")
            where T : class
        {
            logger.Internal(LogEventLevel.Information, obj, caller);
        }
        public static void LogTraceObj<T>(this ILogger logger, T obj, [CallerMemberName] string caller = "")
            where T : class
        {
            logger.Internal(LogEventLevel.Verbose, obj, caller);
        }
        public static void LogWarningObj<T>(this ILogger logger, T obj, [CallerMemberName] string caller = "")
            where T : class
        {
            logger.Internal(LogEventLevel.Warning, obj, caller);
        }
        public static void LogCriticalObj<T>(this ILogger logger, T obj, [CallerMemberName] string caller = "")
            where T : class
        {
            logger.Internal(LogEventLevel.Fatal, obj, caller);
        }
        public static void LogErrorObj<T>(this ILogger logger, T obj, [CallerMemberName] string caller = "")
            where T : class
        {
            logger.Internal(LogEventLevel.Error, obj, caller);
        }

        public static void LogErrorException(this ILogger logger, Exception ex, [CallerMemberName] string caller = "")
        {
            logger.Internal(LogEventLevel.Error, ex, caller);
        }

        public static void LogDebugObj<T>(this ILogger logger, T obj, [CallerMemberName] string caller = "")
            where T : class
        {
            logger.Internal(LogEventLevel.Debug, obj, caller);
        }

        static void Internal<T>(this ILogger logger, LogEventLevel level, T obj,string caller)
            where T : class
        {
            if (obj is Exception ex)
            {
                logger.Write(LogEventLevel.Error,ex, $"Caller{caller}:\r\nMessage:{ex.Message}\r\nStace:{ex.StackTrace}");
            }
            else
            {
                if(obj is string str)
                    logger.Write(level, $"Caller:{caller}\r\n{str}");
                else
                {
                    var msg = JsonSerializer.Serialize(obj, Options);
                    logger.Write(level, $"Caller:{caller}\r\n{msg}");
                }
            }
        }
    }
}
