[![CI](https://github.com/OuterlimitsTech/olt-dotnet-core/actions/workflows/build.yml/badge.svg)](https://github.com/OuterlimitsTech/olt-dotnet-core/actions/workflows/build.yml) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=OuterlimitsTech_olt-dotnet-core&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=OuterlimitsTech_olt-dotnet-core)


# OLT.Logging.Serilog.Enricher - `OltEventTypeEnricher`

`OltEventTypeEnricher` is a Serilog enricher that attaches a deterministic, compact event-type identifier to each log event based on the log message template text. This enables grouping, deduplication, and analysis of log events by template rather than by rendered message.

Key points
- Property added: `OltSerilogConstants.Properties.EventType` (constant provided in the library).
- Hash algorithm: Murmur3 32-bit (Murmur32).
- Input: `LogEvent.MessageTemplate.Text` (the message template, not the rendered message).
- Output: Uppercase hexadecimal string representation of the 4-byte Murmur3 hash (e.g. `9A3F2B7C`).
- Purpose: Create a stable identifier for a message template so queries, dashboards, or telemetry systems can aggregate or correlate events by template.

Why use this enricher
- Identify the logical event type even when structured properties vary per occurrence.
- Group similar events in dashboards or alerts.
- Reduce storage/aggregation complexity by using an identifier instead of full templates.
- Lightweight and deterministic: same template → same identifier across services.

- Explain algorithm:
  - Create a new Murmur3 32-bit hasher (`MurmurHash.Create32()`).
  - Convert `logEvent.MessageTemplate.Text` to UTF-8 bytes.
  - Compute 32-bit Murmur hash bytes.
  - Convert hash bytes to uppercase hexadecimal string (`Convert.ToHexString`).
  - Create a Serilog `LogEventProperty` with the property name constant `OltSerilogConstants.Properties.EventType`.
  - Attach the property to the `LogEvent` using `AddPropertyIfAbsent`.


- ### _How To:_ Configure 

```csharp
services.WithOltMSSqlServer();
```



