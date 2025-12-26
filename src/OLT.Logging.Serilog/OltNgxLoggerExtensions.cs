
using Microsoft.Extensions.Logging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLT.Logging.Serilog
{
    public static class OltNgxLoggerExtensions
    {
        /// <summary>
        /// Converts ngx-logger <see href="https://www.npmjs.com/package/ngx-logger"/> log level to <see cref="LogEventLevel"/>
        /// </summary>
        /// <param name="ngxLoggerLevel"></param>
        /// <returns></returns>
        public static LogEventLevel ToSerilogLogLevel(this OltNgxLoggerLevel ngxLoggerLevel)
        {
            switch (ngxLoggerLevel)
            {
                case OltNgxLoggerLevel.Trace:
                    return LogEventLevel.Verbose;
                case OltNgxLoggerLevel.Debug:
                    return LogEventLevel.Debug;
                case OltNgxLoggerLevel.Information:
                    return LogEventLevel.Information;
                case OltNgxLoggerLevel.Log:
                    return LogEventLevel.Information;
                case OltNgxLoggerLevel.Warning:
                    return LogEventLevel.Warning;
                case OltNgxLoggerLevel.Error:
                    return LogEventLevel.Error;
                case OltNgxLoggerLevel.Fatal:
                    return LogEventLevel.Fatal;
                case OltNgxLoggerLevel.Off:
                    return LogEventLevel.Information;
                default:
                    return LogEventLevel.Information;
            }
        }

        /// <summary>
        /// Converts ngx-logger <see href="https://www.npmjs.com/package/ngx-logger"/> log level to <see cref="LogLevel"/>
        /// </summary>
        /// <param name="ngxLoggerLevel"></param>
        /// <returns></returns>
        public static LogLevel ToMicrosoftLogLevel(this OltNgxLoggerLevel ngxLoggerLevel)
        {
            switch (ngxLoggerLevel)
            {
                case OltNgxLoggerLevel.Trace:
                    return LogLevel.Trace;
                case OltNgxLoggerLevel.Debug:
                    return LogLevel.Debug;
                case OltNgxLoggerLevel.Information:
                    return LogLevel.Information;
                case OltNgxLoggerLevel.Log:
                    return LogLevel.Information;
                case OltNgxLoggerLevel.Warning:
                    return LogLevel.Warning;
                case OltNgxLoggerLevel.Error:
                    return LogLevel.Error;
                case OltNgxLoggerLevel.Fatal:
                    return LogLevel.Critical;
                case OltNgxLoggerLevel.Off:
                    return LogLevel.Information;
                default:
                    return LogLevel.Information;
            }
        }

    }
}
