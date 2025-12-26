using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLT.Logging.Serilog.Tests.NgxLogger
{
    public static  class NgxTestHelper
    {

        public static OltNgxLoggerStackJson FakerStackJson(int seedSoureLines)
        {
            return new OltNgxLoggerStackJson
            {
                ColumnNumber = Faker.RandomNumber.Next(10, 10),
                LineNumber = Faker.RandomNumber.Next(500, 1000),
                FileName = Faker.Name.First(),
                FunctionName = Faker.Name.Last(),
                Source = Faker.Lorem.Words(seedSoureLines).LastOrDefault(),
            };
        }


    }
}
