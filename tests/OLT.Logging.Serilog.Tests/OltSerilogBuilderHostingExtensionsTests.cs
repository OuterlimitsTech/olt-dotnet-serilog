using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OLT.Core;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace OLT.Logging.Serilog.Tests;

public class OltSerilogBuilderHostingExtensionsTests
{
    [Fact]
    public void ConfigureLogging_ConfiguresSerilogCorrectly()
    {
        // Arrange
        var builder = new TestApplicationBuilder(Host.CreateApplicationBuilder());

        OltSerilogBuilderHostingExtensions.ConfigureLogging(builder);
        OltSerilogBuilderHostingExtensions.AddSerilog(builder);


        // Act
        var serviceProvider = builder.Services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        var serilog = Log.Logger;

        // Assert
        Assert.NotNull(Log.Logger);
        Assert.NotNull(logger);
        logger.Should().BeSameAs(Log.Logger);
    }

    [Fact]
    public void BuildSerilogConfig_ConfiguresLoggerConfigurationCorrectly()
    {
        // Arrange
        var builder = new TestApplicationBuilder(Host.CreateApplicationBuilder());
        OltSerilogBuilderHostingExtensions.ConfigureLogging(builder);
        OltSerilogBuilderHostingExtensions.AddSerilog(builder);

        var serviceProvider = builder.Services.BuildServiceProvider();

        var test = serviceProvider.GetService<ILogger>();

        // Assert
        Assert.NotNull(builder.LoggerConfiguration);
    }


    public class TestApplicationBuilder : OltApplicationHostBuilder<HostApplicationBuilder>, IOltSerilogHostApplicationBuilder
    {
        public TestApplicationBuilder([NotNull] HostApplicationBuilder builder) : base(builder)
        {
            LoggerConfiguration = new LoggerConfiguration();
        }

        public LoggerConfiguration LoggerConfiguration { get; }

        public override void AddConfiguration()
        {

        }

        public override void AddLoggingConfiguration()
        {
            base.Logging.AddSerilog();
        }

        public override void AddServices()
        {
            //this.AddOltIdentity<IOltIdentity, TestIdentity>();
        }
    }


}


