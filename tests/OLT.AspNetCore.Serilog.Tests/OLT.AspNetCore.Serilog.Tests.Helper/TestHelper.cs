using AwesomeAssertions;
using OLT.Core;
using OLT.Logging.Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;


namespace OLT.AspNetCore.Serilog.Tests
{
    [ExcludeFromCodeCoverage]
    public static class TestHelper
    {

        //public static IHostBuilder WebHostBuilder<T>() where T : class
        //{
        //    var webBuilder = new HostBuilder();

        //    webBuilder.UseSerilog_local();
        //    webBuilder.ConfigureWebHostDefaults(webHost =>
        //    {
        //        webHost.UseStartup<T>();
        //    });

        //    //webBuilder
        //    //    .UseSerilog_local()
        //    //    .ConfigureAppConfiguration(builder =>
        //    //    {
        //    //        builder
        //    //            .SetBasePath(AppContext.BaseDirectory)
        //    //            .AddUserSecrets<T>()
        //    //            .AddJsonFile("appsettings.json", true, false)
        //    //            .AddEnvironmentVariables();
        //    //    })                
        //    //    .UseStartup<T>();


        //    return webBuilder;

        //}


        public static void ValidateAppRequestUid(OltErrorHttpSerilog json, LogEvent @event)
        {
            var appRequestUid = CleanValue(@event.Properties[OltSerilogConstants.Properties.AspNetCore.AppRequestUid]).ToGuid();
            Assert.Equal(json.ErrorUid, appRequestUid);
        }

        public static void ValidatePayloadProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties)
        {
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.AppRequestUid).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.RequestHeaders).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.ResponseHeaders).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.RequestBody).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.ResponseBody).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.AspNetCore.RequestUri).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.Username).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.DbUsername).Should().BeTrue();
        }

        public static void TestIdentityProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, IOltIdentity identity)
        {
            properties.ContainsKey(OltSerilogConstants.Properties.Username).Should().BeTrue();
            properties.ContainsKey(OltSerilogConstants.Properties.DbUsername).Should().BeTrue();
            Assert.Equal(identity.Username, CleanValue(properties[OltSerilogConstants.Properties.Username]));
            Assert.Equal(identity.Username, CleanValue(properties[OltSerilogConstants.Properties.DbUsername]));
        }

        public static string CleanValue(LogEventPropertyValue value)
        {
            return value.ToString().Replace("\"", string.Empty);
        }
    }
}
