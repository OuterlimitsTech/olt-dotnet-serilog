[![CI](https://github.com/OuterlimitsTech/olt-dotnet-core/actions/workflows/build.yml/badge.svg)](https://github.com/OuterlimitsTech/olt-dotnet-core/actions/workflows/build.yml) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=OuterlimitsTech_olt-dotnet-core&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=OuterlimitsTech_olt-dotnet-core)

# OLT.Logging.Serilog.Abstractions

Core abstractions, constants, and models for OLT Serilog extensions. This library provides shared types and constants used across the OLT Serilog ecosystem.

## Features

- **Serilog Constants** - Standardized property names, templates, and format strings
- **NGX Logger Support** - Models and extensions for handling Angular ngx-logger messages


## Components

### OltSerilogConstants

A set of partial static classes containing constants for Serilog configuration and usage.

#### Properties

Standard property names for Serilog enrichment:

```csharp
OltSerilogConstants.Properties.Username           // "Username"
OltSerilogConstants.Properties.DbUsername         // "DbUsername"
OltSerilogConstants.Properties.EventType          // "OltEventType"
OltSerilogConstants.Properties.Environment        // "Environment"
OltSerilogConstants.Properties.Application        // "Application"
```

ASP.NET Core specific properties:

```csharp
OltSerilogConstants.Properties.AspNetCore.AppRequestUid    // "AppRequestUid"
OltSerilogConstants.Properties.AspNetCore.RequestHeaders   // "RequestHeaders"
OltSerilogConstants.Properties.AspNetCore.ResponseHeaders  // "ResponseHeaders"
OltSerilogConstants.Properties.AspNetCore.RequestBody      // "RequestBody"
OltSerilogConstants.Properties.AspNetCore.ResponseBody     // "ResponseBody"
OltSerilogConstants.Properties.AspNetCore.RequestUri       // "RequestUri"
```

NGX Logger properties:

```csharp
OltSerilogConstants.Properties.NgxMessage.NgxDetail        // "NgxDetail"
```

#### Templates

Pre-defined output templates for Serilog:

```csharp
// Default console output template
OltSerilogConstants.Templates.DefaultOutput
// "[{Timestamp:HH:mm:ss} {Level:u3}] {OltEventType:x8} {Message:lj}{NewLine}{Exception}"

// ASP.NET Core templates
OltSerilogConstants.Templates.AspNetCore.ServerError
OltSerilogConstants.Templates.AspNetCore.Payload

// Email templates
OltSerilogConstants.Templates.Email.DefaultEmail
OltSerilogConstants.Templates.Email.DefaultSubject

// NGX Logger template
OltSerilogConstants.Templates.NgxMessage.Template
```

#### Format Strings

Standard date/time format strings:

```csharp
OltSerilogConstants.FormatString.ISO8601  // "yyyy-MM-ddTHH:mm:ss.fffZ"
```

### NGX Logger Support

Models and extensions for handling logs from [ngx-logger](https://www.npmjs.com/package/ngx-logger), an Angular logging library.

#### OltNgxLoggerLevel

Enum matching ngx-logger log levels:

```csharp
public enum OltNgxLoggerLevel
{
    Trace = 0,
    Debug,
    Information,
    Log,
    Warning,
    Error,
    Fatal,
    Off
}
```

#### OltNgxLoggerMessageJson

Main log message model for deserializing ngx-logger payloads:

```csharp
public class OltNgxLoggerMessageJson
{
    public string? Message { get; set; }
    public List<List<OltNgxLoggerDetailJson>> Additional { get; set; }
    public OltNgxLoggerLevel? Level { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public string? FileName { get; set; }
    public int? LineNumber { get; set; }
    public int? ColumnNumber { get; set; }
    public bool IsError { get; }  // True if Level is Fatal or Error

    public string GetUsername();
    public string? FormatMessage();
    public Exception ToException();
}
```

#### OltNgxLoggerDetailJson

Additional detail information from ngx-logger:

```csharp
public class OltNgxLoggerDetailJson
{
    public string? Name { get; set; }
    public string? AppId { get; set; }
    public string? User { get; set; }
    public long? Time { get; set; }
    public string? Id { get; set; }
    public string? Url { get; set; }
    public int? Status { get; set; }
    public string? Message { get; set; }
    public List<OltNgxLoggerStackJson>? Stack { get; set; }

    public ApplicationException ToException();
}
```

#### OltNgxLoggerStackJson

Stack trace information from ngx-logger JavaScript errors:

```csharp
public class OltNgxLoggerStackJson
{
    public string? FileName { get; set; }
    public string? FunctionName { get; set; }
    public int? LineNumber { get; set; }
    public int? ColumnNumber { get; set; }
    public string? Source { get; set; }
}
```

#### OltNgxLoggerStackExtensions

Extension methods for formatting stack traces:

```csharp
public static class OltNgxLoggerStackExtensions
{
    public static string FormatStack(this OltNgxLoggerStackJson value);
    public static string FormatStack(this List<OltNgxLoggerStackJson> stack);
}
```

## Usage

Reference this package from other OLT Serilog libraries or use the constants and models directly in your application:

```csharp
using OLT.Logging.Serilog;

// Use standardized property names
Log.ForContext(OltSerilogConstants.Properties.Username, "john.doe")
   .Information("User action performed");

// Parse ngx-logger messages
var ngxMessage = JsonSerializer.Deserialize<OltNgxLoggerMessageJson>(jsonPayload);
if (ngxMessage.IsError)
{
    var exception = ngxMessage.ToException();
    Log.Error(exception, ngxMessage.FormatMessage());
}
```

## Related Packages

- `OLT.Logging.Serilog` - Core Serilog extensions and enrichers
- `OLT.AspNetCore.Serilog` - ASP.NET Core middleware and extensions
- `OLT.Logging.Serilog.Hosting` - Hosting configuration helpers
- `OLT.Logging.Serilog.MSSqlServer` - SQL Server sink configuration

