namespace Zipkin.NET.Instrumentation
{
	/// <summary>
	/// Propagators are used to pass trace context between distributed systems.
	/// </summary>
	/// <typeparam name="TExtract">
	/// The type from which to extract a <see cref="TraceContext"/>.
	/// </typeparam>
	/// <typeparam name="TInject">
	/// The type on which to inject a <see cref="TraceContext"/>.
	/// </typeparam>
	public interface IPropagator<TExtract, TInject>
	{
		/// <summary>
		/// Extract a <see cref="TraceContext"/> from a <see cref="TExtract"/>.
		/// </summary>
		/// <param name="extract">
		/// The object to extract the trace context from.
		/// </param>
		/// <returns>
		/// The <see cref="TraceContext"/>.
		/// </returns>
		TraceContext Extract (TExtract extract);

		/// <summary>
		/// Inject a <see cref="TraceContext"/> into a <see cref="TInject"/>.
		/// </summary>
		/// <param name="inject">
		/// The object to inject trace context to.
		/// </param>
		/// <param name="context">
		/// The <see cref="TraceContext"/>.
		/// </param>
		/// <returns>
		/// The inject object containing the trace context.
		/// </returns>
		TInject Inject(TInject inject, TraceContext context);
	}
}
