using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OLT.Logging.Serilog;
using Serilog.AspNetCore;
using System;
using Xunit;


namespace OLT.AspNetCore.Serilog.Tests.General
{
    public class UnitTestsExceptions
    {

        [Fact]
        public void ServiceArgumentExceptions()
        {
            var services = new ServiceCollection();
            Action<OltSerilogOptions> action = (OltSerilogOptions opts) =>
            {
                opts.ErrorMessage = "Test";
                opts.ShowExceptionDetails = true;
                opts.BodyPayloadLimit = 1.0;
            };

            Assert.Throws<ArgumentNullException>("services", () => OltSerilogAspNetCoreExtensions.AddOltSerilog(null!, null!));

            try
            {
                OltSerilogAspNetCoreExtensions.AddOltSerilog(services, null!);
                OltSerilogAspNetCoreExtensions.AddOltSerilog(services, action);
                Assert.True(true);
            }
            catch
            {
                Assert.False(true);
            }

        }

        [Fact]
        public void AppArgumentExceptions()
        {
            var services = new ServiceCollection();

            Action<RequestLoggingOptions> action = (RequestLoggingOptions opts) =>
            {
                opts.MessageTemplate = "Test";
            };

            var app = new ApplicationBuilder(services.BuildServiceProvider());
            Assert.Throws<ArgumentNullException>("app", () => OltSerilogAspNetCoreExtensions.UseOltSerilogRequestLogging(null!, null));


            try
            {
                OltSerilogAspNetCoreExtensions.UseOltSerilogRequestLogging(app, null);
                OltSerilogAspNetCoreExtensions.UseOltSerilogRequestLogging(app, action);
                Assert.True(true);
            }
            catch
            {
                Assert.False(true);
            }
        }

    }
}
