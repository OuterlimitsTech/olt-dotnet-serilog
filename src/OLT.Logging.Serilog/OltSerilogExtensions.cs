using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OLT.Logging.Serilog;
using OLT.Logging.Serilog.Enricher;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace OLT.Core
{
    
    public static class OltSerilogExtensions
    {

        /// <summary>
        /// Enrich log unique identifier using <see cref="OltEventTypeEnricher"/>.
        /// </summary>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithOltEventType(this LoggerEnrichmentConfiguration loggerConfiguration)
        {
            return loggerConfiguration.With(new OltEventTypeEnricher());
        }


        /// <summary>
        /// Enrich log the Environment Name <see cref="OltSerilogConstants.Properties.Environment"/> and <see cref="OltSerilogConstants.Properties.DebuggerAttached"/>
        /// </summary>
        /// <returns>Configuration object allowing method chaining.</returns>        
        [Obsolete("Use to Serilog.Enrichers.Environment -> WithEnvironmentName")]
        public static LoggerConfiguration WithOltEnvironment(this LoggerEnrichmentConfiguration loggerConfiguration, string environmentName)
        {

            return loggerConfiguration
                .WithProperty(OltSerilogConstants.Properties.Environment, environmentName)

                // Used to filter out potentially bad data due debugging.
                // Very useful when doing Seq dashboards and want to remove logs under debugging session.
                .Enrich.WithProperty(OltSerilogConstants.Properties.DebuggerAttached, Debugger.IsAttached)
                ;
        }


    


        /// <summary>
        /// Writes <see cref="OltNgxLoggerMessageJson"/> to log
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="model"></param>
        public static void Write(this Serilog.ILogger logger, OltNgxLoggerMessageJson model)
        {
            var level = model.Level?.ToSerilogLogLevel() ?? LogEventLevel.Information;

            if (level == LogEventLevel.Error)
            {
                logger
                    .ForContext(OltSerilogConstants.Properties.NgxMessage.MessageAsJson, model, destructureObjects: true)
                    .Error(model.ToException(), OltSerilogConstants.Templates.NgxMessage.Template, model.FormatMessage());
                return;
            }

            if (level == LogEventLevel.Fatal)
            {
                logger
                    .ForContext(OltSerilogConstants.Properties.NgxMessage.MessageAsJson, model, destructureObjects: true)
                    .Fatal(model.ToException(), OltSerilogConstants.Templates.NgxMessage.Template, model.FormatMessage());
                return;
            }


            logger
                .ForContext(OltSerilogConstants.Properties.NgxMessage.MessageAsJson, model, destructureObjects: true)
                .Write(level, OltSerilogConstants.Templates.NgxMessage.Template, model.FormatMessage());            
        }

        /// <summary>
        /// Writes <see cref="OltNgxLoggerMessageJson"/> to log
        /// </summary>
        /// <param name="msLogger"></param>
        /// <param name="model"></param>
        public static void Write(this Microsoft.Extensions.Logging.ILogger msLogger, OltNgxLoggerMessageJson model)
        {
            var level = model.Level?.ToMicrosoftLogLevel() ?? LogLevel.Information;
            if (level == LogLevel.Error)
            {
                msLogger.LogError(model.ToException(), "ngx-message: {ngx-message}", model.FormatMessage());
                return;
            }

            if (level == LogLevel.Critical)
            {
                msLogger.LogCritical(model.ToException(), "ngx-message: {ngx-message}", model.FormatMessage());
                return;
            }

            msLogger.Log(level, "ngx-message: {ngx-message}", model.FormatMessage());
        }
    }
}
