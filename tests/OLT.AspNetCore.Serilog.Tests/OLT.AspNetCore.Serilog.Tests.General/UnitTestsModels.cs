using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OLT.Logging.Serilog;
using System.Collections.Generic;
using Xunit;


namespace OLT.AspNetCore.Serilog.Tests.General
{
    public class UnitTestsModels
    {
        [Fact]
        public void SerilogOptions()
        {
            var options = new OltSerilogOptions();
            Assert.False(options.ShowExceptionDetails);
            Assert.Equal(0.25, options.BodyPayloadLimit);
            Assert.Equal("An error has occurred.", options.ErrorMessage);
        }

        [Fact]
        public void SerializeErrorHttp()
        {
            var obj = new OltErrorHttpSerilog
            {
                Message = Faker.Lorem.GetFirstWord(),
                Errors = new List<string>
                {
                    Faker.Lorem.GetFirstWord(),
                    Faker.Lorem.GetFirstWord()
                }
            };

            var compareTo = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            Assert.Equal(obj.ToString(), compareTo);
        }


    }
}
