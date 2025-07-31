using System.Globalization;
using System.Text.Json;
using BrickLink.Client.Serialization.Converters;
using Xunit;

namespace BrickLink.Client.Tests.Serialization.Converters;

public class DateTimeOffsetConverterTests
{
    private readonly DateTimeOffsetConverter _converter = new();
    private readonly JsonSerializerOptions _options = new() { Converters = { new DateTimeOffsetConverter() } };

    [Fact]
    public void Read_WithIso8601WithMilliseconds_ShouldParseCorrectly()
    {
        // Arrange
        var json = "\"2023-12-25T15:30:45.123Z\"";
        var expected = new DateTimeOffset(2023, 12, 25, 15, 30, 45, 123, TimeSpan.Zero);

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithIso8601WithoutMilliseconds_ShouldParseCorrectly()
    {
        // Arrange
        var json = "\"2023-12-25T15:30:45Z\"";
        var expected = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.Zero);

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithBrickLinkLegacyFormat_ShouldParseCorrectly()
    {
        // Arrange
        var json = "\"2023-12-25 15:30:45\"";
        var expected = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.Zero);

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithUnixTimestamp_ShouldParseCorrectly()
    {
        // Arrange
        var json = "1703515845"; // Unix timestamp for 2023-12-25 15:30:45 UTC
        var expected = DateTimeOffset.FromUnixTimeSeconds(1703515845);

        // Act
        var result = JsonSerializer.Deserialize<DateTimeOffset>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithInvalidString_ShouldThrowJsonException()
    {
        // Arrange
        var json = "\"invalid-date-string\"";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTimeOffset>(json, _options));
    }

    [Fact]
    public void Read_WithEmptyString_ShouldThrowJsonException()
    {
        // Arrange
        var json = "\"\"";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTimeOffset>(json, _options));
    }

    [Fact]
    public void Read_WithOutOfRangeUnixTimestamp_ShouldThrowJsonException()
    {
        // Arrange
        var json = "9999999999999999999"; // Extremely large timestamp

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTimeOffset>(json, _options));
    }

    [Fact]
    public void Write_WithDateTimeOffset_ShouldWriteIso8601Format()
    {
        // Arrange
        var dateTime = new DateTimeOffset(2023, 12, 25, 15, 30, 45, 123, TimeSpan.Zero);
        var expected = "\"2023-12-25T15:30:45.123Z\"";

        // Act
        var result = JsonSerializer.Serialize(dateTime, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Write_WithLocalDateTimeOffset_ShouldConvertToUtc()
    {
        // Arrange
        var localTime = new DateTimeOffset(2023, 12, 25, 15, 30, 45, 123, TimeSpan.FromHours(5));
        var expected = "\"2023-12-25T10:30:45.123Z\""; // Converted to UTC

        // Act
        var result = JsonSerializer.Serialize(localTime, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2023-12-25T15:30:45.123Z")]
    [InlineData("2023-12-25T15:30:45Z")]
    [InlineData("2023-12-25 15:30:45")]
    [InlineData("2023-12-25T15:30:45.123")]
    [InlineData("2023-12-25T15:30:45")]
    public void RoundTrip_WithSupportedFormats_ShouldMaintainPrecision(string inputFormat)
    {
        // Arrange
        var originalJson = $"\"{inputFormat}\"";

        // Act
        var deserialized = JsonSerializer.Deserialize<DateTimeOffset>(originalJson, _options);
        var serialized = JsonSerializer.Serialize(deserialized, _options);
        var roundTrip = JsonSerializer.Deserialize<DateTimeOffset>(serialized, _options);

        // Assert
        Assert.Equal(deserialized, roundTrip);
    }
}
