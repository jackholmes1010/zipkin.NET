namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Extractors are used extract span context from incoming requests.
    /// </summary>
    /// <typeparam name="TExtract">
    /// The object type from which to extract a <see cref="SpanContext"/>.
    /// </typeparam>
    public interface IExtractor<in TExtract>
    {
        /// <summary>
        /// Extract a <see cref="SpanContext"/> from a <see cref="TExtract"/>.
        /// </summary>
        /// <param name="extract">
        /// The object to extract the span context from.
        /// </param>
        /// <returns>
        /// The <see cref="SpanContext"/>.
        /// </returns>
        SpanContext Extract(TExtract extract);
    }
}
