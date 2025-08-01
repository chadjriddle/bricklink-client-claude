using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class SubsetEntryTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public SubsetEntryTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void SubsetEntry_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var subsetEntry = new SubsetEntry();

        // Assert
        Assert.Equal(0, subsetEntry.MatchNo);
        Assert.NotNull(subsetEntry.Entries);
        Assert.Empty(subsetEntry.Entries);
    }

    [Fact]
    public void SubsetEntry_PropertiesCanBeSet()
    {
        // Arrange
        var subsetEntry = new SubsetEntry();
        var entries = new List<SubsetItem>
        {
            new SubsetItem
            {
                Item = new InventoryItem { No = "3001", Type = ItemType.Part },
                ColorId = 4,
                Quantity = 6,
                ExtraQuantity = 0,
                IsAlternate = false
            }
        };

        // Act
        subsetEntry.MatchNo = 1;
        subsetEntry.Entries = entries;

        // Assert
        Assert.Equal(1, subsetEntry.MatchNo);
        Assert.Equal(entries, subsetEntry.Entries);
        Assert.Single(subsetEntry.Entries);
    }

    [Fact]
    public void SubsetEntry_SerializesToJson()
    {
        // Arrange
        var subsetEntry = new SubsetEntry
        {
            MatchNo = 1,
            Entries = new List<SubsetItem>
            {
                new SubsetItem
                {
                    Item = new InventoryItem
                    {
                        No = "3001",
                        Type = ItemType.Part
                    },
                    ColorId = 4,
                    Quantity = 6,
                    ExtraQuantity = 0,
                    IsAlternate = false
                },
                new SubsetItem
                {
                    Item = new InventoryItem
                    {
                        No = "3002",
                        Type = ItemType.Part
                    },
                    ColorId = 4,
                    Quantity = 2,
                    ExtraQuantity = 1,
                    IsAlternate = true
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(subsetEntry, _jsonOptions);
        var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        // Assert
        Assert.True(root.TryGetProperty("match_no", out var matchNoProperty));
        Assert.Equal(1, matchNoProperty.GetInt32());

        Assert.True(root.TryGetProperty("entries", out var entriesProperty));
        Assert.Equal(JsonValueKind.Array, entriesProperty.ValueKind);
        Assert.Equal(2, entriesProperty.GetArrayLength());

        var firstEntry = entriesProperty[0];
        Assert.True(firstEntry.TryGetProperty("color_id", out var colorIdProperty));
        Assert.Equal(4, colorIdProperty.GetInt32());
        Assert.True(firstEntry.TryGetProperty("quantity", out var quantityProperty));
        Assert.Equal(6, quantityProperty.GetInt32());
        Assert.True(firstEntry.TryGetProperty("is_alternate", out var isAlternateProperty));
        Assert.False(isAlternateProperty.GetBoolean());

        var secondEntry = entriesProperty[1];
        Assert.True(secondEntry.TryGetProperty("quantity", out var quantity2Property));
        Assert.Equal(2, quantity2Property.GetInt32());
        Assert.True(secondEntry.TryGetProperty("extra_quantity", out var extraQuantityProperty));
        Assert.Equal(1, extraQuantityProperty.GetInt32());
        Assert.True(secondEntry.TryGetProperty("is_alternate", out var isAlternate2Property));
        Assert.True(isAlternate2Property.GetBoolean());
    }

    [Fact]
    public void SubsetEntry_DeserializesFromJson()
    {
        // Arrange
        var json = """
        {
            "match_no": 1,
            "entries": [
                {
                    "item": {
                        "no": "3001",
                        "type": "PART"
                    },
                    "color_id": 4,
                    "quantity": 6,
                    "extra_quantity": 0,
                    "is_alternate": false
                },
                {
                    "item": {
                        "no": "3002",
                        "type": "PART"
                    },
                    "color_id": 4,
                    "quantity": 2,
                    "extra_quantity": 1,
                    "is_alternate": true
                }
            ]
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SubsetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.MatchNo);
        Assert.NotNull(result.Entries);
        Assert.Equal(2, result.Entries.Count);

        var firstEntry = result.Entries[0];
        Assert.Equal("3001", firstEntry.Item.No);
        Assert.Equal(ItemType.Part, firstEntry.Item.Type);
        Assert.Equal(4, firstEntry.ColorId);
        Assert.Equal(6, firstEntry.Quantity);
        Assert.Equal(0, firstEntry.ExtraQuantity);
        Assert.False(firstEntry.IsAlternate);

        var secondEntry = result.Entries[1];
        Assert.Equal("3002", secondEntry.Item.No);
        Assert.Equal(ItemType.Part, secondEntry.Item.Type);
        Assert.Equal(4, secondEntry.ColorId);
        Assert.Equal(2, secondEntry.Quantity);
        Assert.Equal(1, secondEntry.ExtraQuantity);
        Assert.True(secondEntry.IsAlternate);
    }

    [Fact]
    public void SubsetEntry_JsonRoundTrip_PreservesData()
    {
        // Arrange
        var originalEntry = new SubsetEntry
        {
            MatchNo = 99,
            Entries = new List<SubsetItem>
            {
                new SubsetItem
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
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(originalEntry, _jsonOptions);
        var deserializedEntry = JsonSerializer.Deserialize<SubsetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedEntry);
        Assert.Equal(originalEntry.MatchNo, deserializedEntry.MatchNo);
        Assert.Equal(originalEntry.Entries.Count, deserializedEntry.Entries.Count);

        var originalItem = originalEntry.Entries[0];
        var deserializedItem = deserializedEntry.Entries[0];
        Assert.Equal(originalItem.Item.No, deserializedItem.Item.No);
        Assert.Equal(originalItem.Item.Type, deserializedItem.Item.Type);
        Assert.Equal(originalItem.ColorId, deserializedItem.ColorId);
        Assert.Equal(originalItem.Quantity, deserializedItem.Quantity);
        Assert.Equal(originalItem.ExtraQuantity, deserializedItem.ExtraQuantity);
        Assert.Equal(originalItem.IsAlternate, deserializedItem.IsAlternate);
    }

    [Fact]
    public void SubsetEntry_HandlesEmptyEntriesArray()
    {
        // Arrange
        var json = """
        {
            "match_no": 42,
            "entries": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SubsetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.MatchNo);
        Assert.NotNull(result.Entries);
        Assert.Empty(result.Entries);
    }

    [Fact]
    public void SubsetEntry_HandlesLargeMatchNo()
    {
        // Arrange
        var subsetEntry = new SubsetEntry
        {
            MatchNo = int.MaxValue,
            Entries = new List<SubsetItem>()
        };

        // Act
        var json = JsonSerializer.Serialize(subsetEntry, _jsonOptions);
        var result = JsonSerializer.Deserialize<SubsetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(int.MaxValue, result.MatchNo);
        Assert.NotNull(result.Entries);
        Assert.Empty(result.Entries);
    }

    [Fact]
    public void SubsetEntry_HandlesMinimalJsonStructure()
    {
        // Arrange
        var json = """
        {
            "match_no": 0,
            "entries": []
        }
        """;

        // Act
        var result = JsonSerializer.Deserialize<SubsetEntry>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.MatchNo);
        Assert.NotNull(result.Entries);
        Assert.Empty(result.Entries);
    }
}
