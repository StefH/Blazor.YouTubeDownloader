using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace System.Text.Json.Extensions.Services
{
    public class Serializer : ISerializer
    {
        public string Serialize<T>(T[] values)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonTimeSpanConverter(),
                    new ImmutableConverter<T>()
                }
            };

            return JsonSerializer.Serialize(values, jsonSerializerOptions);
        }

        public string Serialize<T>(T value, IReadOnlyList<JsonConverter> extraJsonConverters)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonTimeSpanConverter(),
                    new ImmutableConverter<T>()
                }
            };

            foreach (var extraJsonConverter in extraJsonConverters)
            {
                jsonSerializerOptions.Converters.Add(extraJsonConverter);
            }
            
            return JsonSerializer.Serialize(value, jsonSerializerOptions);
        }

        public ValueTask<T?> DeserializeAsync<T>(Stream stream, IReadOnlyList<JsonConverter> extraJsonConverters) where T : class
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonTimeSpanConverter(),
                   // new ImmutableConverter<T>()
                }
            };

            foreach (var extraJsonConverter in extraJsonConverters)
            {
                jsonSerializerOptions.Converters.Add(extraJsonConverter);
            }

            return JsonSerializer.DeserializeAsync<T>(stream, jsonSerializerOptions);
        }
    }
}