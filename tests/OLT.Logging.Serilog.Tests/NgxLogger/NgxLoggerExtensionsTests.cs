using Microsoft.Extensions.Logging;
using Serilog.Events;
using Xunit;

namespace OLT.Logging.Serilog.Tests.NgxLogger
{
    public class NgxLoggerExtensionsTests
    {
        [Theory]
        [InlineData(OltNgxLoggerLevel.Trace, LogEventLevel.Verbose)]
        [InlineData(OltNgxLoggerLevel.Debug, LogEventLevel.Debug)]
        [InlineData(OltNgxLoggerLevel.Information, LogEventLevel.Information)]
        [InlineData(OltNgxLoggerLevel.Log, LogEventLevel.Information)]
        [InlineData(OltNgxLoggerLevel.Warning, LogEventLevel.Warning)]
        [InlineData(OltNgxLoggerLevel.Error, LogEventLevel.Error)]
        [InlineData(OltNgxLoggerLevel.Fatal, LogEventLevel.Fatal)]
        [InlineData(OltNgxLoggerLevel.Off, LogEventLevel.Information)]
        public void ToLogLevelTest(OltNgxLoggerLevel value, LogEventLevel expected)
        {
            Assert.Equal(expected, OltNgxLoggerExtensions.ToSerilogLogLevel(value));
        }

        [Fact]
        public void TestDefault()
        {
            var invalidValue = (OltNgxLoggerLevel)1000;
            Assert.Equal(LogEventLevel.Information, OltNgxLoggerExtensions.ToSerilogLogLevel(invalidValue));
        }


        [Theory]
        [InlineData(OltNgxLoggerLevel.Trace, LogLevel.Trace)]
        [InlineData(OltNgxLoggerLevel.Debug, LogLevel.Debug)]
        [InlineData(OltNgxLoggerLevel.Information, LogLevel.Information)]
        [InlineData(OltNgxLoggerLevel.Log, LogLevel.Information)]
        [InlineData(OltNgxLoggerLevel.Warning, LogLevel.Warning)]
        [InlineData(OltNgxLoggerLevel.Error, LogLevel.Error)]
        [InlineData(OltNgxLoggerLevel.Fatal, LogLevel.Critical)]
        [InlineData(OltNgxLoggerLevel.Off, LogLevel.Information)]
        public void ToMsLogLevelTest(OltNgxLoggerLevel value, LogLevel expected)
        {
            Assert.Equal(expected, OltNgxLoggerExtensions.ToMicrosoftLogLevel(value));
        }

        [Fact]
        public void TestMsDefault()
        {
            var invalidValue = (OltNgxLoggerLevel)1000;
            Assert.Equal(LogLevel.Information, OltNgxLoggerExtensions.ToMicrosoftLogLevel(invalidValue));
        }
    }
}
