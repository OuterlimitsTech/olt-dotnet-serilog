using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OLT.Core;
using OLT.Logging.Serilog;
using Serilog.Sinks.InMemory;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace OLT.AspNetCore.Serilog.Tests.ThrowError
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

            var request = testServer.CreateRequest("/api/throw-error");
            request.AddHeader("header-one", "value-one");
            var response = await request.GetAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();

            var logs = InMemorySink.Instance.LogEvents.ToList();
            logs.Should().HaveCount(2);

            var json = JsonConvert.DeserializeObject<OltErrorHttpSerilog>(body);
            var serverError = logs.First(p => p.MessageTemplate.Text == OltSerilogConstants.Templates.AspNetCore.ServerError);
            var payload = logs.First(p => p.MessageTemplate.Text == OltSerilogConstants.Templates.AspNetCore.Payload);

            Assert.NotNull(json);
            Assert.NotNull(serverError);
            Assert.NotNull(payload);

            TestHelper.ValidatePayloadProperties(payload.Properties);
            TestHelper.TestIdentityProperties(payload.Properties, identity);
            TestHelper.TestIdentityProperties(serverError.Properties, identity);
            TestHelper.ValidateAppRequestUid(json, serverError);
            TestHelper.ValidateAppRequestUid(json, payload);

            TestHelper.CleanValue(payload.Properties[OltSerilogConstants.Properties.AspNetCore.ResponseBody]).Should().Contain(json.ErrorUid.ToString());
            TestHelper.CleanValue(payload.Properties[OltSerilogConstants.Properties.AspNetCore.RequestHeaders]).Should().Contain("header-one");

        }
    }
}