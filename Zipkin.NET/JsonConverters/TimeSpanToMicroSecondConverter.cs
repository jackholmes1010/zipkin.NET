using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Zipkin.NET.JsonConverters
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a JSON string representing the value in microseconds.
    /// </summary>
    public class TimeSpanToMicroSecondConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ticks = ((TimeSpan)value).Ticks / TimeSpan.TicksPerMillisecond;
            var toLong = ticks * 1000;

            writer.WriteRawValue(toLong.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }
    }
}
