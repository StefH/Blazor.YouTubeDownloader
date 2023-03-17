using System.IO;
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

        public string Serialize<T>(T value)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonTimeSpanConverter(),
                    new ImmutableConverter<T>()
                }
            };

            return JsonSerializer.Serialize(value, jsonSerializerOptions);
        }

        public ValueTask<T?> DeserializeAsync<T>(Stream stream) where T : class
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonTimeSpanConverter(),
                    new ImmutableConverter<T>()
                }
            };
            return JsonSerializer.DeserializeAsync<T>(stream, jsonSerializerOptions);
        }
    }
}