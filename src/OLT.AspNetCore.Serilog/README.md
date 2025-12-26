[![CI](https://github.com/OuterlimitsTech/olt-dotnet-core/actions/workflows/build.yml/badge.svg)](https://github.com/OuterlimitsTech/olt-dotnet-core/actions/workflows/build.yml) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=OuterlimitsTech_olt-dotnet-core&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=OuterlimitsTech_olt-dotnet-core)

## OLT Library Serilog AspnetCore Package


- ### _How To:_ Configure Services to include Payload logging middleware and user session middleware

```csharp
services.AddOltSerilog(configOptions => configOptions.ShowExceptionDetails = AppSettings.Hosting.ShowExceptionDetails);
```


- ### _How To:_ Include MSSqlServer Sink Defaults (install-package OLT.Logging.Serilog.MSSqlServer)
```csharp
services.WithOltMSSqlServer(sqlConnectionString);
```


- ### _How To:_ Configure Services to include Payload logging middleware and user session middleware

```csharp
services.AddOltSerilog(configOptions => configOptions.ShowExceptionDetails = AppSettings.Hosting.ShowExceptionDetails);
```

- ### _How To:_ Log a OltNgxLoggerMessageJson from a controller (see [ngx-logger](https://github.com/dbfannin/ngx-logger))

```csharp
[AllowAnonymous]
[HttpPost, Route("")]
public ActionResult<string> Log([FromBody] OLT.Logging.Serilog.OltNgxLoggerMessageJson message)
{
    Serilog.Log.Logger.Write(message);
    return Ok("Received");
}
```

- ### Sample ngx-logger JSON

```json
{
  "level": 6,
  "additional": [
    [
      {
        "name": "HttpErrorResponse",
        "appId": "my-app",
        "user": "test.user@testing.com",
        "time": 1646258802617,
        "id": "app-test.user@testing.com-1646258802617",
        "url": null,
        "status": null,
        "message": "Http failure response for https://localhost:45687/api/customers/1111/images/base64?aspectValue=80&aspectRatio=width&api-version=1.0: 500 OK",
        "stack": null
      },
      {
        "name": "HttpErrorResponse",
        "appId": "my-app",
        "user": "test.user@testing.com",
        "time": 1646258802617,
        "id": "app-test.user@testing.com-1646258802617",
        "url": "/queues/my-queue",
        "status": 404,
        "message": "Http failure response for https://localhost:45687/api/customers/1111/images/base64?aspectValue=80&aspectRatio=width&api-version=1.0: 404 OK",
        "stack": [
          {
            "source": "foobar",
            "functionName": "getValue",
            "fileName": "vendor.js",
            "lineNumber": 131598,
            "columnNumber": 45
          },
          {
            "source": "foobar2",
            "functionName": "setValue",
            "fileName": "vendor.js",
            "lineNumber": 131998,
            "columnNumber": 75
          }
        ]
      }
    ]
  ],
  "message": "Http failure response for https://localhost:45687/api/customers/1111/images/base64?aspectValue=80&aspectRatio=width&api-version=1.0: 404 OK",
  "timestamp": "2022-03-02T22:06:42.617Z",
  "fileName": "vendor.js",
  "lineNumber": 81349,
  "columnNumber": 21
}
```
