using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class InventoryItemTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public InventoryItemTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void InventoryItem_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var inventoryItem = new InventoryItem();

        // Assert
        Assert.Equal(string.Empty, inventoryItem.No);
        Assert.Equal(string.Empty, inventoryItem.Name);
        Assert.Equal(ItemType.Minifig, inventoryItem.Type);
        Assert.Equal(0, inventoryItem.CategoryId);
    }

    [Fact]
    public void InventoryItem_PropertiesCanBeSet()
    {
        // Arrange
        var inventoryItem = new InventoryItem();

        // Act
        inventoryItem.No = "3001";
        inventoryItem.Name = "Brick 2 x 4";
        inventoryItem.Type = ItemType.Part;
        inventoryItem.CategoryId = 5;

        // Assert
        Assert.Equal("3001", inventoryItem.No);
        Assert.Equal("Brick 2 x 4", inventoryItem.Name);
        Assert.Equal(ItemType.Part, inventoryItem.Type);
        Assert.Equal(5, inventoryItem.CategoryId);
    }

    [Fact]
    public void InventoryItem_SerializesToJson_WithCorrectPropertyNames()
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            No = "3001",
            Name = "Brick 2 x 4",
            Type = ItemType.Part,
            CategoryId = 5
        };

        // Act
        var json = JsonSerializer.Serialize(inventoryItem, _jsonOptions);

        // Assert
        Assert.Contains("\"no\":\"3001\"", json);
        Assert.Contains("\"name\":\"Brick 2 x 4\"", json);
        Assert.Contains("\"type\":\"PART\"", json);
        Assert.Contains("\"category_id\":5", json);
    }

    [Fact]
    public void InventoryItem_DeserializesFromJson_WithCorrectPropertyNames()
    {
        // Arrange
        var json = """
        {
            "no": "3001",
            "name": "Brick 2 x 4",
            "type": "PART",
            "category_id": 5
        }
        """;

        // Act
        var inventoryItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(inventoryItem);
        Assert.Equal("3001", inventoryItem.No);
        Assert.Equal("Brick 2 x 4", inventoryItem.Name);
        Assert.Equal(ItemType.Part, inventoryItem.Type);
        Assert.Equal(5, inventoryItem.CategoryId);
    }

    [Theory]
    [InlineData(ItemType.Part)]
    [InlineData(ItemType.Set)]
    [InlineData(ItemType.Minifig)]
    [InlineData(ItemType.Book)]
    [InlineData(ItemType.Gear)]
    [InlineData(ItemType.Catalog)]
    [InlineData(ItemType.Instruction)]
    [InlineData(ItemType.UnsortedLot)]
    [InlineData(ItemType.OriginalBox)]
    public void InventoryItem_SerializesAndDeserializesItemType_Correctly(ItemType itemType)
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            No = "test",
            Name = "Test Item",
            Type = itemType,
            CategoryId = 1
        };

        // Act
        var json = JsonSerializer.Serialize(inventoryItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(itemType, deserializedItem.Type);
    }

    [Fact]
    public void InventoryItem_HandlesEmptyStrings_Correctly()
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            No = "",
            Name = "",
            Type = ItemType.Part,
            CategoryId = 0
        };

        // Act
        var json = JsonSerializer.Serialize(inventoryItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal("", deserializedItem.No);
        Assert.Equal("", deserializedItem.Name);
        Assert.Equal(ItemType.Part, deserializedItem.Type);
        Assert.Equal(0, deserializedItem.CategoryId);
    }

    [Fact]
    public void InventoryItem_HandlesSpecialCharacters_InStrings()
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            No = "test-123/abc",
            Name = "Test Item (Special) & More",
            Type = ItemType.Set,
            CategoryId = 999
        };

        // Act
        var json = JsonSerializer.Serialize(inventoryItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal("test-123/abc", deserializedItem.No);
        Assert.Equal("Test Item (Special) & More", deserializedItem.Name);
        Assert.Equal(ItemType.Set, deserializedItem.Type);
        Assert.Equal(999, deserializedItem.CategoryId);
    }

    [Fact]
    public void InventoryItem_HandlesLongStrings_Correctly()
    {
        // Arrange
        var longNo = new string('X', 100);
        var longName = new string('Y', 500);
        var inventoryItem = new InventoryItem
        {
            No = longNo,
            Name = longName,
            Type = ItemType.Minifig,
            CategoryId = int.MaxValue
        };

        // Act
        var json = JsonSerializer.Serialize(inventoryItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(longNo, deserializedItem.No);
        Assert.Equal(longName, deserializedItem.Name);
        Assert.Equal(ItemType.Minifig, deserializedItem.Type);
        Assert.Equal(int.MaxValue, deserializedItem.CategoryId);
    }

    [Fact]
    public void InventoryItem_RoundTripSerialization_PreservesAllData()
    {
        // Arrange
        var originalItem = new InventoryItem
        {
            No = "sw0001",
            Name = "Luke Skywalker (Tatooine)",
            Type = ItemType.Minifig,
            CategoryId = 273
        };

        // Act
        var json = JsonSerializer.Serialize(originalItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(originalItem.No, deserializedItem.No);
        Assert.Equal(originalItem.Name, deserializedItem.Name);
        Assert.Equal(originalItem.Type, deserializedItem.Type);
        Assert.Equal(originalItem.CategoryId, deserializedItem.CategoryId);
    }

    [Fact]
    public void InventoryItem_DeserializesFromJson_WithMissingOptionalFields()
    {
        // Arrange - minimal JSON with only required fields
        var json = """
        {
            "no": "3001",
            "type": "PART"
        }
        """;

        // Act
        var inventoryItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(inventoryItem);
        Assert.Equal("3001", inventoryItem.No);
        Assert.Equal(string.Empty, inventoryItem.Name); // Should default to empty string
        Assert.Equal(ItemType.Part, inventoryItem.Type);
        Assert.Equal(0, inventoryItem.CategoryId); // Should default to 0
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999999)]
    public void InventoryItem_HandlesCategoryIdValues_Correctly(int categoryId)
    {
        // Arrange
        var inventoryItem = new InventoryItem
        {
            No = "test",
            Name = "Test Item",
            Type = ItemType.Part,
            CategoryId = categoryId
        };

        // Act
        var json = JsonSerializer.Serialize(inventoryItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<InventoryItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(categoryId, deserializedItem.CategoryId);
    }
}