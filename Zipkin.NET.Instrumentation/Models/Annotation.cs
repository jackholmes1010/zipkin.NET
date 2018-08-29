using Newtonsoft.Json;

namespace Zipkin.NET.Instrumentation.Models
{
    /// <summary>
    /// Associates an event that explains latency with a timestamp.
    /// </summary>
    /// <remarks>
    /// Zipkin v1 core annotations such as "cs" and "sr" have been 
    /// replaced with Span.Kind, which interprets timestamp and duration.
    /// </remarks>
    [JsonObject(MemberSerialization.OptIn)]
    public class Annotation
    {
        /// <summary>
        /// Epoch microseconds of the start of this span.
        /// </summary>
        /// <example>
        /// 1502787600000000 corresponds to 2017-08-15 09:00 UTC.
        /// </example>
        [JsonProperty("timestamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// Usually a short tag indicating an event.
        /// </summary>
        /// <example>
        /// "error".
        /// </example>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
