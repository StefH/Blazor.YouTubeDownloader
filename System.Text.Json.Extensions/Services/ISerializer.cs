using System.IO;
using System.Threading.Tasks;

namespace System.Text.Json.Extensions.Services
{
    public interface ISerializer
    {
        string Serialize(object value);

        ValueTask<T?> DeserializeAsync<T>(Stream stream) where T : class;
    }
}