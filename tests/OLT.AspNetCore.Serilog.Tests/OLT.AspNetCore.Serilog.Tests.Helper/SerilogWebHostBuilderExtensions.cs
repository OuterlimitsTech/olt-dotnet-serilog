//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Serilog;
//using Serilog.Extensions.Hosting;
//using Serilog.Extensions.Logging;
//using System;

//namespace Seriolog
//{
//    /// <summary>
//    /// I took this from the original deprecated source in 7.0
//    /// </summary>
//    public static class SerilogWebHostBuilderExtensions
//    {
//        public static IHostBuilder UseSerilog_local(this IHostBuilder builder, Serilog.ILogger? logger = null, bool dispose = false, LoggerProviderCollection? providers = null)
//        {
//            LoggerProviderCollection? providers2 = providers;
//            Serilog.ILogger? logger2 = logger;
//            ArgumentNullException.ThrowIfNull(builder);

//            builder.ConfigureServices(delegate (IServiceCollection collection)
//            {
//                if (providers2 != null)
//                {
//                    collection.AddSingleton((Func<IServiceProvider, ILoggerFactory>)delegate (IServiceProvider services)
//                    {
//                        SerilogLoggerFactory serilogLoggerFactory = new SerilogLoggerFactory(logger2, dispose, providers2);
//                        foreach (ILoggerProvider service in services.GetServices<ILoggerProvider>())
//                        {
//                            serilogLoggerFactory.AddProvider(service);
//                        }

//                        return serilogLoggerFactory;
//                    });
//                }
//                else
//                {
//                    collection.AddSingleton((Func<IServiceProvider, ILoggerFactory>)((IServiceProvider _) => new SerilogLoggerFactory(logger2, dispose)));
//                }

//                ConfigureServices_local(collection, logger2);
//            });
//            return builder;
//        }

//        private static void ConfigureServices_local(IServiceCollection collection, Serilog.ILogger? logger)
//        {
//            ArgumentNullException.ThrowIfNull(collection);

//            if (logger != null)
//            {
//                collection.AddSingleton(logger);
//            }

//            DiagnosticContext implementationInstance = new DiagnosticContext(logger);
//            collection.AddSingleton(implementationInstance);
//            collection.AddSingleton((IDiagnosticContext)implementationInstance);
//        }
//    }
//}
