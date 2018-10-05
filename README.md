# zipkin.NET
Zipkin instrumentation for .NET with support for .NET Core, OWIN and WCF.

## .NET Core
Add dependencies to the service collection.
```csharp
// Register Zipkin server reporter.
// This reporter sends completed spans to a Zipkin 
// server's HTTP collector (POST api/v2/spans).
services.TryAddTransient<IReporter>(provider =>
{
    var sender = new ZipkinHttpSender("http://localhost:9411");
    var reporter = new ZipkinReporter(sender);
    return reporter;
});

// Register .NET Core ILogger span reporter.
// This reporter logs completed spans using the .NET Core ILogger.
services.TryAddTransient<IReporter, LoggerReporter>();

// Register default tracing dependencies.
services.AddTracing("test-api");
```
And add the ```TracingMiddleware``` to the pipeline.
```csharp
app.UseMiddleware<TracingMiddleware>();
```
