using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        }
    }
}
