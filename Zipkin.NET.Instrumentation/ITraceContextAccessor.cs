namespace Zipkin.NET.Instrumentation
{
	/// <summary>
	/// Provides a shared <see cref="TraceContext"/> across the application.
	/// </summary>
    public interface ITraceContextAccessor
    {
		/// <summary>
		/// A <see cref="TraceContext"/> containing information about the current trace.
		/// </summary>
        TraceContext Context { get; set; }
    }
}
