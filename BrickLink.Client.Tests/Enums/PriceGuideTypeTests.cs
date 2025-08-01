using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Enums;

/// <summary>
/// Unit tests for the PriceGuideType enum.
/// </summary>
public class PriceGuideTypeTests
{
    [Theory]
    [InlineData(PriceGuideType.Stock, "stock")]
    [InlineData(PriceGuideType.Sold, "sold")]
    public void PriceGuideType_SerializesToCorrectJsonValue(PriceGuideType guideType, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(guideType, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal($"\"{expectedJson}\"", json);
    }

    [Theory]
    [InlineData("\"stock\"", PriceGuideType.Stock)]
    [InlineData("\"sold\"", PriceGuideType.Sold)]
    public void PriceGuideType_DeserializesFromCorrectJsonValue(string json, PriceGuideType expectedGuideType)
    {
        // Act
        var guideType = JsonSerializer.Deserialize<PriceGuideType>(json, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal(expectedGuideType, guideType);
    }

    [Fact]
    public void PriceGuideType_HasAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            PriceGuideType.Stock,
            PriceGuideType.Sold
        };

        // Act
        var actualValues = Enum.GetValues<PriceGuideType>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    [Theory]
    [InlineData("\"INVALID\"")]
    [InlineData("\"STOCK\"")]
    [InlineData("\"SOLD\"")]
    [InlineData("\"Stock\"")]
    [InlineData("\"Sold\"")]
    public void PriceGuideType_ThrowsExceptionForInvalidJsonValue(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PriceGuideType>(invalidJson, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void PriceGuideType_SerializationRoundTrip_PreservesValues()
    {
        foreach (var guideType in Enum.GetValues<PriceGuideType>())
        {
            // Act
            var json = JsonSerializer.Serialize(guideType, JsonSerializationHelper.DefaultOptions);
            var deserialized = JsonSerializer.Deserialize<PriceGuideType>(json, JsonSerializationHelper.DefaultOptions);

            // Assert
            Assert.Equal(guideType, deserialized);
        }
    }

    [Fact]
    public void PriceGuideType_ToString_ReturnsEnumName()
    {
        // Act & Assert
        Assert.Equal("Stock", PriceGuideType.Stock.ToString());
        Assert.Equal("Sold", PriceGuideType.Sold.ToString());
    }

    [Theory]
    [InlineData(PriceGuideType.Stock, "Current stock-based pricing")]
    [InlineData(PriceGuideType.Sold, "Historical sold pricing")]
    public void PriceGuideType_HasCorrectSemanticMeaning(PriceGuideType guideType, string expectedMeaning)
    {
        // This test documents the semantic meaning of each enum value
        // Act
        var actualName = guideType.ToString();

        // Assert
        switch (guideType)
        {
            case PriceGuideType.Stock:
                Assert.Equal("Stock", actualName);
                Assert.Contains("stock", expectedMeaning.ToLower());
                break;
            case PriceGuideType.Sold:
                Assert.Equal("Sold", actualName);
                Assert.Contains("sold", expectedMeaning.ToLower());
                break;
            default:
                Assert.Fail($"Unexpected guide type value: {guideType}");
                break;
        }
    }

    [Fact]
    public void PriceGuideType_DeserializesFromNullToken_ThrowsJsonException()
    {
        // Arrange
        var json = "null";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PriceGuideType>(json, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void PriceGuideType_DeserializesFromNumberToken_ThrowsJsonException()
    {
        // Arrange
        var json = "123";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PriceGuideType>(json, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void PriceGuideType_SerializesInvalidEnumValue_ThrowsJsonException()
    {
        // Arrange - cast an invalid integer to PriceGuideType
        var invalidGuideType = (PriceGuideType)999;

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Serialize(invalidGuideType, JsonSerializationHelper.DefaultOptions));
    }
}
