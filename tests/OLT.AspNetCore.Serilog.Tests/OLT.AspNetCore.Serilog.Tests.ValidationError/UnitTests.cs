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

namespace OLT.AspNetCore.Serilog.Tests.ValidationError
{
    public class UnitTests
    {
        [Fact]
        public async Task MiddlewareTests()
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

            var request = testServer.CreateRequest("/api/validation-error");
            var response = await request.GetAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();

            var logs = InMemorySink.Instance.LogEvents.ToList();
            logs.Should().HaveCount(1);

            var json = JsonConvert.DeserializeObject<OltErrorHttpSerilog>(body);
            var payload = logs.First(p => p.MessageTemplate.Text == OltSerilogConstants.Templates.AspNetCore.Payload);


            Assert.NotNull(json);
            Assert.NotNull(payload);
            Assert.Equal("Please correct the validation errors", json.Message);

            TestHelper.ValidatePayloadProperties(payload.Properties);
            TestHelper.TestIdentityProperties(payload.Properties, identity);
            TestHelper.ValidateAppRequestUid(json, payload);

            TestHelper.CleanValue(payload.Properties[OltSerilogConstants.Properties.AspNetCore.ResponseBody]).Should().Contain(json.ErrorUid.ToString());           
           
        }
    }
}