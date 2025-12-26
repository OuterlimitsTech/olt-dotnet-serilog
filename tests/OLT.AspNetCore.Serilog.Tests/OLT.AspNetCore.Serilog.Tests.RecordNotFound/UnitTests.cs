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

namespace OLT.AspNetCore.Serilog.Tests.RecordNotFound
{
    public class UnitTests
    {

        [Fact]
        public async Task MiddlewareTests()
        {

            using (var testServer = new TestServer(TestHelper.WebHostBuilder<StartupMiddleware>()))
            {
                using (var logger = new LoggerConfiguration().WriteTo.Sink(new TestCorrelatorSink()).Enrich.FromLogContext().CreateLogger())
                {
                    Log.Logger = logger;
                    var identity = testServer.Services.GetRequiredService<IOltIdentity>();

                    var request = testServer.CreateRequest("/api/record-not-found");
                    var response = await request.GetAsync();
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                    var body = await response.Content.ReadAsStringAsync();

                    var logs = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                    logs.Should().HaveCount(1);

                    var json = JsonConvert.DeserializeObject<OltErrorHttpSerilog>(body);
                    var payload = logs.First(p => p.MessageTemplate.Text == OltSerilogConstants.Templates.AspNetCore.Payload);


                    Assert.NotNull(json);
                    Assert.NotNull(payload);
                    Assert.Equal("record-not-found", json.Message);

                    TestHelper.ValidatePayloadProperties(payload.Properties);
                    TestHelper.TestIdentityProperties(payload.Properties, identity);
                    TestHelper.ValidateAppRequestUid(json, payload);

                    TestHelper.CleanValue(payload.Properties[OltSerilogConstants.Properties.AspNetCore.ResponseBody]).Should().Contain(json.ErrorUid.ToString());

                }
            }
        }

    }
}