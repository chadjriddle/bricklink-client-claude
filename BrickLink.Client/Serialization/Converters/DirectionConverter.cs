using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// JSON converter for Direction enum that handles BrickLink API-specific string values.
/// </summary>
public class DirectionConverter : JsonConverter<Direction>
{
    private static readonly Dictionary<Direction, string> EnumToString = new()
    {
        { Direction.In, "in" },
        { Direction.Out, "out" }
    };

    private static readonly Dictionary<string, Direction> StringToEnum = 
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>
    /// Reads and converts JSON to Direction.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The Direction value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid Direction.</exception>
    public override Direction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

        var value = reader.GetString();
        if (value == null)
        {
            throw new JsonException("Expected non-null string value for Direction");
        }

        if (StringToEnum.TryGetValue(value, out var direction))
        {
            return direction;
        }

        throw new JsonException($"Invalid Direction value: {value}");
    }

    /// <summary>
    /// Writes Direction as JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The Direction value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, Direction value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Invalid Direction value: {value}");
        }
    }
}