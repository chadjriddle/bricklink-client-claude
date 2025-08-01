using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Enums;

/// <summary>
/// Unit tests for the Completeness enum.
/// </summary>
public class CompletenessTests
{
    [Theory]
    [InlineData(Completeness.Complete, "C")]
    [InlineData(Completeness.Incomplete, "B")]
    [InlineData(Completeness.Sealed, "S")]
    public void Completeness_SerializesToCorrectJsonValue(Completeness completeness, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(completeness, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal($"\"{expectedJson}\"", json);
    }

    [Theory]
    [InlineData("\"C\"", Completeness.Complete)]
    [InlineData("\"B\"", Completeness.Incomplete)]
    [InlineData("\"S\"", Completeness.Sealed)]
    public void Completeness_DeserializesFromCorrectJsonValue(string json, Completeness expectedCompleteness)
    {
        // Act
        var completeness = JsonSerializer.Deserialize<Completeness>(json, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal(expectedCompleteness, completeness);
    }

    [Fact]
    public void Completeness_HasAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            Completeness.Complete,
            Completeness.Incomplete,
            Completeness.Sealed
        };

        // Act
        var actualValues = Enum.GetValues<Completeness>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    [Theory]
    [InlineData("\"INVALID\"")]
    [InlineData("\"complete\"")]
    [InlineData("\"incomplete\"")]
    [InlineData("\"sealed\"")]
    [InlineData("\"COMPLETE\"")]
    [InlineData("\"INCOMPLETE\"")]
    [InlineData("\"SEALED\"")]
    public void Completeness_ThrowsExceptionForInvalidJsonValue(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Completeness>(invalidJson, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void Completeness_SerializationRoundTrip_PreservesValues()
    {
        foreach (var completeness in Enum.GetValues<Completeness>())
        {
            // Act
            var json = JsonSerializer.Serialize(completeness, JsonSerializationHelper.DefaultOptions);
            var deserialized = JsonSerializer.Deserialize<Completeness>(json, JsonSerializationHelper.DefaultOptions);

            // Assert
            Assert.Equal(completeness, deserialized);
        }
    }

    [Fact]
    public void Completeness_ToString_ReturnsEnumName()
    {
        // Act & Assert
        Assert.Equal("Complete", Completeness.Complete.ToString());
        Assert.Equal("Incomplete", Completeness.Incomplete.ToString());
        Assert.Equal("Sealed", Completeness.Sealed.ToString());
    }

    [Theory]
    [InlineData(Completeness.Complete)]
    [InlineData(Completeness.Incomplete)]
    [InlineData(Completeness.Sealed)]
    public void Completeness_HasCorrectSemanticMeaning(Completeness completeness)
    {
        // This test documents the semantic meaning of each enum value
        // Act
        var actualName = completeness.ToString();

        // Assert
        switch (completeness)
        {
            case Completeness.Complete:
                Assert.Equal("Complete", actualName);
                break;
            case Completeness.Incomplete:
                Assert.Equal("Incomplete", actualName);
                break;
            case Completeness.Sealed:
                Assert.Equal("Sealed", actualName);
                break;
            default:
                Assert.Fail($"Unexpected completeness value: {completeness}");
                break;
        }
    }
}