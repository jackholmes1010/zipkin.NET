using Zipkin.NET.Models;

namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Propagators are used to pass span context between distributed systems.
    /// </summary>
    /// <typeparam name="TInject">
    /// The type on which to inject a <see cref="Span"/>.
    /// </typeparam>
    public interface IPropagator<TInject>
    {
        /// <summary>
        /// Inject <see cref="Span"/> details into a <see cref="TInject"/>.
        /// </summary>
        /// <param name="inject">
        /// The object to inject span details into.
        /// </param>
        /// <param name="span">
        /// The <see cref="Span"/>.
        /// </param>
        /// <returns>
        /// The <see cref="TInject"/> object with span details.
        /// </returns>
        TInject Inject(TInject inject, Span span);
    }
}
