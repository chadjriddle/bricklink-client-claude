using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class CatalogItemTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public CatalogItemTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void CatalogItem_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var catalogItem = new CatalogItem();

        // Assert
        Assert.Equal(string.Empty, catalogItem.No);
        Assert.Equal(string.Empty, catalogItem.Name);
        Assert.Equal(ItemType.Minifig, catalogItem.Type);
        Assert.Equal(0, catalogItem.CategoryId);
        Assert.Null(catalogItem.AlternateNo);
        Assert.Null(catalogItem.ImageUrl);
        Assert.Null(catalogItem.ThumbnailUrl);
        Assert.Equal(0m, catalogItem.Weight);
        Assert.Equal(0m, catalogItem.DimX);
        Assert.Equal(0m, catalogItem.DimY);
        Assert.Equal(0m, catalogItem.DimZ);
        Assert.Equal(0, catalogItem.YearReleased);
        Assert.Null(catalogItem.Description);
        Assert.False(catalogItem.IsObsolete);
        Assert.Null(catalogItem.LanguageCode);
    }

    [Fact]
    public void CatalogItem_PropertiesCanBeSet()
    {
        // Arrange
        var catalogItem = new CatalogItem();

        // Act
        catalogItem.No = "3001";
        catalogItem.Name = "Brick 2 x 4";
        catalogItem.Type = ItemType.Part;
        catalogItem.CategoryId = 5;
        catalogItem.AlternateNo = "3001b";
        catalogItem.ImageUrl = "https://img.bricklink.com/ItemImage/PN/11/3001.png";
        catalogItem.ThumbnailUrl = "https://img.bricklink.com/ItemImage/TN/11/3001.png";
        catalogItem.Weight = 2.54m;
        catalogItem.DimX = 32.0m;
        catalogItem.DimY = 16.0m;
        catalogItem.DimZ = 9.6m;
        catalogItem.YearReleased = 1958;
        catalogItem.Description = "Basic building brick";
        catalogItem.IsObsolete = false;
        catalogItem.LanguageCode = "en";

        // Assert
        Assert.Equal("3001", catalogItem.No);
        Assert.Equal("Brick 2 x 4", catalogItem.Name);
        Assert.Equal(ItemType.Part, catalogItem.Type);
        Assert.Equal(5, catalogItem.CategoryId);
        Assert.Equal("3001b", catalogItem.AlternateNo);
        Assert.Equal("https://img.bricklink.com/ItemImage/PN/11/3001.png", catalogItem.ImageUrl);
        Assert.Equal("https://img.bricklink.com/ItemImage/TN/11/3001.png", catalogItem.ThumbnailUrl);
        Assert.Equal(2.54m, catalogItem.Weight);
        Assert.Equal(32.0m, catalogItem.DimX);
        Assert.Equal(16.0m, catalogItem.DimY);
        Assert.Equal(9.6m, catalogItem.DimZ);
        Assert.Equal(1958, catalogItem.YearReleased);
        Assert.Equal("Basic building brick", catalogItem.Description);
        Assert.False(catalogItem.IsObsolete);
        Assert.Equal("en", catalogItem.LanguageCode);
    }

    [Fact]
    public void CatalogItem_SerializesToJson_WithCorrectPropertyNames()
    {
        // Arrange
        var catalogItem = new CatalogItem
        {
            No = "3001",
            Name = "Brick 2 x 4",
            Type = ItemType.Part,
            CategoryId = 5,
            AlternateNo = "3001b",
            ImageUrl = "https://img.bricklink.com/ItemImage/PN/11/3001.png",
            ThumbnailUrl = "https://img.bricklink.com/ItemImage/TN/11/3001.png",
            Weight = 2.54m,
            DimX = 32.0m,
            DimY = 16.0m,
            DimZ = 9.6m,
            YearReleased = 1958,
            Description = "Basic building brick",
            IsObsolete = false,
            LanguageCode = "en"
        };

        // Act
        var json = JsonSerializer.Serialize(catalogItem, _jsonOptions);

        // Assert
        Assert.Contains("\"no\":\"3001\"", json);
        Assert.Contains("\"name\":\"Brick 2 x 4\"", json);
        Assert.Contains("\"type\":\"PART\"", json);
        Assert.Contains("\"category_id\":5", json);
        Assert.Contains("\"alternate_no\":\"3001b\"", json);
        Assert.Contains("\"image_url\":\"https://img.bricklink.com/ItemImage/PN/11/3001.png\"", json);
        Assert.Contains("\"thumbnail_url\":\"https://img.bricklink.com/ItemImage/TN/11/3001.png\"", json);
        Assert.Contains("\"weight\":2.54", json);
        Assert.Contains("\"dim_x\":32.0", json);
        Assert.Contains("\"dim_y\":16.0", json);
        Assert.Contains("\"dim_z\":9.6", json);
        Assert.Contains("\"year_released\":1958", json);
        Assert.Contains("\"description\":\"Basic building brick\"", json);
        Assert.Contains("\"is_obsolete\":false", json);
        Assert.Contains("\"language_code\":\"en\"", json);
    }

    [Fact]
    public void CatalogItem_DeserializesFromJson_WithCorrectPropertyNames()
    {
        // Arrange
        var json = """
        {
            "no": "3001",
            "name": "Brick 2 x 4",
            "type": "PART",
            "category_id": 5,
            "alternate_no": "3001b",
            "image_url": "https://img.bricklink.com/ItemImage/PN/11/3001.png",
            "thumbnail_url": "https://img.bricklink.com/ItemImage/TN/11/3001.png",
            "weight": 2.54,
            "dim_x": 32.0,
            "dim_y": 16.0,
            "dim_z": 9.6,
            "year_released": 1958,
            "description": "Basic building brick",
            "is_obsolete": false,
            "language_code": "en"
        }
        """;

        // Act
        var catalogItem = JsonSerializer.Deserialize<CatalogItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(catalogItem);
        Assert.Equal("3001", catalogItem.No);
        Assert.Equal("Brick 2 x 4", catalogItem.Name);
        Assert.Equal(ItemType.Part, catalogItem.Type);
        Assert.Equal(5, catalogItem.CategoryId);
        Assert.Equal("3001b", catalogItem.AlternateNo);
        Assert.Equal("https://img.bricklink.com/ItemImage/PN/11/3001.png", catalogItem.ImageUrl);
        Assert.Equal("https://img.bricklink.com/ItemImage/TN/11/3001.png", catalogItem.ThumbnailUrl);
        Assert.Equal(2.54m, catalogItem.Weight);
        Assert.Equal(32.0m, catalogItem.DimX);
        Assert.Equal(16.0m, catalogItem.DimY);
        Assert.Equal(9.6m, catalogItem.DimZ);
        Assert.Equal(1958, catalogItem.YearReleased);
        Assert.Equal("Basic building brick", catalogItem.Description);
        Assert.False(catalogItem.IsObsolete);
        Assert.Equal("en", catalogItem.LanguageCode);
    }

    [Fact]
    public void CatalogItem_DeserializesFromJson_WithNullOptionalFields()
    {
        // Arrange
        var json = """
        {
            "no": "3001",
            "name": "Brick 2 x 4",
            "type": "PART",
            "category_id": 5,
            "weight": 2.54,
            "dim_x": 32.0,
            "dim_y": 16.0,
            "dim_z": 9.6,
            "year_released": 1958,
            "is_obsolete": false
        }
        """;

        // Act
        var catalogItem = JsonSerializer.Deserialize<CatalogItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(catalogItem);
        Assert.Equal("3001", catalogItem.No);
        Assert.Equal("Brick 2 x 4", catalogItem.Name);
        Assert.Equal(ItemType.Part, catalogItem.Type);
        Assert.Equal(5, catalogItem.CategoryId);
        Assert.Null(catalogItem.AlternateNo);
        Assert.Null(catalogItem.ImageUrl);
        Assert.Null(catalogItem.ThumbnailUrl);
        Assert.Equal(2.54m, catalogItem.Weight);
        Assert.Equal(32.0m, catalogItem.DimX);
        Assert.Equal(16.0m, catalogItem.DimY);
        Assert.Equal(9.6m, catalogItem.DimZ);
        Assert.Equal(1958, catalogItem.YearReleased);
        Assert.Null(catalogItem.Description);
        Assert.False(catalogItem.IsObsolete);
        Assert.Null(catalogItem.LanguageCode);
    }

    [Theory]
    [InlineData(ItemType.Part)]
    [InlineData(ItemType.Set)]
    [InlineData(ItemType.Minifig)]
    [InlineData(ItemType.Book)]
    [InlineData(ItemType.Gear)]
    public void CatalogItem_SerializesAndDeserializesItemType_Correctly(ItemType itemType)
    {
        // Arrange
        var catalogItem = new CatalogItem
        {
            No = "test",
            Name = "Test Item",
            Type = itemType,
            CategoryId = 1
        };

        // Act
        var json = JsonSerializer.Serialize(catalogItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<CatalogItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(itemType, deserializedItem.Type);
    }

    [Fact]
    public void CatalogItem_HandlesLargeDecimalValues_Correctly()
    {
        // Arrange
        var catalogItem = new CatalogItem
        {
            No = "test",
            Name = "Test Item",
            Weight = 999999.9999m,
            DimX = 1000.5678m,
            DimY = 2000.1234m,
            DimZ = 3000.9876m
        };

        // Act
        var json = JsonSerializer.Serialize(catalogItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<CatalogItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(999999.9999m, deserializedItem.Weight);
        Assert.Equal(1000.5678m, deserializedItem.DimX);
        Assert.Equal(2000.1234m, deserializedItem.DimY);
        Assert.Equal(3000.9876m, deserializedItem.DimZ);
    }

    [Fact]
    public void CatalogItem_RoundTripSerialization_PreservesAllData()
    {
        // Arrange
        var originalItem = new CatalogItem
        {
            No = "sw0001",
            Name = "Luke Skywalker (Tatooine)",
            Type = ItemType.Minifig,
            CategoryId = 273,
            AlternateNo = "sw001",
            ImageUrl = "https://img.bricklink.com/ItemImage/MN/0/sw0001.png",
            ThumbnailUrl = "https://img.bricklink.com/ItemImage/TN/0/sw0001.png",
            Weight = 4.0m,
            DimX = 0.0m,
            DimY = 0.0m,
            DimZ = 0.0m,
            YearReleased = 1999,
            Description = "Luke Skywalker minifigure from Tatooine",
            IsObsolete = true,
            LanguageCode = null
        };

        // Act
        var json = JsonSerializer.Serialize(originalItem, _jsonOptions);
        var deserializedItem = JsonSerializer.Deserialize<CatalogItem>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedItem);
        Assert.Equal(originalItem.No, deserializedItem.No);
        Assert.Equal(originalItem.Name, deserializedItem.Name);
        Assert.Equal(originalItem.Type, deserializedItem.Type);
        Assert.Equal(originalItem.CategoryId, deserializedItem.CategoryId);
        Assert.Equal(originalItem.AlternateNo, deserializedItem.AlternateNo);
        Assert.Equal(originalItem.ImageUrl, deserializedItem.ImageUrl);
        Assert.Equal(originalItem.ThumbnailUrl, deserializedItem.ThumbnailUrl);
        Assert.Equal(originalItem.Weight, deserializedItem.Weight);
        Assert.Equal(originalItem.DimX, deserializedItem.DimX);
        Assert.Equal(originalItem.DimY, deserializedItem.DimY);
        Assert.Equal(originalItem.DimZ, deserializedItem.DimZ);
        Assert.Equal(originalItem.YearReleased, deserializedItem.YearReleased);
        Assert.Equal(originalItem.Description, deserializedItem.Description);
        Assert.Equal(originalItem.IsObsolete, deserializedItem.IsObsolete);
        Assert.Equal(originalItem.LanguageCode, deserializedItem.LanguageCode);
    }
}
