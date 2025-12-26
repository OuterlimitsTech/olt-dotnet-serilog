using System;
using System.Text;
using Xunit;

namespace OLT.Logging.Serilog.Tests.NgxLogger
{
    public class NgxLoggerStackMessageTests
    {
        [Fact]
        public void ToStringTests()
        {
            var model = NgxTestHelper.FakerStackJson(7);
            var expected = new StringBuilder();

            expected.Append($"Column Number: {model.ColumnNumber}{Environment.NewLine}");
            expected.Append($"Line Number: {model.LineNumber}{Environment.NewLine}");
            expected.Append($"FileName: {model.FileName}{Environment.NewLine}");
            expected.Append($"FunctionName: {model.FunctionName}{Environment.NewLine}");
            expected.Append($"Source: {model.Source}");

            Assert.Equal(expected.ToString(), model.FormatStack());            

        }
    }
}
