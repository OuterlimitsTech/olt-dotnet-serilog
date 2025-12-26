using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OLT.Logging.Serilog.Tests
{
    internal class Startup
    {

        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureHostConfiguration(builder =>
            {
                builder
                    .SetBasePath(AppContext.BaseDirectory)
                    //.AddJsonFile("appsettings.json", true)
                    .AddEnvironmentVariables()
                    .AddUserSecrets<Startup>();
            });
        }

        public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
        {

        }
    }
}
