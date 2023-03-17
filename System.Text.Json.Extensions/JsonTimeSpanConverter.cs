using System.Globalization;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Text.Json
{
    // Copied from https://github.com/smokedlinq/Extensions.System.Text.Json/
    public class JsonTimeSpanConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(TimeSpan) || typeToConvert == typeof(TimeSpan?);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => typeToConvert.IsGenericType
                ? (JsonConverter)new JsonNullableTimeSpanConverter()
                : new JsonStandardTimeSpanConverter();

        private class JsonStandardTimeSpanConverter : JsonConverter<TimeSpan>
        {
            public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => reader.TokenType != JsonTokenType.String
                    ? throw new JsonException()
                    : TimeSpan.ParseExact(reader.GetString(), "c", CultureInfo.InvariantCulture);

            public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
        }

        private class JsonNullableTimeSpanConverter : JsonConverter<TimeSpan?>
        {
            public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }

                return reader.TokenType != JsonTokenType.String
                    ? throw new JsonException()
                    : TimeSpan.ParseExact(reader.GetString(), "c", CultureInfo.InvariantCulture);
            }

            public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
            {
                if (value is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteStringValue(value.Value.ToString("c", CultureInfo.InvariantCulture));
                }
            }
        }
    }
}