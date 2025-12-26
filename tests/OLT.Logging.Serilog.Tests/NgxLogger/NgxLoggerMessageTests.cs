using AwesomeAssertions;
using OLT.Constants;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OLT.Core;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace OLT.Logging.Serilog.Tests.NgxLogger
{

    public class NgxLoggerMessageTests
    {
        private readonly ITestOutputHelper _output;

        public NgxLoggerMessageTests(ITestOutputHelper output)
        {
            _output = output;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger();
        }


        [Theory]
        [MemberData(nameof(NgxLoggerMessageData))]
        public void MessageTest(OltNgxLoggerLevel? level, bool loadDetail, bool expectedError, HelperNgxExceptionTest data)
        {
            var msg = data.BuildMessage(level, loadDetail ? data.Detail : null);

            if (!loadDetail && level == OltNgxLoggerLevel.Fatal)
            {
                msg.Additional.Add(new List<OltNgxLoggerDetailJson>());
            }

            var exception = msg.ToException();
            var exceptionMessage = msg.Additional.FirstOrDefault()?.FirstOrDefault()?.Message ?? msg.Message;
            Assert.Equal(exceptionMessage, exception.Message);

            var formattedLogMsg = msg.FormatMessage();
            Assert.Equal(exception.Message, formattedLogMsg);

            Assert.Equal(expectedError, msg.IsError);

            var dict = TestHelper.ToDictionary(exception.Data);
            if (loadDetail)
            {
                Assert.Collection(dict,
                    item => Assert.Equal(data.Result["Name"], item.Value),
                    item => Assert.Equal(data.Result["AppId"], item.Value),
                    item => Assert.Equal(data.Result["User"], item.Value),
                    item => Assert.Equal(data.Result["Time"], item.Value),
                    item => Assert.Equal(data.Result["Url"], item.Value),
                    item => Assert.Equal(data.Result["Status"], item.Value),
                    item => Assert.Equal(data.Result["Stack"], item.Value),
                    item => Assert.Equal(data.Result["Username"], item.Value),
                    item => Assert.Equal(data.Result["Level"], item.Value),
                    item => Assert.Equal(data.Result["FileName"], item.Value),
                    item => Assert.Equal(data.Result["LineNumber"], item.Value),
                    item => Assert.Equal(data.Result["ColumnNumber"], item.Value),                    
                    item => Assert.Equal(data.Result["Timestamp"], item.Value)
                );

                return;
            }


            Assert.Collection(dict,
                item => Assert.Equal(data.Result["Username"], item.Value),
                item => Assert.Equal(data.Result["Level"], item.Value),
                item => Assert.Equal(data.Result["FileName"], item.Value),
                item => Assert.Equal(data.Result["LineNumber"], item.Value),
                item => Assert.Equal(data.Result["ColumnNumber"], item.Value),                
                item => Assert.Equal(data.Result["Timestamp"], item.Value)
            );

        }


        public static IEnumerable<object[]> NgxLoggerMessageData =>
         new List<object[]>
         {
                new object[] { OltNgxLoggerLevel.Error, true, true, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { OltNgxLoggerLevel.Fatal, true, true, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { OltNgxLoggerLevel.Information, true, false, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { null!, true, false, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { OltNgxLoggerLevel.Trace, true, false, new HelperNgxExceptionTest(null) },
                new object[] { OltNgxLoggerLevel.Warning, true, false, new HelperNgxExceptionTest(null) },

                new object[] { OltNgxLoggerLevel.Error, false, true, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { OltNgxLoggerLevel.Fatal, false, true, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { OltNgxLoggerLevel.Warning, false, false, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { null!, false, false, new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { null!, false, false, new HelperNgxExceptionTest(null) },
                new object[] { OltNgxLoggerLevel.Debug, false, false, new HelperNgxExceptionTest(null) },
                new object[] { OltNgxLoggerLevel.Log, false, false, new HelperNgxExceptionTest(null) },

                new object[] { OltNgxLoggerLevel.Error, false, true, new HelperNgxExceptionTest(DateTimeOffset.Now, true) },
                new object[] { OltNgxLoggerLevel.Error, false, true, new HelperNgxExceptionTest(null, true) },

         };


        private async Task<OltNgxLoggerMessageJson> FromJsonFile()
        {
            string fileName = "ngx-sample.json";
            var filePath = Path.Combine(AppContext.BaseDirectory, "NgxLogger", fileName);

            OltNgxLoggerMessageJson? result;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            using (FileStream openStream = File.OpenRead(filePath))
            {
                result = await JsonSerializer.DeserializeAsync<OltNgxLoggerMessageJson>(openStream, options);
            }

            return result ?? new OltNgxLoggerMessageJson();
        }


        [Theory]
        [InlineData(OltNgxLoggerLevel.Trace, LogEventLevel.Verbose)]
        [InlineData(OltNgxLoggerLevel.Debug, LogEventLevel.Debug)]
        [InlineData(OltNgxLoggerLevel.Information, LogEventLevel.Information)]
        [InlineData(OltNgxLoggerLevel.Log, LogEventLevel.Information)]
        [InlineData(OltNgxLoggerLevel.Warning, LogEventLevel.Warning)]
        [InlineData(OltNgxLoggerLevel.Error, LogEventLevel.Error)]
        [InlineData(OltNgxLoggerLevel.Fatal, LogEventLevel.Fatal)]
        [InlineData(OltNgxLoggerLevel.Off, LogEventLevel.Information)]
        [InlineData(null, LogEventLevel.Information)]
        public async Task ForContextTests(OltNgxLoggerLevel? level, LogEventLevel expected)
        {
            var model = await FromJsonFile();

            model.Level = level;

            using (TestCorrelator.CreateContext())
            using (var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(_output)
                .WriteTo.Sink(new TestCorrelatorSink())
                .CreateLogger())
            {
                logger.Write(model);

                TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.MessageTemplate.Text.Should().Be(OltSerilogConstants.Templates.NgxMessage.Template);
                TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.Level.Should().Be(expected);
                var props = TestCorrelator.GetLogEventsFromCurrentContext().First().Properties;
                props.Count.Should().Be(1);
                props.Should().ContainKey(OltSerilogConstants.Properties.NgxMessage.MessageAsJson);                

                if (model.IsError)
                {
                    TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.Exception.Should().NotBeNull();
                }
                else
                {
                    TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.Exception.Should().BeNull();
                }
            }


            // Test MS Logging Now
            var services = new ServiceCollection().AddLogging(config => config.AddConsole());

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                using (var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>())
                {
                    var msLogger = loggerFactory.CreateLogger<NgxLoggerMessageTests>();
                    msLogger.Write(model);
                    msLogger.Should().NotBeNull();  //I can only validate that it did not throw an error
                }
            }

        }
    }

}
