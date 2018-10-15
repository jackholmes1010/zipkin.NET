namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Injectors are used to inject span context into an object
    /// in order to propagate span context across process boundaries.
    /// </summary>
    /// <typeparam name="TInject">
    /// The type of object to inject span context.
    /// </typeparam>
    public interface ISpanContextInjector<TInject>
    {
        /// <summary>
        /// Injects a <see cref="SpanContext"/> into a <see cref="TInject"/>.
        /// </summary>
        /// <param name="inject">
        /// The object to inject span context.
        /// </param>
        /// <param name="spanContext">
        /// The <see cref="SpanContext"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TInject"/> with span context.
        /// </returns>
        TInject Inject(TInject inject, SpanContext spanContext);
    }
}
