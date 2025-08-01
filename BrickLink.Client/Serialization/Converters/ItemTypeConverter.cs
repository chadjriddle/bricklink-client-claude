using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Serialization.Converters;

/// <summary>
/// JSON converter for ItemType enum that handles BrickLink API-specific string values.
/// </summary>
public class ItemTypeConverter : JsonConverter<ItemType>
{
    private static readonly Dictionary<ItemType, string> EnumToString = new()
    {
        { ItemType.Minifig, "MINIFIG" },
        { ItemType.Part, "PART" },
        { ItemType.Set, "SET" },
        { ItemType.Book, "BOOK" },
        { ItemType.Gear, "GEAR" },
        { ItemType.Catalog, "CATALOG" },
        { ItemType.Instruction, "INSTRUCTION" },
        { ItemType.UnsortedLot, "UNSORTED_LOT" },
        { ItemType.OriginalBox, "ORIGINAL_BOX" }
    };

    private static readonly Dictionary<string, ItemType> StringToEnum = 
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <summary>
    /// Reads and converts JSON to ItemType.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The ItemType value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is not a valid ItemType.</exception>
    public override ItemType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token but got {reader.TokenType}");
        }

        var value = reader.GetString();
        if (value == null)
        {
            throw new JsonException("Expected non-null string value for ItemType");
        }

        if (StringToEnum.TryGetValue(value, out var itemType))
        {
            return itemType;
        }

        throw new JsonException($"Invalid ItemType value: {value}");
    }

    /// <summary>
    /// Writes ItemType as JSON string.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The ItemType value to write.</param>
    /// <param name="options">The serializer options.</param>
    public override void Write(Utf8JsonWriter writer, ItemType value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            throw new JsonException($"Invalid ItemType value: {value}");
        }
    }
}