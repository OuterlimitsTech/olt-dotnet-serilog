using OLT.Constants;
using OLT.Logging.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;

namespace OLT.Core
{

    public static class OltSerilogMsSqlExtensions
    {

        /// <summary>
        /// Configure SQL Logging Table
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <param name="connectionString"></param>
        /// <param name="options"></param>
        /// <param name="columnOptions"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="throwInvalidConnectionStringException"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static LoggerConfiguration WithOltMSSqlServer(this LoggerConfiguration loggerConfiguration, 
            string? connectionString, 
            MSSqlServerSinkOptions? options = null, 
            ColumnOptions? columnOptions = null, 
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information,
            bool throwInvalidConnectionStringException = false)
        {
            ArgumentNullException.ThrowIfNull(loggerConfiguration);

            if (connectionString == null)
            {
                if (throwInvalidConnectionStringException)
                {
                    ArgumentNullException.ThrowIfNull(connectionString);
                }
                return loggerConfiguration;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                if (throwInvalidConnectionStringException)
                {
                    throw new ArgumentOutOfRangeException(nameof(connectionString), $"connectionString='{connectionString}'");
                }
                return loggerConfiguration;
            }

            options = options ?? DefaultSinkOptions;
            columnOptions = columnOptions ?? DefaultColumnOptions;

            loggerConfiguration
                .WriteTo.MSSqlServer(
                    connectionString,
                    restrictedToMinimumLevel: restrictedToMinimumLevel,
                    sinkOptions: options,
                    columnOptions: columnOptions);

            return loggerConfiguration;
        }

        public static MSSqlServerSinkOptions DefaultSinkOptions
        {
            get
            {
                return new MSSqlServerSinkOptions
                {
                    TableName = OltSerilogMsSqlConstants.Table.Name,
                    SchemaName = OltSerilogMsSqlConstants.Table.Schema,
                    AutoCreateSqlTable = true,
                };
            }
        }

        public static ColumnOptions DefaultColumnOptions
        {
            get
            {
                var additionalColumns = new List<SqlColumn>
                {
                    new SqlColumn
                        {ColumnName = OltSerilogMsSqlConstants.ColumnNames.Application, PropertyName = OltSerilogConstants.Properties.Application, DataType = SqlDbType.NVarChar, DataLength = 100},

                    new SqlColumn
                        {ColumnName = OltSerilogConstants.Properties.EventType, PropertyName = OltSerilogConstants.Properties.EventType, DataType = SqlDbType.NVarChar, DataLength = 20},

                    new SqlColumn
                        {ColumnName = OltSerilogConstants.Properties.Username, DataType = SqlDbType.NVarChar, DataLength = 255},

                    new SqlColumn
                        {ColumnName = OltSerilogConstants.Properties.DbUsername, DataType = SqlDbType.NVarChar, DataLength = 255},

                    new SqlColumn
                        {ColumnName = OltSerilogMsSqlConstants.ColumnNames.RequestPath, DataType = SqlDbType.NVarChar, DataLength = -1},

                    new SqlColumn
                        {ColumnName = OltSerilogMsSqlConstants.ColumnNames.Source, DataType = SqlDbType.NVarChar, DataLength = 255},
                };

                return new ColumnOptions
                {
                    Id = { ColumnName = OltSerilogMsSqlConstants.ColumnNames.Id },
                    TimeStamp = { DataType = SqlDbType.DateTimeOffset },
                    AdditionalColumns = additionalColumns
                };
            }
        }
    }


}
