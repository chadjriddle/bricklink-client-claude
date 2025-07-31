using System.Text.Json;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Serialization;

public class JsonSerializationHelperTests
{
    private readonly TestData _testData = new TestData { Id = 42, Name = "Test", Value = 123.45m };
    private readonly string _validJson = "{\"id\":42,\"name\":\"Test\",\"value\":123.45}";
    private readonly string _invalidJson = "{invalid json}";

    [Fact]
    public void DefaultOptions_ShouldReturnConfiguredOptions()
    {
        // Act
        var options = JsonSerializationHelper.DefaultOptions;

        // Assert
        Assert.NotNull(options);
        Assert.Equal(JsonNamingPolicy.CamelCase, options.PropertyNamingPolicy);
        Assert.True(options.PropertyNameCaseInsensitive);
        Assert.False(options.WriteIndented);
    }

    [Fact]
    public void DebugOptions_ShouldReturnIndentedOptions()
    {
        // Act
        var options = JsonSerializationHelper.DebugOptions;

        // Assert
        Assert.NotNull(options);
        Assert.True(options.WriteIndented);
    }

    [Fact]
    public void ProductionOptions_ShouldReturnNonIndentedOptions()
    {
        // Act
        var options = JsonSerializationHelper.ProductionOptions;

        // Assert
        Assert.NotNull(options);
        Assert.False(options.WriteIndented);
    }

    [Fact]
    public void Serialize_WithValidObject_ShouldReturnJson()
    {
        // Act
        var result = JsonSerializationHelper.Serialize(_testData);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"id\":42", result);
        Assert.Contains("\"name\":\"Test\"", result);
    }

    [Fact]
    public void Serialize_WithCustomOptions_ShouldUseProvidedOptions()
    {
        // Arrange
        var customOptions = new JsonSerializerOptions { WriteIndented = true };

        // Act
        var result = JsonSerializationHelper.Serialize(_testData, customOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(Environment.NewLine, result); // Should be indented
    }

    [Fact]
    public void Deserialize_WithValidJson_ShouldReturnObject()
    {
        // Act
        var result = JsonSerializationHelper.Deserialize<TestData>(_validJson);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("Test", result.Name);
        Assert.Equal(123.45m, result.Value);
    }

    [Fact]
    public void Deserialize_WithEmptyString_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => JsonSerializationHelper.Deserialize<TestData>(""));
    }

    [Fact]
    public void Deserialize_WithNullString_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => JsonSerializationHelper.Deserialize<TestData>(null!));
    }

    [Fact]
    public void Deserialize_WithInvalidJson_ShouldThrowJsonException()
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializationHelper.Deserialize<TestData>(_invalidJson));
    }

    [Fact]
    public void TryDeserialize_WithValidJson_ShouldReturnObject()
    {
        // Act
        var result = JsonSerializationHelper.TryDeserialize<TestData>(_validJson);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public void TryDeserialize_WithInvalidJson_ShouldReturnNull()
    {
        // Act
        var result = JsonSerializationHelper.TryDeserialize<TestData>(_invalidJson);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void TryDeserialize_WithEmptyString_ShouldReturnNull()
    {
        // Act
        var result = JsonSerializationHelper.TryDeserialize<TestData>("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DeserializeApiResponse_WithValidApiResponse_ShouldReturnResponse()
    {
        // Arrange
        var apiResponseJson = "{\"meta\":{\"code\":200,\"message\":\"OK\"},\"data\":{\"id\":42,\"name\":\"Test\",\"value\":123.45}}";

        // Act
        var result = JsonSerializationHelper.DeserializeApiResponse<TestData>(apiResponseJson);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Meta);
        Assert.Equal(200, result.Meta.Code);
        Assert.Equal("OK", result.Meta.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(42, result.Data.Id);
    }

    [Fact]
    public void DeserializeApiResponse_WithEmptyString_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => JsonSerializationHelper.DeserializeApiResponse<TestData>(""));
    }

    [Fact]
    public void IsValidJson_WithValidJson_ShouldReturnTrue()
    {
        // Act
        var result = JsonSerializationHelper.IsValidJson(_validJson);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidJson_WithInvalidJson_ShouldReturnFalse()
    {
        // Act
        var result = JsonSerializationHelper.IsValidJson(_invalidJson);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidJson_WithEmptyString_ShouldReturnFalse()
    {
        // Act
        var result = JsonSerializationHelper.IsValidJson("");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void PrettyPrint_WithValidJson_ShouldReturnFormattedJson()
    {
        // Act
        var result = JsonSerializationHelper.PrettyPrint(_validJson);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(Environment.NewLine, result);
        Assert.Contains("  ", result); // Should contain indentation
    }

    [Fact]
    public void PrettyPrint_WithInvalidJson_ShouldReturnOriginalString()
    {
        // Act
        var result = JsonSerializationHelper.PrettyPrint(_invalidJson);

        // Assert
        Assert.Equal(_invalidJson, result);
    }

    [Fact]
    public void RoundTrip_WithComplexObject_ShouldMaintainData()
    {
        // Arrange
        var originalData = new TestData
        {
            Id = 42,
            Name = "Test with special chars: åäö",
            Value = 123.4567m,
            CreatedAt = new DateTimeOffset(2023, 12, 25, 15, 30, 45, 123, TimeSpan.Zero) // Use fixed timestamp
        };

        // Act
        var json = JsonSerializationHelper.Serialize(originalData);
        var roundTrip = JsonSerializationHelper.Deserialize<TestData>(json);

        // Assert
        Assert.Equal(originalData.Id, roundTrip.Id);
        Assert.Equal(originalData.Name, roundTrip.Name);
        Assert.Equal(originalData.Value, roundTrip.Value);
        Assert.Equal(originalData.CreatedAt, roundTrip.CreatedAt);
    }

    private class TestData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
