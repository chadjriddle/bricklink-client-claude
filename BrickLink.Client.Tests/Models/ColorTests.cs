using System.Text.Json;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Models;

public class ColorTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public ColorTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Fact]
    public void Color_DefaultConstructor_SetsDefaultValues()
    {
        // Arrange & Act
        var color = new Color();

        // Assert
        Assert.Equal(0, color.ColorId);
        Assert.Equal(string.Empty, color.ColorName);
        Assert.Equal(string.Empty, color.ColorCode);
        Assert.Equal(string.Empty, color.ColorType);
    }

    [Fact]
    public void Color_PropertiesCanBeSet()
    {
        // Arrange
        var color = new Color();

        // Act
        color.ColorId = 11;
        color.ColorName = "Black";
        color.ColorCode = "05131D";
        color.ColorType = "Solid";

        // Assert
        Assert.Equal(11, color.ColorId);
        Assert.Equal("Black", color.ColorName);
        Assert.Equal("05131D", color.ColorCode);
        Assert.Equal("Solid", color.ColorType);
    }

    [Fact]
    public void Color_SerializesToJson_WithCorrectPropertyNames()
    {
        // Arrange
        var color = new Color
        {
            ColorId = 11,
            ColorName = "Black",
            ColorCode = "05131D",
            ColorType = "Solid"
        };

        // Act
        var json = JsonSerializer.Serialize(color, _jsonOptions);

        // Assert
        Assert.Contains("\"color_id\":11", json);
        Assert.Contains("\"color_name\":\"Black\"", json);
        Assert.Contains("\"color_code\":\"05131D\"", json);
        Assert.Contains("\"color_type\":\"Solid\"", json);
    }

    [Fact]
    public void Color_DeserializesFromJson_WithCorrectPropertyNames()
    {
        // Arrange
        var json = """
        {
            "color_id": 11,
            "color_name": "Black",
            "color_code": "05131D",
            "color_type": "Solid"
        }
        """;

        // Act
        var color = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(color);
        Assert.Equal(11, color.ColorId);
        Assert.Equal("Black", color.ColorName);
        Assert.Equal("05131D", color.ColorCode);
        Assert.Equal("Solid", color.ColorType);
    }

    [Theory]
    [InlineData(0, "White", "FFFFFF", "Solid")]
    [InlineData(11, "Black", "05131D", "Solid")]
    [InlineData(1, "Blue", "0055BF", "Solid")]
    [InlineData(43, "Trans-Light Blue", "AEEFEC", "Transparent")]
    [InlineData(61, "Chrome Silver", "E0E0E0", "Chrome")]
    public void Color_SerializesAndDeserializesVariousColors_Correctly(int colorId, string colorName, string colorCode, string colorType)
    {
        // Arrange
        var originalColor = new Color
        {
            ColorId = colorId,
            ColorName = colorName,
            ColorCode = colorCode,
            ColorType = colorType
        };

        // Act
        var json = JsonSerializer.Serialize(originalColor, _jsonOptions);
        var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedColor);
        Assert.Equal(colorId, deserializedColor.ColorId);
        Assert.Equal(colorName, deserializedColor.ColorName);
        Assert.Equal(colorCode, deserializedColor.ColorCode);
        Assert.Equal(colorType, deserializedColor.ColorType);
    }

    [Fact]
    public void Color_HandlesEmptyStrings_Correctly()
    {
        // Arrange
        var color = new Color
        {
            ColorId = 1,
            ColorName = "",
            ColorCode = "",
            ColorType = ""
        };

        // Act
        var json = JsonSerializer.Serialize(color, _jsonOptions);
        var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedColor);
        Assert.Equal(1, deserializedColor.ColorId);
        Assert.Equal("", deserializedColor.ColorName);
        Assert.Equal("", deserializedColor.ColorCode);
        Assert.Equal("", deserializedColor.ColorType);
    }

    [Fact]
    public void Color_HandlesSpecialCharacters_InStrings()
    {
        // Arrange
        var color = new Color
        {
            ColorId = 999,
            ColorName = "Special Color (Test) & More",
            ColorCode = "FF00FF",
            ColorType = "Test-Type"
        };

        // Act
        var json = JsonSerializer.Serialize(color, _jsonOptions);
        var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedColor);
        Assert.Equal(999, deserializedColor.ColorId);
        Assert.Equal("Special Color (Test) & More", deserializedColor.ColorName);
        Assert.Equal("FF00FF", deserializedColor.ColorCode);
        Assert.Equal("Test-Type", deserializedColor.ColorType);
    }

    [Fact]
    public void Color_HandlesLongStrings_Correctly()
    {
        // Arrange
        var longColorName = new string('A', 200);
        var longColorCode = new string('F', 10);
        var longColorType = new string('B', 100);
        var color = new Color
        {
            ColorId = int.MaxValue,
            ColorName = longColorName,
            ColorCode = longColorCode,
            ColorType = longColorType
        };

        // Act
        var json = JsonSerializer.Serialize(color, _jsonOptions);
        var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedColor);
        Assert.Equal(int.MaxValue, deserializedColor.ColorId);
        Assert.Equal(longColorName, deserializedColor.ColorName);
        Assert.Equal(longColorCode, deserializedColor.ColorCode);
        Assert.Equal(longColorType, deserializedColor.ColorType);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(999999)]
    public void Color_HandlesColorIdValues_Correctly(int colorId)
    {
        // Arrange
        var color = new Color
        {
            ColorId = colorId,
            ColorName = "Test Color",
            ColorCode = "000000",
            ColorType = "Test"
        };

        // Act
        var json = JsonSerializer.Serialize(color, _jsonOptions);
        var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedColor);
        Assert.Equal(colorId, deserializedColor.ColorId);
    }

    [Fact]
    public void Color_RoundTripSerialization_PreservesAllData()
    {
        // Arrange
        var originalColor = new Color
        {
            ColorId = 43,
            ColorName = "Trans-Light Blue",
            ColorCode = "AEEFEC",
            ColorType = "Transparent"
        };

        // Act
        var json = JsonSerializer.Serialize(originalColor, _jsonOptions);
        var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(deserializedColor);
        Assert.Equal(originalColor.ColorId, deserializedColor.ColorId);
        Assert.Equal(originalColor.ColorName, deserializedColor.ColorName);
        Assert.Equal(originalColor.ColorCode, deserializedColor.ColorCode);
        Assert.Equal(originalColor.ColorType, deserializedColor.ColorType);
    }

    [Fact]
    public void Color_DeserializesFromJson_WithMissingOptionalFields()
    {
        // Arrange - minimal JSON with only color_id
        var json = """
        {
            "color_id": 11
        }
        """;

        // Act
        var color = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

        // Assert
        Assert.NotNull(color);
        Assert.Equal(11, color.ColorId);
        Assert.Equal(string.Empty, color.ColorName); // Should default to empty string
        Assert.Equal(string.Empty, color.ColorCode); // Should default to empty string
        Assert.Equal(string.Empty, color.ColorType); // Should default to empty string
    }

    [Fact]
    public void Color_HandlesHexColorCodes_Correctly()
    {
        // Arrange
        var testCases = new[]
        {
            ("05131D", "Black"),
            ("FFFFFF", "White"),
            ("FF0000", "Red"),
            ("00FF00", "Green"),
            ("0000FF", "Blue"),
            ("AEEFEC", "Trans-Light Blue")
        };

        foreach (var (colorCode, colorName) in testCases)
        {
            var color = new Color
            {
                ColorId = 1,
                ColorName = colorName,
                ColorCode = colorCode,
                ColorType = "Test"
            };

            // Act
            var json = JsonSerializer.Serialize(color, _jsonOptions);
            var deserializedColor = JsonSerializer.Deserialize<Color>(json, _jsonOptions);

            // Assert
            Assert.NotNull(deserializedColor);
            Assert.Equal(colorCode, deserializedColor.ColorCode);
            Assert.Equal(colorName, deserializedColor.ColorName);
        }
    }
}