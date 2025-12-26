using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OLT.Constants;
using OLT.Core;
using OLT.Logging.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using Seriolog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Seriolog
{
    /// <summary>
    /// I took this from the original deprecated source in 7.0
    /// </summary>
    public static class SerilogWebHostBuilderExtensions
    {
        public static IHostBuilder UseSerilog_local(this IHostBuilder builder, Serilog.ILogger? logger = null, bool dispose = false, LoggerProviderCollection? providers = null)
        {
            LoggerProviderCollection? providers2 = providers;
            Serilog.ILogger? logger2 = logger;
            ArgumentNullException.ThrowIfNull(builder);

            builder.ConfigureServices(delegate (IServiceCollection collection)
            {
                if (providers2 != null)
                {
                    collection.AddSingleton((Func<IServiceProvider, ILoggerFactory>)delegate (IServiceProvider services)
                    {
                        SerilogLoggerFactory serilogLoggerFactory = new SerilogLoggerFactory(logger2, dispose, providers2);
                        foreach (ILoggerProvider service in services.GetServices<ILoggerProvider>())
                        {
                            serilogLoggerFactory.AddProvider(service);
                        }

                        return serilogLoggerFactory;
                    });
                }
                else
                {
                    collection.AddSingleton((Func<IServiceProvider, ILoggerFactory>)((IServiceProvider _) => new SerilogLoggerFactory(logger2, dispose)));
                }

                ConfigureServices_local(collection, logger2);
            });
            return builder;
        }

        private static void ConfigureServices_local(IServiceCollection collection, Serilog.ILogger? logger)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (logger != null)
            {
                collection.AddSingleton(logger);
            }

            DiagnosticContext implementationInstance = new DiagnosticContext(logger);
            collection.AddSingleton(implementationInstance);
            collection.AddSingleton((IDiagnosticContext)implementationInstance);
        }
    }
}

namespace OLT.AspNetCore.Serilog.Tests
{
    [ExcludeFromCodeCoverage]
    public static class TestHelper
    {

        public static IHostBuilder WebHostBuilder<T>() where T : class
        {
            var webBuilder = new HostBuilder();

            webBuilder.UseSerilog_local();
            webBuilder.ConfigureWebHostDefaults(webHost =>
            {
                webHost.UseStartup<T>();
            });

            //webBuilder
            //    .UseSerilog_local()
            //    .ConfigureAppConfiguration(builder =>
            //    {
            //        builder
            //            .SetBasePath(AppContext.BaseDirectory)
            //            .AddUserSecrets<T>()
            //            .AddJsonFile("appsettings.json", true, false)
            //            .AddEnvironmentVariables();
            //    })                
            //    .UseStartup<T>();


            return webBuilder;

        }


        public static void ValidateAppRequestUid(OltErrorHttpSerilog json, LogEvent @event)
        {
            var appRequestUid = CleanValue(@event.Properties[OltSerilogConstants.Properties.AspNetCore.AppRequestUid]).ToGuid();
            Assert.Equal(json.ErrorUid, appRequestUid);
        }

        public static void ValidatePayloadProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties)
        {
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.AppRequestUid).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.RequestHeaders).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.ResponseHeaders).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.RequestBody).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.ResponseBody).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.RequestUri).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.Username).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.DbUsername).Should().BeTrue();
        }

        public static void TestIdentityProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, IOltIdentity identity)
        {
            properties.ContainsKey(OltSerilogConstants.Properties.Username).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.DbUsername).Should().BeTrue();
            Assert.Equal(identity.Username, CleanValue(properties[OltSerilogConstants.Properties.Username]));
            Assert.Equal(identity.Username, CleanValue(properties[OltSerilogConstants.Properties.DbUsername]));
        }

        public static string CleanValue(LogEventPropertyValue value)
        {
            return value.ToString().Replace("\"", string.Empty);
        }
    }
}
