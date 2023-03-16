using System.Text.Json.Serialization;
using Nelibur.ObjectMapper;

namespace System.Text.Json.Extensions.Services;

internal sealed class TinyMapperUtils
{
    public static TinyMapperUtils Instance { get; } = new();

    private TinyMapperUtils()
    {
        TinyMapper.Bind<JsonSerializerOptions, JsonSerializerOptions>();
        TinyMapper.Bind<JsonConverter, JsonConverter>();
    }

    public void Map<TSource, TTarget>(TSource source, TTarget target)
    {
        TinyMapper.Map(source, target);
    }

    public T Map<T>(T source)
    {
        return TinyMapper.Map<T>(source);
    }
}