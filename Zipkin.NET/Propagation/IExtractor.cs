namespace Zipkin.NET.Propagation
{
    /// <summary>
    /// Extractors are used to trace context from incoming requests.
    /// </summary>
    /// <typeparam name="TExtract">
    /// The object type from which to extract a <see cref="Trace"/>.
    /// </typeparam>
    public interface IExtractor<in TExtract>
    {
        /// <summary>
        /// Extract a <see cref="Trace"/> from a <see cref="TExtract"/>.
        /// </summary>
        /// <param name="extract">
        /// The object to extract the trace from.
        /// </param>
        /// <returns>
        /// The <see cref="Trace"/>.
        /// </returns>
        Trace Extract(TExtract extract);
    }
}
