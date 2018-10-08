# zipkin.NET
Zipkin instrumentation for .NET with support for .NET Core, OWIN and WCF.

Uses Zipkin V2 format.

## Client Tracing
The ```TracingHandler``` delegating handler can be used to create spans and propagate trace context for outgoing HTTP requests.

For outgoing WCF requests, the ```EndpointTracingBehavior``` can be used to add a ```ClientTracingMessageInspector``` to the client runtime.

## .NET Core
Add dependencies to the service collection.
```csharp
// Register Zipkin server reporter.
// This reporter sends completed spans to a Zipkin 
// server's HTTP collector (POST api/v2/spans).
services.AddSingleton<IReporter>(provider =>
{
	var sender = new ZipkinHttpSender("http://localhost:9411");
	var reporter = new ZipkinReporter(sender);
	return reporter;
});

// Register .NET Core ILogger span reporter.
// This reporter logs completed spans using the .NET Core ILogger.
services.AddSingleton<IReporter, LoggerReporter>();

// Register default tracing dependencies.
services.AddTracing("example-api", 1f);
```

Add the ```TracingMiddleware``` to the pipeline.
```csharp
// Middleware builds server spans from incoming requests
// and reports them using the registered dispatcher.
app.UseMiddleware<TracingMiddleware>();
```
Using the ```HttpClientFactory```, create an HTTP client with a ```TracingHandler```.
```csharp
// The TracingHandler builds client spans from outgoing HTTP
// requests and reports them using the registered dispatcher.
services.AddHttpClient("my-http-client").AddTracingMessageHandler("my-other-api");
```
## OWIN
Register dependencies, for example, if using Autofac.
```csharp
var builder = new ContainerBuilder();
builder.Register(ctx => new ZipkinHttpSender("http://localhost:9411")).As<ISender>().SingleInstance();
builder.RegisterType<AsyncActionBlockDispatcher>().As<IDispatcher>().SingleInstance();
builder.RegisterType<CallContextTraceContextAccessor>().As<ITraceContextAccessor>().SingleInstance();
builder.RegisterType<ConsoleInstrumentationLogger>().As<IInstrumentationLogger>().SingleInstance();
builder.RegisterType<RateSampler>().As<ISampler>().WithParameter("rate", 1f).SingleInstance();
builder.RegisterType<ConsoleReporter>().As<IReporter>().SingleInstance();
builder.RegisterType<ZipkinReporter>().As<IReporter>().SingleInstance();
builder.RegisterType<TracingMiddleware>().WithParameter("localEndpointName", "owin-api");
```
Add the OWIN ```TracingMiddleware``` to the pipeline.
```csharp
app.UseMiddlewareFromContainer<TracingMiddleware>();
```
