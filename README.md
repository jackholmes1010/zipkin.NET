# zipkin.NET
Zipkin instrumentation for .NET with support for .NET Core, OWIN and WCF.

## Client Tracing
The ```TracingHandler``` delegating handler can be used to create spans and propagate trace context for outgoing HTTP requests.

For outgoing WCF requests, the ```EndpointTracingBehavior``` can be used to add a ```ClientTracingMessageInspector``` to the client runtime.

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

Add the ```TracingMiddleware``` to the pipeline.
```csharp
// Middleware creates server spans from incoming requests
// and reports them using the registered dispatcher.
app.UseMiddleware<TracingMiddleware>();
```
Create a new HTTP client which uses a ```TracingHandler```.
```csharp
// Add a tracing handler to the HTTP clients.
// The TracingHandler builds client from outgoing HTTP
// requests and reports them using the registered dispatcher.
services.AddHttpClient("tracingClient").AddTracingMessageHandler("my-other-api");
```
