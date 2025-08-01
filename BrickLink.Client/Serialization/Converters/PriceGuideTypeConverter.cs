using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// JSON converter for PriceGuideType enum that handles BrickLink API-specific string values.
/// </summary>
public class PriceGuideTypeConverter : JsonConverter<PriceGuideType>
{
    private static readonly Dictionary<PriceGuideType, string> EnumToString = new()
    {
        { PriceGuideType.Stock, "stock" },
        { PriceGuideType.Sold, "sold" }
    };

    private static readonly Dictionary<string, PriceGuideType> StringToEnum =
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>
    /// Reads and converts JSON to PriceGuideType.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The PriceGuideType value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid PriceGuideType.</exception>
    public override PriceGuideType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

        var value = reader.GetString();
        if (value == null)
        {
            throw new JsonException("Expected non-null string value for PriceGuideType");
        }

        if (StringToEnum.TryGetValue(value, out var guideType))
        {
            return guideType;
        }

        throw new JsonException($"Invalid PriceGuideType value: {value}");
    }

    /// <summary>
    /// Writes PriceGuideType as JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The PriceGuideType value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, PriceGuideType value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Invalid PriceGuideType value: {value}");
        }
    }
}
