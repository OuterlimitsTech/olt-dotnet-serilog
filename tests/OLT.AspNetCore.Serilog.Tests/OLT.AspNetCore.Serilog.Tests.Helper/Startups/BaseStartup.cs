using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using System.Diagnostics.CodeAnalysis;

namespace OLT.AspNetCore.Serilog.Tests
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseStartup
    {
        protected BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected IConfiguration Configuration { get; }

        public virtual void Configure(IApplicationBuilder app)
        {

        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var logger = new LoggerConfiguration()                
                .MinimumLevel.Information()        
                .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
                .MinimumLevel.Override("System", LogEventLevel.Fatal)
                .Enrich.FromLogContext()
                .WriteTo.InMemory()
                .CreateLogger();
            Log.Logger = logger;
            services.AddSerilog(logger);
        }
    }
}
