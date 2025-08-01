using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// JSON converter for Completeness enum that handles BrickLink API-specific string values.
/// </summary>
public class CompletenessConverter : JsonConverter<Completeness>
{
    private static readonly Dictionary<Completeness, string> EnumToString = new()
    {
        { Completeness.Complete, "C" },
        { Completeness.Incomplete, "B" },
        { Completeness.Sealed, "S" }
    };

    private static readonly Dictionary<string, Completeness> StringToEnum = 
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>
    /// Reads and converts JSON to Completeness.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The Completeness value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid Completeness.</exception>
    public override Completeness Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

        var value = reader.GetString();
        if (value == null)
        {
            throw new JsonException("Expected non-null string value for Completeness");
        }

        if (StringToEnum.TryGetValue(value, out var completeness))
        {
            return completeness;
        }

        throw new JsonException($"Invalid Completeness value: {value}");
    }

    /// <summary>
    /// Writes Completeness as JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The Completeness value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Completeness value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Invalid Completeness value: {value}");
        }
    }
}