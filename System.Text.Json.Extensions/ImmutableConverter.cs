using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Extensions.Services;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Text.Json;

// https://github.com/dotnet/runtime/issues/29895
public class ImmutableConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        bool result;
        var constructors = typeToConvert.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        if (constructors.Length != 1)
        {
            result = false;
        }
        else
        {
            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var hasParameters = parameters.Length > 0;
            if (hasParameters)
            {
                var properties = typeToConvert.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                result = true;
                foreach (var parameter in parameters)
                {
                    var hasMatchingProperty = properties.Any(p => NameOfPropertyAndParameter.Matches(p.Name, parameter.Name));
                    if (!hasMatchingProperty)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }
        }

        return result;
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var valueOfProperty = new Dictionary<PropertyInfo, object>();
        var namedPropertiesMapping = GetNamedProperties(options, GetProperties(typeToConvert));
        reader.Read();
        while (true)
        {
            if (reader.TokenType != JsonTokenType.PropertyName && reader.TokenType != JsonTokenType.String)
            {
                break;
            }

            var jsonPropName = reader.GetString();
            var normalizedPropName = ConvertAndNormalizeName(jsonPropName, options);
            if (!namedPropertiesMapping.TryGetValue(normalizedPropName, out var obProp))
            {
                reader.Read();
            }
            else
            {
                var value = JsonSerializer.Deserialize(ref reader, obProp.PropertyType, options);
                reader.Read();
                valueOfProperty[obProp] = value;
            }
        }

        var ctor = typeToConvert.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First();
        var parameters = ctor.GetParameters();
        var parameterValues = new object[parameters.Length];
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameterInfo = parameters[index];
            var value = valueOfProperty.First(prop => NameOfPropertyAndParameter.Matches(prop.Key.Name, parameterInfo.Name)).Value;

            parameterValues[index] = value;
        }

        return ctor.Invoke(parameterValues);
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        var newOptions = TinyMapperUtils.Instance.Map(options);
        newOptions.Converters.Clear();

        foreach (var c in options.Converters.Where(c => !(c is ImmutableConverter)))
        {
            newOptions.Converters.Add(c);
        }

        JsonSerializer.Serialize(writer, value, newOptions);
    }

    private static PropertyInfo[] GetProperties(IReflect typeToConvert)
    {
        return typeToConvert.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    private static IReadOnlyDictionary<string, PropertyInfo> GetNamedProperties(JsonSerializerOptions options, IEnumerable<PropertyInfo> properties)
    {
        var result = new Dictionary<string, PropertyInfo>();
        foreach (var property in properties)
        {
            string name;
            var nameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }
            else
            {
                name = options.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
            }

            var normalizedName = NormalizeName(name, options);
            result.Add(normalizedName, property);
        }

        return result;
    }

    private static string ConvertAndNormalizeName(string name, JsonSerializerOptions options)
    {
        var convertedName = options.PropertyNamingPolicy?.ConvertName(name) ?? name;
        return options.PropertyNameCaseInsensitive ? convertedName.ToLowerInvariant() : convertedName;
    }

    private static string NormalizeName(string name, JsonSerializerOptions options)
    {
        return options.PropertyNameCaseInsensitive ? name.ToLowerInvariant() : name;
    }
}

internal static class NameOfPropertyAndParameter
{
    public static bool Matches(string? propertyName, string? parameterName)
    {
        if (propertyName is null && parameterName is null)
        {
            return true;
        }

        if (propertyName is null || parameterName is null)
        {
            return false;
        }

        var xRight = propertyName.AsSpan(1);
        var yRight = parameterName.AsSpan(1);
        return char.ToLowerInvariant(propertyName[0]).CompareTo(parameterName[0]) == 0 && xRight.Equals(yRight, StringComparison.Ordinal);
    }
}