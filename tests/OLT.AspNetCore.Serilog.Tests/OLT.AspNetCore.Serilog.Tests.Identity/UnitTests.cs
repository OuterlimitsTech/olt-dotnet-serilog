using AwesomeAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OLT.Constants;
using OLT.Core;
using OLT.Logging.Serilog;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace OLT.AspNetCore.Serilog.Tests.Identity
{
    public class UnitTests
    {
        [Fact]
        public async Task MiddlewareTest()
        {
            using (var testServer = new TestServer(TestHelper.WebHostBuilder<StartupMiddleware>()))
            {
                using (var logger = new LoggerConfiguration().WriteTo.Sink(new TestCorrelatorSink()).Enrich.FromLogContext().CreateLogger())
                {
                    Log.Logger = logger;
                    var identity = testServer.Services.GetRequiredService<IOltIdentity>();

                    var response = await testServer.CreateRequest("/api").SendAsync("GET");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    var logs = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                    TestHelper.TestIdentityProperties(logs.First().Properties, identity);
                }
            }
        }
    }
}