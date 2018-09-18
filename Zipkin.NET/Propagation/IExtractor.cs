using Zipkin.NET.Models;

namespace Zipkin.NET.Propagation
{
	/// <summary>
	/// Extractors are used to extract spans from incoming requests.
	/// </summary>
	/// <typeparam name="TExtract">
	/// The object type from which to extract a <see cref="Span"/>.
	/// </typeparam>
	public interface IExtractor<in TExtract>
    {
		/// <summary>
		/// Extract a <see cref="Span"/> from a <see cref="TExtract"/>.
		/// </summary>
		/// <param name="extract">
		/// The object to extract the trace from.
		/// </param>
		/// <returns>
		/// The <see cref="Span"/>.
		/// </returns>
		Span Extract(TExtract extract);
    }
}
