using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class SubsetItemTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public SubsetItemTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void SubsetItem_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var subsetItem = new SubsetItem();

        // Assert
        Assert.NotNull(subsetItem.Item);
        Assert.Equal(0, subsetItem.ColorId);
        Assert.Equal(0, subsetItem.Quantity);
        Assert.Equal(0, subsetItem.ExtraQuantity);
        Assert.False(subsetItem.IsAlternate);
    }

    [Fact]
    public void SubsetItem_PropertiesCanBeSet()
    {
        // Arrange
        var subsetItem = new SubsetItem();
        var inventoryItem = new InventoryItem
        {
            No = "3001",
            Type = ItemType.Part
        };

        // Act
        subsetItem.Item = inventoryItem;
        subsetItem.ColorId = 4;
        subsetItem.Quantity = 6;
        subsetItem.ExtraQuantity = 2;
        subsetItem.IsAlternate = true;

        // Assert
        Assert.Equal(inventoryItem, subsetItem.Item);
        Assert.Equal(4, subsetItem.ColorId);
        Assert.Equal(6, subsetItem.Quantity);
        Assert.Equal(2, subsetItem.ExtraQuantity);
        Assert.True(subsetItem.IsAlternate);
    }

    [Fact]
    public void SubsetItem_SerializesToJson()
    {
        // Arrange
        var subsetItem = new SubsetItem
        {
            Item = new InventoryItem
            {
                No = "3001",
                Type = ItemType.Part
            },
            ColorId = 4,
            Quantity = 6,
            ExtraQuantity = 2,
            IsAlternate = true
        };

        // Act
        var json = JsonSerializer.Serialize(subsetItem, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("item", out var itemProperty));
        Assert.True(itemProperty.TryGetProperty("no", out var noProperty));
        Assert.Equal("3001", noProperty.GetString());

        Assert.True(root.TryGetProperty("color_id", out var colorIdProperty));
        Assert.Equal(4, colorIdProperty.GetInt32());

        Assert.True(root.TryGetProperty("quantity", out var quantityProperty));
        Assert.Equal(6, quantityProperty.GetInt32());

        Assert.True(root.TryGetProperty("extra_quantity", out var extraQuantityProperty));
        Assert.Equal(2, extraQuantityProperty.GetInt32());

        Assert.True(root.TryGetProperty("is_alternate", out var isAlternateProperty));
        Assert.True(isAlternateProperty.GetBoolean());
    }

    [Fact]
    public void SubsetItem_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3001",
                "type": "PART"
            },
            "color_id": 4,
            "quantity": 6,
            "extra_quantity": 2,
            "is_alternate": true
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SubsetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal("3001", result.Item.No);
        Assert.Equal(ItemType.Part, result.Item.Type);
        Assert.Equal(4, result.ColorId);
        Assert.Equal(6, result.Quantity);
        Assert.Equal(2, result.ExtraQuantity);
        Assert.True(result.IsAlternate);
    }

    [Fact]
    public void SubsetItem_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalItem = new SubsetItem
        {
            Item = new InventoryItem
            {
                No = "2456",
                Type = ItemType.Part
            },
            ColorId = 86,
            Quantity = 1,
            ExtraQuantity = 0,
            IsAlternate = false
        };

        // Act
        var json = JsonSerializer.Serialize(originalItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<SubsetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(originalItem.Item.No, deserializedItem.Item.No);
        Assert.Equal(originalItem.Item.Type, deserializedItem.Item.Type);
        Assert.Equal(originalItem.ColorId, deserializedItem.ColorId);
        Assert.Equal(originalItem.Quantity, deserializedItem.Quantity);
        Assert.Equal(originalItem.ExtraQuantity, deserializedItem.ExtraQuantity);
        Assert.Equal(originalItem.IsAlternate, deserializedItem.IsAlternate);
    }

    [Fact]
    public void SubsetItem_HandlesZeroQuantities()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "3005",
                "type": "PART"
            },
            "color_id": 1,
            "quantity": 0,
            "extra_quantity": 0,
            "is_alternate": false
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SubsetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("3005", result.Item.No);
        Assert.Equal(1, result.ColorId);
        Assert.Equal(0, result.Quantity);
        Assert.Equal(0, result.ExtraQuantity);
        Assert.False(result.IsAlternate);
    }

    [Fact]
    public void SubsetItem_HandlesLargeQuantities()
    {
        // Arrange
        var subsetItem = new SubsetItem
        {
            Item = new InventoryItem
            {
                No = "3024",
                Type = ItemType.Part
            },
            ColorId = 5,
            Quantity = int.MaxValue,
            ExtraQuantity = 999999,
            IsAlternate = true
        };

        // Act
        var json = JsonSerializer.Serialize(subsetItem, _jsonOptions);
        var result = JsonSerializer.Deserialize<SubsetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(int.MaxValue, result.Quantity);
        Assert.Equal(999999, result.ExtraQuantity);
        Assert.True(result.IsAlternate);
    }

    [Fact]
    public void SubsetItem_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "item": {},
            "color_id": 0,
            "quantity": 0,
            "extra_quantity": 0,
            "is_alternate": false
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SubsetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal(0, result.ColorId);
        Assert.Equal(0, result.Quantity);
        Assert.Equal(0, result.ExtraQuantity);
        Assert.False(result.IsAlternate);
    }
}
