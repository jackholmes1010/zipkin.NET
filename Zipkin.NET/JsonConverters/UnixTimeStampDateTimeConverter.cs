using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Zipkin.NET.JsonConverters
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to a JSON string representing the value as a unix timestamp.
    /// </summary>
    public class UnixTimeStampDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var unixTime = ((DateTime)value).ToUniversalTime().Subtract(Epoch);
            writer.WriteRawValue(((long)(unixTime.TotalMilliseconds * 1000L)).ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            return Epoch.AddMilliseconds((long)reader.Value / 1000d);
        }
    }
}
