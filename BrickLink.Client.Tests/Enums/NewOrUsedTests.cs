using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Enums;

/// <summary>
/// Unit tests for the NewOrUsed enum.
/// </summary>
public class NewOrUsedTests
{
    [Theory]
    [InlineData(NewOrUsed.New, "N")]
    [InlineData(NewOrUsed.Used, "U")]
    public void NewOrUsed_SerializesToCorrectJsonValue(NewOrUsed condition, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(condition, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal($"\"{expectedJson}\"", json);
    }

    [Theory]
    [InlineData("\"N\"", NewOrUsed.New)]
    [InlineData("\"U\"", NewOrUsed.Used)]
    public void NewOrUsed_DeserializesFromCorrectJsonValue(string json, NewOrUsed expectedCondition)
    {
        // Act
        var condition = JsonSerializer.Deserialize<NewOrUsed>(json, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal(expectedCondition, condition);
    }

    [Fact]
    public void NewOrUsed_HasAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            NewOrUsed.New,
            NewOrUsed.Used
        };

        // Act
        var actualValues = Enum.GetValues<NewOrUsed>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    [Theory]
    [InlineData("\"INVALID\"")]
    [InlineData("\"new\"")]
    [InlineData("\"used\"")]
    [InlineData("\"NEW\"")]
    [InlineData("\"USED\"")]
    public void NewOrUsed_ThrowsExceptionForInvalidJsonValue(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<NewOrUsed>(invalidJson, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void NewOrUsed_SerializationRoundTrip_PreservesValues()
    {
        foreach (var condition in Enum.GetValues<NewOrUsed>())
        {
            // Act
            var json = JsonSerializer.Serialize(condition, JsonSerializationHelper.DefaultOptions);
            var deserialized = JsonSerializer.Deserialize<NewOrUsed>(json, JsonSerializationHelper.DefaultOptions);

            // Assert
            Assert.Equal(condition, deserialized);
        }
    }

    [Fact]
    public void NewOrUsed_ToString_ReturnsEnumName()
    {
        // Act & Assert
        Assert.Equal("New", NewOrUsed.New.ToString());
        Assert.Equal("Used", NewOrUsed.Used.ToString());
    }
}