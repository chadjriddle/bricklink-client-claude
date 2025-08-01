using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// JSON converter for NewOrUsed enum that handles BrickLink API-specific string values.
/// </summary>
public class NewOrUsedConverter : JsonConverter<NewOrUsed>
{
    private static readonly Dictionary<NewOrUsed, string> EnumToString = new()
    {
        { NewOrUsed.New, "N" },
        { NewOrUsed.Used, "U" }
    };

    private static readonly Dictionary<string, NewOrUsed> StringToEnum =
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>
    /// Reads and converts JSON to NewOrUsed.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The NewOrUsed value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid NewOrUsed.</exception>
    public override NewOrUsed Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

        var value = reader.GetString();
        if (value == null)
        {
            throw new JsonException("Expected non-null string value for NewOrUsed");
        }

        if (StringToEnum.TryGetValue(value, out var condition))
        {
            return condition;
        }

        throw new JsonException($"Invalid NewOrUsed value: {value}");
    }

    /// <summary>
    /// Writes NewOrUsed as JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The NewOrUsed value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, NewOrUsed value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Invalid NewOrUsed value: {value}");
        }
    }
}
