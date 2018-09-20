namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Extractors are used extract trace context from incoming requests.
    /// </summary>
    /// <typeparam name="TExtract">
    /// The object type from which to extract a <see cref="TraceContext"/>.
    /// </typeparam>
    public interface IExtractor<in TExtract>
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
        TraceContext Extract(TExtract extract);
    }
}
