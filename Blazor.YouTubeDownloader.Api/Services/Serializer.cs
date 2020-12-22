using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Blazor.YouTubeDownloader.Api.Services
{
    internal class Serializer : ISerializer
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new ImmutableConverter() }
        };

        public ValueTask<T?> DeserializeAsync<T>(Stream stream) where T : class
        {
            return JsonSerializer.DeserializeAsync<T>(stream, _jsonSerializerOptions);
        }
    }
}