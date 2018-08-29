namespace Zipkin.NET.Instrumentation
{
    public interface ITraceIdentifierGenerator
    {
        /// <summary>
        /// Generates 64-bit identifiers.
        /// </summary>
        /// <remarks>
        /// Encoded as 16 lowercase hex characters.
        /// </remarks>
        /// <example>
        /// "ffdc9bb9a6453df3"
        /// </example>
        /// <returns></returns>
        string GenerateId();
    }
}
