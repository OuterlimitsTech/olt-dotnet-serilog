using Microsoft.Extensions.Configuration;
using Serilog;

namespace OLT.Core
{
    public static class OltSerilogBuilderHostingExtensions
    {
        public static TBuilder ConfigureLogging<TBuilder>(this TBuilder builder) where TBuilder : IOltSerilogHostApplicationBuilder
        {
            BuildSerilogConfig(builder.LoggerConfiguration, builder.Configuration);
            Log.Logger = builder.LoggerConfiguration.CreateLogger();
            return builder.AddSerilog();
        }


        public static TBuilder AddSerilog<TBuilder>(this TBuilder builder) where TBuilder : IOltSerilogHostApplicationBuilder
        {
            builder.Services.AddSerilog(Log.Logger);
            return builder;
        }

        public static LoggerConfiguration BuildSerilogConfig(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            return loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithOltEventType()
                ;
        }
    }
}
