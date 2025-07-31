using System.Text.Json;
using BrickLink.Client.Serialization.Converters;
using Xunit;

namespace BrickLink.Client.Tests.Serialization.Converters;

public class DecimalPrecisionConverterTests
{
    private readonly DecimalPrecisionConverter _converter = new();
    private readonly JsonSerializerOptions _options = new() { Converters = { new DecimalPrecisionConverter() } };

    [Fact]
    public void Read_WithValidDecimalNumber_ShouldParseCorrectly()
    {
        // Arrange
        var json = "123.4567";
        var expected = 123.4567m;

        // Act
        var result = JsonSerializer.Deserialize<decimal>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithHighPrecisionDecimal_ShouldRoundTo4Places()
    {
        // Arrange
        var json = "123.456789123";
        var expected = 123.4568m; // Rounded to 4 decimal places

        // Act
        var result = JsonSerializer.Deserialize<decimal>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithIntegerNumber_ShouldParseCorrectly()
    {
        // Arrange
        var json = "42";
        var expected = 42m;

        // Act
        var result = JsonSerializer.Deserialize<decimal>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithZero_ShouldParseCorrectly()
    {
        // Arrange
        var json = "0";
        var expected = 0m;

        // Act
        var result = JsonSerializer.Deserialize<decimal>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithNegativeNumber_ShouldParseCorrectly()
    {
        // Arrange
        var json = "-123.45";
        var expected = -123.45m;

        // Act
        var result = JsonSerializer.Deserialize<decimal>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithStringNumber_ShouldParseCorrectly()
    {
        // Arrange
        var json = "\"123.45\"";
        var expected = 123.45m;

        // Act
        var result = JsonSerializer.Deserialize<decimal>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithEmptyString_ShouldThrowJsonException()
    {
        // Arrange
        var json = "\"\"";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal>(json, _options));
    }

    [Fact]
    public void Read_WithInvalidString_ShouldThrowJsonException()
    {
        // Arrange
        var json = "\"not-a-number\"";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal>(json, _options));
    }

    [Fact]
    public void Read_WithNull_ShouldThrowJsonException()
    {
        // Arrange
        var json = "null";

        // Act & Assert
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<decimal>(json, _options));
    }

    [Fact]
    public void Write_WithDecimalValue_ShouldWriteAsNumber()
    {
        // Arrange
        var value = 123.45m;
        var expected = "123.45";

        // Act
        var result = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Write_WithHighPrecisionDecimal_ShouldMaintainPrecision()
    {
        // Arrange
        var value = 123.4567m;
        var expected = "123.4567";

        // Act
        var result = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Write_WithZero_ShouldWriteAsZero()
    {
        // Arrange
        var value = 0m;
        var expected = "0";

        // Act
        var result = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123.45", 123.45)]
    [InlineData("0.0001", 0.0001)]
    [InlineData("-999.9999", -999.9999)]
    [InlineData("1000000", 1000000)]
    public void RoundTrip_WithValidValues_ShouldMaintainPrecision(string jsonInput, decimal expectedValue)
    {
        // Act
        var deserialized = JsonSerializer.Deserialize<decimal>(jsonInput, _options);
        var serialized = JsonSerializer.Serialize(deserialized, _options);
        var roundTrip = JsonSerializer.Deserialize<decimal>(serialized, _options);

        // Assert
        Assert.Equal(expectedValue, deserialized);
        Assert.Equal(deserialized, roundTrip);
    }
}

public class NullableDecimalPrecisionConverterTests
{
    private readonly JsonSerializerOptions _options = new() { Converters = { new NullableDecimalPrecisionConverter() } };

    [Fact]
    public void Read_WithValidDecimal_ShouldReturnValue()
    {
        // Arrange
        var json = "123.45";
        var expected = 123.45m;

        // Act
        var result = JsonSerializer.Deserialize<decimal?>(json, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Read_WithNull_ShouldReturnNull()
    {
        // Arrange
        var json = "null";

        // Act
        var result = JsonSerializer.Deserialize<decimal?>(json, _options);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Write_WithValue_ShouldWriteNumber()
    {
        // Arrange
        decimal? value = 123.45m;
        var expected = "123.45";

        // Act
        var result = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Write_WithNull_ShouldWriteNull()
    {
        // Arrange
        decimal? value = null;
        var expected = "null";

        // Act
        var result = JsonSerializer.Serialize(value, _options);

        // Assert
        Assert.Equal(expected, result);
    }
}