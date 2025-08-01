using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class SupersetItemTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public SupersetItemTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void SupersetItem_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var supersetItem = new SupersetItem();

        // Assert
        Assert.NotNull(supersetItem.Item);
        Assert.Equal(0, supersetItem.Quantity);
        Assert.Equal(AppearAs.Alternate, supersetItem.AppearAs);
    }

    [Fact]
    public void SupersetItem_PropertiesCanBeSet()
    {
        // Arrange
        var supersetItem = new SupersetItem();
        var inventoryItem = new InventoryItem
        {
            No = "10179",
            Type = ItemType.Set
        };

        // Act
        supersetItem.Item = inventoryItem;
        supersetItem.Quantity = 2;
        supersetItem.AppearAs = AppearAs.Regular;

        // Assert
        Assert.Equal(inventoryItem, supersetItem.Item);
        Assert.Equal(2, supersetItem.Quantity);
        Assert.Equal(AppearAs.Regular, supersetItem.AppearAs);
    }

    [Fact]
    public void SupersetItem_SerializesToJson()
    {
        // Arrange
        var supersetItem = new SupersetItem
        {
            Item = new InventoryItem
            {
                No = "10179",
                Type = ItemType.Set
            },
            Quantity = 1,
            AppearAs = AppearAs.Regular
        };

        // Act
        var json = JsonSerializer.Serialize(supersetItem, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("item", out var itemProperty));
        Assert.True(itemProperty.TryGetProperty("no", out var noProperty));
        Assert.Equal("10179", noProperty.GetString());

        Assert.True(root.TryGetProperty("quantity", out var quantityProperty));
        Assert.Equal(1, quantityProperty.GetInt32());

        Assert.True(root.TryGetProperty("appear_as", out var appearAsProperty));
        Assert.Equal("R", appearAsProperty.GetString());
    }

    [Fact]
    public void SupersetItem_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "item": {
                "no": "10179",
                "type": "SET"
            },
            "quantity": 1,
            "appear_as": "R"
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SupersetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal("10179", result.Item.No);
        Assert.Equal(ItemType.Set, result.Item.Type);
        Assert.Equal(1, result.Quantity);
        Assert.Equal(AppearAs.Regular, result.AppearAs);
    }

    [Fact]
    public void SupersetItem_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalItem = new SupersetItem
        {
            Item = new InventoryItem
            {
                No = "3001",
                Type = ItemType.Part
            },
            Quantity = 4,
            AppearAs = AppearAs.Extra
        };

        // Act
        var json = JsonSerializer.Serialize(originalItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<SupersetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(originalItem.Item.No, deserializedItem.Item.No);
        Assert.Equal(originalItem.Item.Type, deserializedItem.Item.Type);
        Assert.Equal(originalItem.Quantity, deserializedItem.Quantity);
        Assert.Equal(originalItem.AppearAs, deserializedItem.AppearAs);
    }

    [Theory]
    [InlineData(AppearAs.Alternate, "A")]
    [InlineData(AppearAs.Counterpart, "C")]
    [InlineData(AppearAs.Extra, "E")]
    [InlineData(AppearAs.Regular, "R")]
    public void SupersetItem_SerializesAppearAsCorrectly(AppearAs appearAs, string expectedValue)
    {
        // Arrange
        var supersetItem = new SupersetItem
        {
            AppearAs = appearAs
        };

        // Act
        var json = JsonSerializer.Serialize(supersetItem, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);

        // Assert
        Assert.True(jsonDocument.RootElement.TryGetProperty("appear_as", out var appearAsProperty));
        Assert.Equal(expectedValue, appearAsProperty.GetString());
    }

    [Fact]
    public void SupersetItem_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "item": {},
            "quantity": 0,
            "appear_as": "A"
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SupersetItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Item);
        Assert.Equal(0, result.Quantity);
        Assert.Equal(AppearAs.Alternate, result.AppearAs);
    }
}
