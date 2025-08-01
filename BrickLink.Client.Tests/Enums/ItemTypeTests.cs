using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Enums;

/// <summary>
/// Unit tests for the ItemType enum.
/// </summary>
public class ItemTypeTests
{
    [Theory]
    [InlineData(ItemType.Minifig, "MINIFIG")]
    [InlineData(ItemType.Part, "PART")]
    [InlineData(ItemType.Set, "SET")]
    [InlineData(ItemType.Book, "BOOK")]
    [InlineData(ItemType.Gear, "GEAR")]
    [InlineData(ItemType.Catalog, "CATALOG")]
    [InlineData(ItemType.Instruction, "INSTRUCTION")]
    [InlineData(ItemType.UnsortedLot, "UNSORTED_LOT")]
    [InlineData(ItemType.OriginalBox, "ORIGINAL_BOX")]
    public void ItemType_SerializesToCorrectJsonValue(ItemType itemType, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(itemType, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal($"\"{expectedJson}\"", json);
    }

    [Theory]
    [InlineData("\"MINIFIG\"", ItemType.Minifig)]
    [InlineData("\"PART\"", ItemType.Part)]
    [InlineData("\"SET\"", ItemType.Set)]
    [InlineData("\"BOOK\"", ItemType.Book)]
    [InlineData("\"GEAR\"", ItemType.Gear)]
    [InlineData("\"CATALOG\"", ItemType.Catalog)]
    [InlineData("\"INSTRUCTION\"", ItemType.Instruction)]
    [InlineData("\"UNSORTED_LOT\"", ItemType.UnsortedLot)]
    [InlineData("\"ORIGINAL_BOX\"", ItemType.OriginalBox)]
    public void ItemType_DeserializesFromCorrectJsonValue(string json, ItemType expectedItemType)
    {
        // Act
        var itemType = JsonSerializer.Deserialize<ItemType>(json, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal(expectedItemType, itemType);
    }

    [Fact]
    public void ItemType_HasAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            ItemType.Minifig,
            ItemType.Part,
            ItemType.Set,
            ItemType.Book,
            ItemType.Gear,
            ItemType.Catalog,
            ItemType.Instruction,
            ItemType.UnsortedLot,
            ItemType.OriginalBox
        };

        // Act
        var actualValues = Enum.GetValues<ItemType>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    [Theory]
    [InlineData("\"INVALID_TYPE\"")]
    [InlineData("\"minifig\"")]
    [InlineData("\"part\"")]
    public void ItemType_ThrowsExceptionForInvalidJsonValue(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ItemType>(invalidJson, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void ItemType_SerializationRoundTrip_PreservesValues()
    {
        foreach (var itemType in Enum.GetValues<ItemType>())
        {
            // Act
            var json = JsonSerializer.Serialize(itemType, JsonSerializationHelper.DefaultOptions);
            var deserialized = JsonSerializer.Deserialize<ItemType>(json, JsonSerializationHelper.DefaultOptions);

            // Assert
            Assert.Equal(itemType, deserialized);
        }
    }

    [Fact]
    public void ItemType_DeserializesFromNullToken_ThrowsJsonException()
    {
        // Arrange
        var json = "null";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ItemType>(json, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void ItemType_DeserializesFromNumberToken_ThrowsJsonException()
    {
        // Arrange
        var json = "123";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<ItemType>(json, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void ItemType_SerializesInvalidEnumValue_ThrowsJsonException()
    {
        // Arrange - cast an invalid integer to ItemType
        var invalidItemType = (ItemType)999;

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Serialize(invalidItemType, JsonSerializationHelper.DefaultOptions));
    }
}
