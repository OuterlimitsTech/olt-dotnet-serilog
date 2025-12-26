using AwesomeAssertions;
using Serilog;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using OLT.Constants;
using OLT.Core;
using System.Data;
using System;
using Serilog.Events;

namespace OLT.Logging.Serilog.Tests
{
    public class SerilogMsSqlTests
    {
        [Fact]
        public void DefaultColumnOptionTests()
        {
            var result = OltSerilogMsSqlExtensions.DefaultColumnOptions;
            result.AdditionalColumns.Should().HaveCount(6);
            result.AdditionalColumns.Should().Satisfy(
                e => e.ColumnName == OltSerilogMsSqlConstants.ColumnNames.Application,
                e => e.ColumnName == OltSerilogMsSqlConstants.ColumnNames.RequestPath,
                e => e.ColumnName == OltSerilogMsSqlConstants.ColumnNames.Source,
                e => e.ColumnName == OltSerilogConstants.Properties.EventType,
                e => e.ColumnName == OltSerilogConstants.Properties.Username,
                e => e.ColumnName == OltSerilogConstants.Properties.DbUsername
                );

            Assert.Equal(OltSerilogMsSqlConstants.ColumnNames.Id, result.Id.ColumnName);
            Assert.Equal(SqlDbType.DateTimeOffset, result.TimeStamp.DataType);

        }

        [Fact]
        public void DefaultSinkOptionTests()
        {
            var result = OltSerilogMsSqlExtensions.DefaultSinkOptions;
            Assert.Equal(OltSerilogMsSqlConstants.Table.Name, result.TableName);
            Assert.Equal(OltSerilogMsSqlConstants.Table.Schema, result.SchemaName);            
        }



        [Fact]
        public void ExtensionTests()
        {
            
            var connectionString = "data source=localhost,1433;initial catalog=test;integrated security=False;user id=sa;password=nopass#4U";

            Assert.Throws<ArgumentNullException>("loggerConfiguration", () => OltSerilogMsSqlExtensions.WithOltMSSqlServer(null!, null, null, null, LogEventLevel.Debug));
            Assert.Throws<ArgumentNullException>("loggerConfiguration", () => OltSerilogMsSqlExtensions.WithOltMSSqlServer(null!, connectionString, null, null, LogEventLevel.Debug));
            Assert.Throws<ArgumentNullException>("connectionString", () => OltSerilogMsSqlExtensions.WithOltMSSqlServer(new LoggerConfiguration(), null, null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: true));
            Assert.Throws<ArgumentOutOfRangeException>("connectionString", () => OltSerilogMsSqlExtensions.WithOltMSSqlServer(new LoggerConfiguration(), " ", null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: true));
            Assert.Throws<ArgumentOutOfRangeException>("connectionString", () => OltSerilogMsSqlExtensions.WithOltMSSqlServer(new LoggerConfiguration(), "", null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: true));


            OltSerilogMsSqlExtensions
                .WithOltMSSqlServer(new LoggerConfiguration(), null, null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: false)
                .Should()
                .BeOfType<LoggerConfiguration>();

            OltSerilogMsSqlExtensions
                .WithOltMSSqlServer(new LoggerConfiguration(), " ", null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: false)
                .Should()
                .BeOfType<LoggerConfiguration>();

            OltSerilogMsSqlExtensions
                .WithOltMSSqlServer(new LoggerConfiguration(), "", null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: false)
                .Should()
                .BeOfType<LoggerConfiguration>();



            // This started attempting to connect to the sql server with version Serilog.Sinks.MSSqlServer 6.4.0, making the test fail
            //var logger = OltSerilogMsSqlExtensions
            //    .WithOltMSSqlServer(new LoggerConfiguration(), connectionString, null, null, LogEventLevel.Debug, throwInvalidConnectionStringException: true)
            //    .CreateLogger();

            //Action act = () => logger.Debug("{value1}", Faker.Lorem.Words(10).Last());
            //act.Should().NotThrow();
        }
    }
}