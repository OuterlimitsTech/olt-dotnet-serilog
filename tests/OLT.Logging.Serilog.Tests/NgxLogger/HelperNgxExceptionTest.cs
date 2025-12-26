using Faker;
using OLT.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLT.Logging.Serilog.Tests.NgxLogger
{
    public class HelperNgxExceptionTest
    {
        public HelperNgxExceptionTest(DateTimeOffset? dt, bool nullStack = false)
        {
            if (dt.HasValue)
            {
                Timestamp = dt.Value;
                UnixTime = dt.Value.ToUnixTimeMilliseconds();
            }


            
            Stack = new List<OltNgxLoggerStackJson>
            {
                NgxTestHelper.FakerStackJson(6),
                NgxTestHelper.FakerStackJson(12),
            };

            var status = Faker.RandomNumber.Next(200, 600);

            Result = new Dictionary<string, string?>
            {
                { "Name", Faker.Name.FullName(NameFormats.WithSuffix) },
                { "AppId", Faker.Lorem.GetFirstWord() },
                { "User", Faker.Internet.UserName() },
                { "Time", UnixTime.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(UnixTime.Value).ToString(OltSerilogConstants.FormatString.ISO8601) : null },
                { "Url", Faker.Internet.Url() },
                { "Status", status.ToString() },
                { "Stack", Stack.FormatStack() }
            };

            Detail = new OltNgxLoggerDetailJson
            {
                Id = Faker.Lorem.GetFirstWord(),
                AppId = Result["AppId"],
                Message = Faker.Lorem.Sentence(),
                Name = Result["Name"],
                Status = status,
                Time = UnixTime,
                Url = Result["Url"],
                User = Result["User"],
                Stack = Stack
            };

            if (nullStack)
            {
                Detail.Stack = null;
            }
        }


        public long? UnixTime { get; }
        public DateTimeOffset? Timestamp { get; }
        public Dictionary<string, string?> Result { get; }
        public List<OltNgxLoggerStackJson> Stack { get; }
        public OltNgxLoggerDetailJson Detail { get; }


        public OltNgxLoggerMessageJson BuildMessage(OltNgxLoggerLevel? level, OltNgxLoggerDetailJson? detail)
        {
            var msg = new OltNgxLoggerMessageJson
            {
                Message = Faker.Lorem.Sentence(),
                Timestamp = Timestamp,
                FileName = Faker.Lorem.GetFirstWord(),
                LineNumber = Faker.RandomNumber.Next(1000, 4000),
                ColumnNumber = Faker.RandomNumber.Next(7000, 56000),
            };

            msg.Level = level ?? msg.Level;

            if (detail != null)
            {
                msg.Additional = new List<List<OltNgxLoggerDetailJson>>
                {
                    new List<OltNgxLoggerDetailJson>
                    {
                        detail
                    }
                };
            }

            Result.Add("Username", msg.GetUsername());
            Result.Add("Level", msg.Level?.ToString());
            Result.Add("FileName", msg.FileName);
            Result.Add("LineNumber", msg.LineNumber.ToString());
            Result.Add("ColumnNumber", msg.ColumnNumber.ToString());            
            Result.Add("Timestamp", msg.Timestamp?.ToString(OltSerilogConstants.FormatString.ISO8601));

            return msg;
        }

    }
}
