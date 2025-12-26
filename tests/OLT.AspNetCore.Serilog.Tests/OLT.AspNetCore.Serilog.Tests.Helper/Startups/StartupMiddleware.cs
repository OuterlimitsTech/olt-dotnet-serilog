using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OLT.Core;
using OLT.Logging.Serilog;
using System.Diagnostics.CodeAnalysis;

namespace OLT.AspNetCore.Serilog.Tests
{
    [ExcludeFromCodeCoverage]
    public class StartupMiddleware : BaseStartup
    {
        public StartupMiddleware(IConfiguration configuration) : base(configuration) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services
                .AddScoped<IOltIdentity, TestIdentity>()
                .AddScoped<IOltDbAuditUser>(x => x.GetRequiredService<IOltIdentity>());

            services.AddRouting();
            services.AddControllers();

            services.AddOltSerilog(opt =>
            {
                opt.ErrorMessage = $"{nameof(StartupMiddleware)} Error Message";
                opt.ShowExceptionDetails = true;
            });

        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);

            //app.UseAuthentication();
            app.UseOltSerilogRequestLogging();
            app.UseRouting();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
