using System.Text.Json;
using BrickLink.Client.Enums;
using BrickLink.Client.Serialization;
using Xunit;

namespace BrickLink.Client.Tests.Enums;

/// <summary>
/// Unit tests for the Direction enum.
/// </summary>
public class DirectionTests
{
    [Theory]
    [InlineData(Direction.In, "in")]
    [InlineData(Direction.Out, "out")]
    public void Direction_SerializesToCorrectJsonValue(Direction direction, string expectedJson)
    {
        // Act
        var json = JsonSerializer.Serialize(direction, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal($"\"{expectedJson}\"", json);
    }

    [Theory]
    [InlineData("\"in\"", Direction.In)]
    [InlineData("\"out\"", Direction.Out)]
    public void Direction_DeserializesFromCorrectJsonValue(string json, Direction expectedDirection)
    {
        // Act
        var direction = JsonSerializer.Deserialize<Direction>(json, JsonSerializationHelper.DefaultOptions);

        // Assert
        Assert.Equal(expectedDirection, direction);
    }

    [Fact]
    public void Direction_HasAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            Direction.In,
            Direction.Out
        };

        // Act
        var actualValues = Enum.GetValues<Direction>();

        // Assert
        Assert.Equal(expectedValues.Length, actualValues.Length);
        foreach (var expectedValue in expectedValues)
        {
            Assert.Contains(expectedValue, actualValues);
        }
    }

    [Theory]
    [InlineData("\"INVALID\"")]
    [InlineData("\"IN\"")]
    [InlineData("\"OUT\"")]
    [InlineData("\"In\"")]
    [InlineData("\"Out\"")]
    public void Direction_ThrowsExceptionForInvalidJsonValue(string invalidJson)
    {
        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Direction>(invalidJson, JsonSerializationHelper.DefaultOptions));
    }

    [Fact]
    public void Direction_SerializationRoundTrip_PreservesValues()
    {
        foreach (var direction in Enum.GetValues<Direction>())
        {
            // Act
            var json = JsonSerializer.Serialize(direction, JsonSerializationHelper.DefaultOptions);
            var deserialized = JsonSerializer.Deserialize<Direction>(json, JsonSerializationHelper.DefaultOptions);

            // Assert
            Assert.Equal(direction, deserialized);
        }
    }

    [Fact]
    public void Direction_ToString_ReturnsEnumName()
    {
        // Act & Assert
        Assert.Equal("In", Direction.In.ToString());
        Assert.Equal("Out", Direction.Out.ToString());
    }

    [Theory]
    [InlineData(Direction.In)]
    [InlineData(Direction.Out)]
    public void Direction_HasCorrectSemanticMeaning(Direction direction)
    {
        // This test documents the semantic meaning of each enum value
        // Act
        var actualName = direction.ToString();

        // Assert
        switch (direction)
        {
            case Direction.In:
                Assert.Equal("In", actualName);
                break;
            case Direction.Out:
                Assert.Equal("Out", actualName);
                break;
            default:
                Assert.Fail($"Unexpected direction value: {direction}");
                break;
        }
    }
}