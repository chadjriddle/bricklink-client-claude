using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="TimestampGenerator"/> class.
/// </summary>
public class TimestampGeneratorTests
{
    [Fact]
    public void Generate_ReturnsNonEmptyString()
    {
        // Act
        var timestamp = TimestampGenerator.Generate();

        // Assert
        Assert.False(string.IsNullOrEmpty(timestamp));
    }

    [Fact]
    public void Generate_ReturnsValidUnixTimestamp()
    {
        // Act
        var timestamp = TimestampGenerator.Generate();

        // Assert
        Assert.True(long.TryParse(timestamp, out var unixTime));

        // Unix timestamp should be positive and reasonable (after Unix epoch)
        Assert.True(unixTime > 0);

        // Should be within reasonable range (after 2020, before 2050)
        var year2020 = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var year2050 = new DateTimeOffset(2050, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var timestampAsDate = TimestampGenerator.FromTimestamp(timestamp);

        Assert.True(timestampAsDate >= year2020);
        Assert.True(timestampAsDate <= year2050);
    }

    [Fact]
    public void Generate_WithSpecificDateTime_ReturnsCorrectTimestamp()
    {
        // Arrange
        var testDate = new DateTimeOffset(2023, 6, 15, 14, 30, 45, TimeSpan.Zero);
        var expectedTimestamp = "1686839445"; // Unix timestamp for 2023-06-15 14:30:45 UTC

        // Act
        var timestamp = TimestampGenerator.Generate(testDate);

        // Assert
        Assert.Equal(expectedTimestamp, timestamp);
    }

    [Fact]
    public void Generate_WithUtcDateTime_ReturnsConsistentResult()
    {
        // Arrange
        var utcDate = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        var timestamp = TimestampGenerator.Generate(utcDate);

        // Assert
        Assert.Equal("1672574400", timestamp); // Unix timestamp for 2023-01-01 12:00:00 UTC
    }

    [Fact]
    public void Generate_WithTimeZoneOffset_ConvertsToUtc()
    {
        // Arrange
        var easternTime = new DateTimeOffset(2023, 1, 1, 7, 0, 0, TimeSpan.FromHours(-5)); // EST
        var utcTime = new DateTimeOffset(2023, 1, 1, 12, 0, 0, TimeSpan.Zero); // UTC

        // Act
        var timestampFromEst = TimestampGenerator.Generate(easternTime);
        var timestampFromUtc = TimestampGenerator.Generate(utcTime);

        // Assert
        // Both should produce the same timestamp since they represent the same moment
        Assert.Equal(timestampFromUtc, timestampFromEst);
    }

    [Fact]
    public void Generate_ConsecutiveCalls_ReturnsIncreasingValues()
    {
        // Act
        var timestamp1 = TimestampGenerator.Generate();

        // Small delay to ensure different timestamps
        Thread.Sleep(1100); // Sleep for slightly more than 1 second

        var timestamp2 = TimestampGenerator.Generate();

        // Assert
        Assert.True(long.TryParse(timestamp1, out var time1));
        Assert.True(long.TryParse(timestamp2, out var time2));
        Assert.True(time2 > time1);
    }

    [Fact]
    public void FromTimestamp_WithValidTimestamp_ReturnsCorrectDateTime()
    {
        // Arrange
        var expectedDate = new DateTimeOffset(2023, 6, 15, 14, 30, 45, TimeSpan.Zero);
        var timestamp = "1686839445";

        // Act
        var result = TimestampGenerator.FromTimestamp(timestamp);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void FromTimestamp_WithZeroTimestamp_ReturnsUnixEpoch()
    {
        // Arrange
        var timestamp = "0";
        var expectedDate = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = TimestampGenerator.FromTimestamp(timestamp);

        // Assert
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public void FromTimestamp_WithNullTimestamp_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            TimestampGenerator.FromTimestamp(null!));

        Assert.Equal("timestamp", exception.ParamName);
    }

    [Theory]
    [InlineData("not-a-number")]
    [InlineData("abc123")]
    [InlineData("12.34")]
    [InlineData("")]
    [InlineData("   ")]
    public void FromTimestamp_WithInvalidFormat_ThrowsFormatException(string invalidTimestamp)
    {
        // Act & Assert
        var exception = Assert.Throws<FormatException>(() =>
            TimestampGenerator.FromTimestamp(invalidTimestamp));

        Assert.Contains($"Invalid timestamp format: {invalidTimestamp}", exception.Message);
    }

    [Theory]
    [InlineData("999999999999999")] // Very large timestamp
    [InlineData("-999999999999999")] // Negative timestamp that would underflow
    public void FromTimestamp_WithOutOfRangeTimestamp_ThrowsArgumentOutOfRangeException(string outOfRangeTimestamp)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            TimestampGenerator.FromTimestamp(outOfRangeTimestamp));

        Assert.Equal("timestamp", exception.ParamName);
        Assert.Contains($"Timestamp {outOfRangeTimestamp} is outside the valid date range", exception.Message);
    }

    [Fact]
    public void Generate_AndFromTimestamp_AreInverse()
    {
        // Arrange
        var originalDate = new DateTimeOffset(2023, 8, 15, 10, 30, 45, TimeSpan.Zero);

        // Act
        var timestamp = TimestampGenerator.Generate(originalDate);
        var recoveredDate = TimestampGenerator.FromTimestamp(timestamp);

        // Assert
        Assert.Equal(originalDate, recoveredDate);
    }

    [Fact]
    public void Generate_WithCurrentTime_IsReasonablyClose()
    {
        // Arrange
        var beforeGeneration = DateTimeOffset.UtcNow.AddSeconds(-1); // Add 1 second tolerance

        // Act
        var timestamp = TimestampGenerator.Generate();
        var afterGeneration = DateTimeOffset.UtcNow.AddSeconds(1); // Add 1 second tolerance

        // Assert
        var timestampAsDate = TimestampGenerator.FromTimestamp(timestamp);

        // The generated timestamp should be between before and after measurements
        Assert.True(timestampAsDate >= beforeGeneration,
            $"Generated timestamp {timestampAsDate} should be >= {beforeGeneration}");
        Assert.True(timestampAsDate <= afterGeneration,
            $"Generated timestamp {timestampAsDate} should be <= {afterGeneration}");
    }

    [Fact]
    public void Generate_WithUnixEpoch_ReturnsZero()
    {
        // Arrange
        var unixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var timestamp = TimestampGenerator.Generate(unixEpoch);

        // Assert
        Assert.Equal("0", timestamp);
    }

    [Fact]
    public void Generate_WithFutureDate_ReturnsPositiveTimestamp()
    {
        // Arrange
        var futureDate = new DateTimeOffset(2030, 12, 31, 23, 59, 59, TimeSpan.Zero);

        // Act
        var timestamp = TimestampGenerator.Generate(futureDate);

        // Assert
        Assert.True(long.TryParse(timestamp, out var unixTime));
        Assert.True(unixTime > 0);

        // Verify round-trip conversion
        var recoveredDate = TimestampGenerator.FromTimestamp(timestamp);
        Assert.Equal(futureDate, recoveredDate);
    }

    [Fact]
    public void FromTimestamp_WithNegativeTimestamp_ReturnsDateBeforeEpoch()
    {
        // Arrange - represents 1969-12-31 23:59:59 UTC
        var timestamp = "-1";
        var expectedDate = new DateTimeOffset(1969, 12, 31, 23, 59, 59, TimeSpan.Zero);

        // Act
        var result = TimestampGenerator.FromTimestamp(timestamp);

        // Assert
        Assert.Equal(expectedDate, result);
    }
}
