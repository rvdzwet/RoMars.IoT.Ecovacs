using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RoMars.IoT.Ecovacs.Infrastructure.JsonConverters;

/// <summary>
/// JSON converter factory for enums that use string values with JsonPropertyName attributes.
/// </summary>
public class StringValueEnumConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(StringValueEnumConverterInner<>).MakeGenericType(typeToConvert),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null)!;

        return converter;
    }

    private class StringValueEnumConverterInner<T> : JsonConverter<T> where T : struct, Enum
    {
        private readonly Dictionary<string, T> _stringToEnum = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<T, string> _enumToString = new Dictionary<T, string>();

        public StringValueEnumConverterInner(JsonSerializerOptions options)
        {
            var enumType = typeof(T);
            foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var jsonPropertyName = field.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
                var enumValue = (T)field.GetValue(null)!;

                string key = jsonPropertyName ?? field.Name;
                _stringToEnum[key] = enumValue;
                _enumToString[enumValue] = key;
            }
        }

        /// <inheritdoc/>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected string for enum type {typeToConvert.Name}");

            string? jsonString = reader.GetString();

            if (jsonString != null && _stringToEnum.TryGetValue(jsonString, out T enumValue))
            {
                return enumValue;
            }

            return (T)(object)-1;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (_enumToString.TryGetValue(value, out string? stringValue))
            {
                writer.WriteStringValue(stringValue);
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
