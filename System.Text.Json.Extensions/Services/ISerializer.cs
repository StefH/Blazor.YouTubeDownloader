using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace System.Text.Json.Extensions.Services
{
    public interface ISerializer
    {
        string Serialize<T>(T[] values);

        string Serialize<T>(T value, IReadOnlyList<JsonConverter> extraJsonConverters);

        ValueTask<T?> DeserializeAsync<T>(Stream stream, IReadOnlyList<JsonConverter> extraJsonConverters) where T : class;
    }
}