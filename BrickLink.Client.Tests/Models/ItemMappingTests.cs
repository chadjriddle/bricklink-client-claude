using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class ItemMappingTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public ItemMappingTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void ItemMapping_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var itemMapping = new ItemMapping();

        // Assert
        Assert.NotNull(itemMapping.Item);
        Assert.Equal(0, itemMapping.ColorId);
        Assert.Equal(string.Empty, itemMapping.ElementId);
    }

    [Fact]
    public void ItemMapping_PropertiesCanBeSet()
    {
        // Arrange
        var itemMapping = new ItemMapping();
        var inventoryItem = new InventoryItem
        {
            No = "3001",
            Type = ItemType.Part
        };

        // Act
        itemMapping.Item = inventoryItem;
        itemMapping.ColorId = 4;
        itemMapping.ElementId = "300124";

        // Assert
        Assert.Equal(inventoryItem, itemMapping.Item);
        Assert.Equal(4, itemMapping.ColorId);
        Assert.Equal("300124", itemMapping.ElementId);
    }

    [Fact]
    public void ItemMapping_SerializesToJson()
    {
        // Arrange
        var itemMapping = new ItemMapping
        {
            Item = new InventoryItem
            {
                No = "3001",
                Type = ItemType.Part
            },
            ColorId = 4,
            ElementId = "300124"
        };

        // Act
        var json = JsonSerializer.Serialize(itemMapping, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("item", out var itemProperty));
        Assert.True(itemProperty.TryGetProperty("no", out var noProperty));
        Assert.Equal("3001", noProperty.GetString());
        Assert.True(itemProperty.TryGetProperty("type", out var typeProperty));
        Assert.Equal("PART", typeProperty.GetString());

        Assert.True(root.TryGetProperty("color_id", out var colorIdProperty));
        Assert.Equal(4, colorIdProperty.GetInt32());

        Assert.True(root.TryGetProperty("element_id", out var elementIdProperty));
        Assert.Equal("300124", elementIdProperty.GetString());
    }

    [Fact]
    public void ItemMapping_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3001",
                "type": "PART"
            },
            "color_id": 4,
            "element_id": "300124"
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal("3001", result.Item.No);
        Assert.Equal(ItemType.Part, result.Item.Type);
        Assert.Equal(4, result.ColorId);
        Assert.Equal("300124", result.ElementId);
    }

    [Fact]
    public void ItemMapping_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalMapping = new ItemMapping
        {
            Item = new InventoryItem
            {
                No = "2456",
                Type = ItemType.Part
            },
            ColorId = 86,
            ElementId = "245686"
        };

        // Act
        var json = JsonSerializer.Serialize(originalMapping, _jsonOptions);
        var deserializedMapping = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedMapping);
        Assert.Equal(originalMapping.Item.No, deserializedMapping.Item.No);
        Assert.Equal(originalMapping.Item.Type, deserializedMapping.Item.Type);
        Assert.Equal(originalMapping.ColorId, deserializedMapping.ColorId);
        Assert.Equal(originalMapping.ElementId, deserializedMapping.ElementId);
    }

    [Fact]
    public void ItemMapping_HandlesEmptyElementId()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3024",
                "type": "PART"
            },
            "color_id": 1,
            "element_id": ""
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("3024", result.Item.No);
        Assert.Equal(ItemType.Part, result.Item.Type);
        Assert.Equal(1, result.ColorId);
        Assert.Equal(string.Empty, result.ElementId);
    }

    [Fact]
    public void ItemMapping_HandlesLongElementId()
    {
        // Arrange
        var longElementId = "123456789012345678901234567890";
        var itemMapping = new ItemMapping
        {
            Item = new InventoryItem
            {
                No = "12345",
                Type = ItemType.Part
            },
            ColorId = 999,
            ElementId = longElementId
        };

        // Act
        var json = JsonSerializer.Serialize(itemMapping, _jsonOptions);
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(longElementId, result.ElementId);
        Assert.Equal(999, result.ColorId);
    }

    [Fact]
    public void ItemMapping_HandlesZeroColorId()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3005",
                "type": "PART"
            },
            "color_id": 0,
            "element_id": "300500"
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("3005", result.Item.No);
        Assert.Equal(0, result.ColorId);
        Assert.Equal("300500", result.ElementId);
    }

    [Fact]
    public void ItemMapping_HandlesLargeColorId()
    {
        // Arrange
        var itemMapping = new ItemMapping
        {
            Item = new InventoryItem
            {
                No = "6141",
                Type = ItemType.Part
            },
            ColorId = int.MaxValue,
            ElementId = "614199999"
        };

        // Act
        var json = JsonSerializer.Serialize(itemMapping, _jsonOptions);
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(int.MaxValue, result.ColorId);
        Assert.Equal("614199999", result.ElementId);
    }

    [Fact]
    public void ItemMapping_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "item": {},
            "color_id": 0,
            "element_id": ""
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal(0, result.ColorId);
        Assert.Equal(string.Empty, result.ElementId);
    }

    [Fact]
    public void ItemMapping_WorksWithDifferentItemTypes()
    {
        // Arrange
        var itemMapping = new ItemMapping
        {
            Item = new InventoryItem
            {
                No = "sw0001",
                Type = ItemType.Minifig
            },
            ColorId = 0,
            ElementId = "SW0001"
        };

        // Act
        var json = JsonSerializer.Serialize(itemMapping, _jsonOptions);
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("sw0001", result.Item.No);
        Assert.Equal(ItemType.Minifig, result.Item.Type);
        Assert.Equal("SW0001", result.ElementId);
    }

    [Theory]
    [InlineData("300124")]
    [InlineData("4114332")]
    [InlineData("6007973")]
    [InlineData("614126")]
    public void ItemMapping_HandlesVariousElementIdFormats(string elementId)
    {
        // Arrange
        var itemMapping = new ItemMapping
        {
            Item = new InventoryItem
            {
                No = "3001",
                Type = ItemType.Part
            },
            ColorId = 4,
            ElementId = elementId
        };

        // Act
        var json = JsonSerializer.Serialize(itemMapping, _jsonOptions);
        var result = JsonSerializer.Deserialize<ItemMapping>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(elementId, result.ElementId);
    }
}
