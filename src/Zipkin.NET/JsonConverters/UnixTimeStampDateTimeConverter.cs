using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Zipkin.NET.Extensions;

namespace Zipkin.NET.JsonConverters
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to a JSON string representing the value as a unix timestamp.
    /// </summary>
    public class UnixTimeStampDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var unixTime = ((DateTime) value).ToUnixTime();
            writer.WriteRawValue(((long)(unixTime.TotalMilliseconds * 1000L)).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
