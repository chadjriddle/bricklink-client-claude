using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// JSON converter for AppearAs enum that handles BrickLink API-specific string values.
/// </summary>
public class AppearAsConverter : JsonConverter<AppearAs>
{
    private static readonly Dictionary<AppearAs, string> EnumToString = new()
    {
        { AppearAs.Alternate, "A" },
        { AppearAs.Counterpart, "C" },
        { AppearAs.Extra, "E" },
        { AppearAs.Regular, "R" }
    };

    private static readonly Dictionary<string, AppearAs> StringToEnum =
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>
    /// Reads and converts JSON to AppearAs.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The AppearAs value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid AppearAs.</exception>
    public override AppearAs Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

        var value = reader.GetString();
        if (value == null)
        {
            throw new JsonException("Expected non-null string value for AppearAs");
        }

        if (StringToEnum.TryGetValue(value, out var appearAs))
        {
            return appearAs;
        }

        throw new JsonException($"Invalid AppearAs value: {value}");
    }

    /// <summary>
    /// Writes AppearAs as JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The AppearAs value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, AppearAs value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Invalid AppearAs value: {value}");
        }
    }
}
