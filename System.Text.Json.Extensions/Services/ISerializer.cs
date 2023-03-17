using System.IO;
using System.Threading.Tasks;

namespace System.Text.Json.Extensions.Services
{
    public interface ISerializer
    {
        string Serialize<T>(T[] values);

        string Serialize<T>(T value);

        ValueTask<T?> DeserializeAsync<T>(Stream stream) where T : class;
    }
}