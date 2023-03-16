using System.IO;
using System.Threading.Tasks;

namespace System.Text.Json.Extensions.Services
{
    public class Serializer : ISerializer
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonTimeSpanConverter(),
                new ImmutableConverter()
            }
        };

        public string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, _jsonSerializerOptions);
        }

        public ValueTask<T?> DeserializeAsync<T>(Stream stream) where T : class
        {
            return JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);
        }
    }
}