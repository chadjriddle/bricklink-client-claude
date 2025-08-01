using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Enums;

public class AppearAsTests
{
    private readonly JsonSerializerOptions _jsonOptions;

    public AppearAsTests()
    {
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    [Theory]
    [InlineData(AppearAs.Alternate, "A")]
    [InlineData(AppearAs.Counterpart, "C")]
    [InlineData(AppearAs.Extra, "E")]
    [InlineData(AppearAs.Regular, "R")]
    public void AppearAs_SerializesToCorrectStringValue(AppearAs appearAs, string expectedJson)
    {
        // Arrange
        var testObject = new { Value = appearAs };

        // Act
        var json = JsonSerializer.Serialize(testObject, _jsonOptions);
        var parsedJson = JsonDocument.Parse(json);
        var actualValue = parsedJson.RootElement.GetProperty("value").GetString();

        // Assert
        Assert.Equal(expectedJson, actualValue);
    }

    [Theory]
    [InlineData("A", AppearAs.Alternate)]
    [InlineData("C", AppearAs.Counterpart)]
    [InlineData("E", AppearAs.Extra)]
    [InlineData("R", AppearAs.Regular)]
    public void AppearAs_DeserializesFromCorrectStringValue(string jsonValue, AppearAs expectedAppearAs)
    {
        // Arrange
        var json = $"{{\"value\":\"{jsonValue}\"}}";

        // Act
        var result = JsonSerializer.Deserialize<TestObject>(json, _jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAppearAs, result.Value);
    }

    [Theory]
    [InlineData("X")]
    [InlineData("INVALID")]
    [InlineData("")]
    public void AppearAs_ThrowsJsonExceptionForInvalidStringValue(string invalidValue)
    {
        // Arrange
        var json = $"{{\"value\":\"{invalidValue}\"}}";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestObject>(json, _jsonOptions));
    }

    [Fact]
    public void AppearAs_ThrowsJsonExceptionForNullValue()
    {
        // Arrange
        var json = "{\"value\":null}";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestObject>(json, _jsonOptions));
    }

    [Fact]
    public void AppearAs_ThrowsJsonExceptionForNonStringToken()
    {
        // Arrange
        var json = "{\"value\":123}";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TestObject>(json, _jsonOptions));
    }

    [Fact]
    public void AppearAs_AllEnumValuesCanBeSerializedAndDeserialized()
    {
        // Arrange
        var allValues = Enum.GetValues<AppearAs>();

        foreach (var value in allValues)
        {
            // Act
            var serialized = JsonSerializer.Serialize(new { Value = value }, _jsonOptions);
            var deserialized = JsonSerializer.Deserialize<TestObject>(serialized, _jsonOptions);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(value, deserialized.Value);
        }
    }

    [Fact]
    public void AppearAs_EnumHasExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            AppearAs.Alternate,
            AppearAs.Counterpart,
            AppearAs.Extra,
            AppearAs.Regular
        };

        // Act
        var actualValues = Enum.GetValues<AppearAs>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    private class TestObject
    {
        public AppearAs Value { get; set; }
    }
}
