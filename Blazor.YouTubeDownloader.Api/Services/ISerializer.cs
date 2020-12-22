using System.IO;
using System.Threading.Tasks;

namespace Blazor.YouTubeDownloader.Api.Services
{
    public interface ISerializer
    {
        ValueTask<T?> DeserializeAsync<T>(Stream stream) where T : class;
    }
}
