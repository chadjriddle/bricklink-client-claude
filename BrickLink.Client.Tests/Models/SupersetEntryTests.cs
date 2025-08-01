using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class SupersetEntryTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public SupersetEntryTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void SupersetEntry_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var supersetEntry = new SupersetEntry();

        // Assert
        Assert.Equal(0, supersetEntry.ColorId);
        Assert.NotNull(supersetEntry.Entries);
        Assert.Empty(supersetEntry.Entries);
    }

    [Fact]
    public void SupersetEntry_PropertiesCanBeSet()
    {
        // Arrange
        var supersetEntry = new SupersetEntry();
        var entries = new List<SupersetItem>
        {
            new SupersetItem
            {
                Item = new InventoryItem { No = "10179", Type = ItemType.Set },
                Quantity = 1,
                AppearAs = AppearAs.Regular
            }
        };

        // Act
        supersetEntry.ColorId = 5;
        supersetEntry.Entries = entries;

        // Assert
        Assert.Equal(5, supersetEntry.ColorId);
        Assert.Equal(entries, supersetEntry.Entries);
        Assert.Single(supersetEntry.Entries);
    }

    [Fact]
    public void SupersetEntry_SerializesToJson()
    {
        // Arrange
        var supersetEntry = new SupersetEntry
        {
            ColorId = 4,
            Entries = new List<SupersetItem>
            {
                new SupersetItem
                {
                    Item = new InventoryItem
                    {
                        No = "10179",
                        Type = ItemType.Set
                    },
                    Quantity = 1,
                    AppearAs = AppearAs.Regular
                },
                new SupersetItem
                {
                    Item = new InventoryItem
                    {
                        No = "8043",
                        Type = ItemType.Set
                    },
                    Quantity = 2,
                    AppearAs = AppearAs.Extra
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(supersetEntry, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("color_id", out var colorIdProperty));
        Assert.Equal(4, colorIdProperty.GetInt32());

        Assert.True(root.TryGetProperty("entries", out var entriesProperty));
        Assert.Equal(JsonValueKind.Array, entriesProperty.ValueKind);
        Assert.Equal(2, entriesProperty.GetArrayLength());

        var firstEntry = entriesProperty[0];
        Assert.True(firstEntry.TryGetProperty("quantity", out var quantityProperty));
        Assert.Equal(1, quantityProperty.GetInt32());
        Assert.True(firstEntry.TryGetProperty("appear_as", out var appearAsProperty));
        Assert.Equal("R", appearAsProperty.GetString());

        var secondEntry = entriesProperty[1];
        Assert.True(secondEntry.TryGetProperty("quantity", out var quantity2Property));
        Assert.Equal(2, quantity2Property.GetInt32());
        Assert.True(secondEntry.TryGetProperty("appear_as", out var appearAs2Property));
        Assert.Equal("E", appearAs2Property.GetString());
    }

    [Fact]
    public void SupersetEntry_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "color_id": 4,
            "entries": [
                {
                    "item": {
                        "no": "10179",
                        "type": "SET"
                    },
                    "quantity": 1,
                    "appear_as": "R"
                },
                {
                    "item": {
                        "no": "8043",
                        "type": "SET"
                    },
                    "quantity": 2,
                    "appear_as": "E"
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SupersetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.ColorId);
        Assert.NotNull(result.Entries);
        Assert.Equal(2, result.Entries.Count);

        var firstEntry = result.Entries[0];
        Assert.Equal("10179", firstEntry.Item.No);
        Assert.Equal(ItemType.Set, firstEntry.Item.Type);
        Assert.Equal(1, firstEntry.Quantity);
        Assert.Equal(AppearAs.Regular, firstEntry.AppearAs);

        var secondEntry = result.Entries[1];
        Assert.Equal("8043", secondEntry.Item.No);
        Assert.Equal(ItemType.Set, secondEntry.Item.Type);
        Assert.Equal(2, secondEntry.Quantity);
        Assert.Equal(AppearAs.Extra, secondEntry.AppearAs);
    }

    [Fact]
    public void SupersetEntry_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalEntry = new SupersetEntry
        {
            ColorId = 86,
            Entries = new List<SupersetItem>
            {
                new SupersetItem
                {
                    Item = new InventoryItem
                    {
                        No = "3001",
                        Type = ItemType.Part
                    },
                    Quantity = 4,
                    AppearAs = AppearAs.Alternate
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(originalEntry, _jsonOptions);
        var deserializedEntry = JsonSerializer.Deserialize<SupersetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedEntry);
        Assert.Equal(originalEntry.ColorId, deserializedEntry.ColorId);
        Assert.Equal(originalEntry.Entries.Count, deserializedEntry.Entries.Count);

        var originalItem = originalEntry.Entries[0];
        var deserializedItem = deserializedEntry.Entries[0];
        Assert.Equal(originalItem.Item.No, deserializedItem.Item.No);
        Assert.Equal(originalItem.Item.Type, deserializedItem.Item.Type);
        Assert.Equal(originalItem.Quantity, deserializedItem.Quantity);
        Assert.Equal(originalItem.AppearAs, deserializedItem.AppearAs);
    }

    [Fact]
    public void SupersetEntry_HandlesEmptyEntriesArray()
    {
        // Arrange
        var json = """
        {
            "color_id": 7,
            "entries": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SupersetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.ColorId);
        Assert.NotNull(result.Entries);
        Assert.Empty(result.Entries);
    }

    [Fact]
    public void SupersetEntry_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "color_id": 0,
            "entries": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SupersetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.ColorId);
        Assert.NotNull(result.Entries);
        Assert.Empty(result.Entries);
    }
}
