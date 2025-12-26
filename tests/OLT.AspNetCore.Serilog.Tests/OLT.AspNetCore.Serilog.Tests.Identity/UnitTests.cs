using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OLT.Core;
using Serilog.Sinks.InMemory;
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
            using var host = new HostBuilder()
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseTestServer()
                        .UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                        .UseStartup<StartupMiddleware>();
                })
            .Build();

            await host.StartAsync();

            using var testServer = host.GetTestServer();

            var identity = testServer.Services.GetRequiredService<IOltIdentity>();

            var response = await testServer.CreateRequest("/api").SendAsync("GET");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var logs = InMemorySink.Instance.LogEvents.ToList();
            TestHelper.TestIdentityProperties(logs.First().Properties, identity);
        }
    }
}