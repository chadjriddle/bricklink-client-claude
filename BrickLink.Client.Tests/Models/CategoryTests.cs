using System.Text.Json;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class CategoryTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public CategoryTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void Category_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        Assert.Equal(0, category.CategoryId);
        Assert.Equal(string.Empty, category.CategoryName);
        Assert.Equal(0, category.ParentId);
    }

    [Fact]
    public void Category_PropertiesCanBeSet()
    {
        // Arrange
        var category = new Category();

        // Act
        category.CategoryId = 5;
        category.CategoryName = "Bricks";
        category.ParentId = 1;

        // Assert
        Assert.Equal(5, category.CategoryId);
        Assert.Equal("Bricks", category.CategoryName);
        Assert.Equal(1, category.ParentId);
    }

    [Fact]
    public void Category_SerializesToJson_WithCorrectPropertyNames()
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 5,
            CategoryName = "Bricks",
            ParentId = 1
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);

        // Assert
        Assert.Contains("\"category_id\":5", json);
        Assert.Contains("\"category_name\":\"Bricks\"", json);
        Assert.Contains("\"parent_id\":1", json);
    }

    [Fact]
    public void Category_DeserializesFromJson_WithCorrectPropertyNames()
    {
        // Arrange
        var json = """
        {
            "category_id": 5,
            "category_name": "Bricks",
            "parent_id": 1
        }
        """;

        // Act
        var category = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(5, category.CategoryId);
        Assert.Equal("Bricks", category.CategoryName);
        Assert.Equal(1, category.ParentId);
    }

    [Theory]
    [InlineData(1, "Bricks", 0)]
    [InlineData(5, "Bricks", 1)]
    [InlineData(273, "Minifigures", 0)]
    [InlineData(11, "Baseplates", 1)]
    [InlineData(896, "Books", 0)]
    public void Category_SerializesAndDeserializesVariousCategories_Correctly(int categoryId, string categoryName, int parentId)
    {
        // Arrange
        var originalCategory = new Category
        {
            CategoryId = categoryId,
            CategoryName = categoryName,
            ParentId = parentId
        };

        // Act
        var json = JsonSerializer.Serialize(originalCategory, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(categoryId, deserializedCategory.CategoryId);
        Assert.Equal(categoryName, deserializedCategory.CategoryName);
        Assert.Equal(parentId, deserializedCategory.ParentId);
    }

    [Fact]
    public void Category_HandlesRootCategory_WithParentIdZero()
    {
        // Arrange
        var rootCategory = new Category
        {
            CategoryId = 1,
            CategoryName = "Root Category",
            ParentId = 0 // Root category has parent_id of 0
        };

        // Act
        var json = JsonSerializer.Serialize(rootCategory, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(1, deserializedCategory.CategoryId);
        Assert.Equal("Root Category", deserializedCategory.CategoryName);
        Assert.Equal(0, deserializedCategory.ParentId);
    }

    [Fact]
    public void Category_HandlesEmptyStrings_Correctly()
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 1,
            CategoryName = "",
            ParentId = 0
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(1, deserializedCategory.CategoryId);
        Assert.Equal("", deserializedCategory.CategoryName);
        Assert.Equal(0, deserializedCategory.ParentId);
    }

    [Fact]
    public void Category_HandlesSpecialCharacters_InCategoryName()
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 999,
            CategoryName = "Category (Test) & More",
            ParentId = 1
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(999, deserializedCategory.CategoryId);
        Assert.Equal("Category (Test) & More", deserializedCategory.CategoryName);
        Assert.Equal(1, deserializedCategory.ParentId);
    }

    [Fact]
    public void Category_HandlesLongCategoryName_Correctly()
    {
        // Arrange
        var longCategoryName = new string('A', 500);
        var category = new Category
        {
            CategoryId = int.MaxValue,
            CategoryName = longCategoryName,
            ParentId = int.MaxValue
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(int.MaxValue, deserializedCategory.CategoryId);
        Assert.Equal(longCategoryName, deserializedCategory.CategoryName);
        Assert.Equal(int.MaxValue, deserializedCategory.ParentId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999999)]
    public void Category_HandlesCategoryIdValues_Correctly(int categoryId)
    {
        // Arrange
        var category = new Category
        {
            CategoryId = categoryId,
            CategoryName = "Test Category",
            ParentId = 0
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(categoryId, deserializedCategory.CategoryId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999999)]
    public void Category_HandlesParentIdValues_Correctly(int parentId)
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 1,
            CategoryName = "Test Category",
            ParentId = parentId
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(parentId, deserializedCategory.ParentId);
    }

    [Fact]
    public void Category_RoundTripSerialization_PreservesAllData()
    {
        // Arrange
        var originalCategory = new Category
        {
            CategoryId = 273,
            CategoryName = "Minifigures",
            ParentId = 0
        };

        // Act
        var json = JsonSerializer.Serialize(originalCategory, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(originalCategory.CategoryId, deserializedCategory.CategoryId);
        Assert.Equal(originalCategory.CategoryName, deserializedCategory.CategoryName);
        Assert.Equal(originalCategory.ParentId, deserializedCategory.ParentId);
    }

    [Fact]
    public void Category_DeserializesFromJson_WithMissingOptionalFields()
    {
        // Arrange - minimal JSON with only category_id
        var json = """
        {
            "category_id": 5
        }
        """;

        // Act
        var category = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(5, category.CategoryId);
        Assert.Equal(string.Empty, category.CategoryName); // Should default to empty string
        Assert.Equal(0, category.ParentId); // Should default to 0
    }

    [Fact]
    public void Category_HandlesHierarchicalStructure_Correctly()
    {
        // Arrange - Simulating a category hierarchy
        var rootCategory = new Category { CategoryId = 1, CategoryName = "Parts", ParentId = 0 };
        var childCategory = new Category { CategoryId = 5, CategoryName = "Bricks", ParentId = 1 };
        var grandchildCategory = new Category { CategoryId = 25, CategoryName = "Standard Bricks", ParentId = 5 };

        var categories = new[] { rootCategory, childCategory, grandchildCategory };

        // Act & Assert
        foreach (var category in categories)
        {
            var json = JsonSerializer.Serialize(category, _jsonOptions);
            var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

            Assert.NotNull(deserializedCategory);
            Assert.Equal(category.CategoryId, deserializedCategory.CategoryId);
            Assert.Equal(category.CategoryName, deserializedCategory.CategoryName);
            Assert.Equal(category.ParentId, deserializedCategory.ParentId);
        }
    }

    [Fact]
    public void Category_HandlesUnicodeCharacters_InCategoryName()
    {
        // Arrange
        var category = new Category
        {
            CategoryId = 1,
            CategoryName = "Catégorie Spéciale 特殊类别 Κατηγορία",
            ParentId = 0
        };

        // Act
        var json = JsonSerializer.Serialize(category, _jsonOptions);
        var deserializedCategory = JsonSerializer.Deserialize<Category>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedCategory);
        Assert.Equal(1, deserializedCategory.CategoryId);
        Assert.Equal("Catégorie Spéciale 特殊类别 Κατηγορία", deserializedCategory.CategoryName);
        Assert.Equal(0, deserializedCategory.ParentId);
    }
}