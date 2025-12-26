using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OLT.Logging.Serilog.Tests.NgxLogger
{

    public class NgxLoggerDetailMessageTests
    {
        [Fact]
        public void ToExceptionTests()
        {
            var model = new OltNgxLoggerDetailJson();
            Assert.Null(model.Stack);
            var exception = model.ToException();
            Assert.Null(exception.Source);
            Assert.NotEmpty(exception.Data);
            Assert.Null(exception.Data["Name"]);
            Assert.Null(exception.Data["AppId"]);
            Assert.Null(exception.Data["User"]);
            Assert.Null(exception.Data["Url"]);
            Assert.Null(exception.Data["Status"]);
            Assert.Null(exception.Data["Time"]);
            Assert.False(exception.Data.Contains("Stack"));

            model.Stack = new List<OltNgxLoggerStackJson>();
            exception = model.ToException();
            Assert.False(exception.Data.Contains("Stack"));           
            
            model.Stack.Add(NgxTestHelper.FakerStackJson(5));
            model.Stack.Add(NgxTestHelper.FakerStackJson(15));
            model.Stack.Add(NgxTestHelper.FakerStackJson(25));
            

            var stack = model.Stack.Select(s => $"{OltNgxLoggerStackExtensions.FormatStack(s)}{Environment.NewLine}").ToList();
            var expected = string.Join($"----------------------------{Environment.NewLine}", stack);

            exception = model.ToException();
            Assert.Equal(expected, exception.Data["Stack"]); 
        }

        [Theory]
        [MemberData(nameof(NgxLoggerDetailData))]
        public void MessageTest(HelperNgxExceptionTest data)
        {
            var exception = data.Detail.ToException();
            Assert.Equal(data.Detail.Message, exception.Message);
            Assert.Equal(data.Detail.Id, exception.Source);
            var dict = TestHelper.ToDictionary(exception.Data);

            Assert.Collection(dict,
                item => Assert.Equal(data.Result["Name"], item.Value),
                item => Assert.Equal(data.Result["AppId"], item.Value),
                item => Assert.Equal(data.Result["User"], item.Value),
                item => Assert.Equal(data.Result["Time"], item.Value),
                item => Assert.Equal(data.Result["Url"], item.Value),
                item => Assert.Equal(data.Result["Status"], item.Value),
                item => Assert.Equal(data.Result["Stack"], item.Value)
            );

        }

        public static IEnumerable<object[]> NgxLoggerDetailData =>
            new List<object[]>
            {
                new object[] { new HelperNgxExceptionTest(DateTimeOffset.Now) },
                new object[] { new HelperNgxExceptionTest(null) },
            };
    }
}
