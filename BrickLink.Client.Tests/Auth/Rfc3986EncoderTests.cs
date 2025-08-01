using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="Rfc3986Encoder"/> class.
/// </summary>
public class Rfc3986EncoderTests
{
    /// <summary>
    /// Test data for RFC 3986 encoding scenarios.
    /// </summary>
    public static IEnumerable<object[]> EncodingTestData => new[]
    {
        // Basic unreserved characters should not be encoded
        new object[] { "abc", "abc" },
        new object[] { "ABC", "ABC" },
        new object[] { "123", "123" },
        new object[] { "-._~", "-._~" },
        new object[] { "abcABC123-._~", "abcABC123-._~" },
        
        // Reserved characters should be encoded
        new object[] { " ", "%20" }, // Space
        new object[] { "!", "%21" }, // Exclamation
        new object[] { "\"", "%22" }, // Quote
        new object[] { "#", "%23" }, // Hash
        new object[] { "$", "%24" }, // Dollar
        new object[] { "%", "%25" }, // Percent
        new object[] { "&", "%26" }, // Ampersand
        new object[] { "'", "%27" }, // Apostrophe
        new object[] { "(", "%28" }, // Left parenthesis
        new object[] { ")", "%29" }, // Right parenthesis
        new object[] { "*", "%2A" }, // Asterisk
        new object[] { "+", "%2B" }, // Plus
        new object[] { ",", "%2C" }, // Comma
        new object[] { "/", "%2F" }, // Forward slash
        new object[] { ":", "%3A" }, // Colon
        new object[] { ";", "%3B" }, // Semicolon
        new object[] { "=", "%3D" }, // Equals
        new object[] { "?", "%3F" }, // Question mark
        new object[] { "@", "%40" }, // At symbol
        new object[] { "[", "%5B" }, // Left bracket
        new object[] { "\\", "%5C" }, // Backslash
        new object[] { "]", "%5D" }, // Right bracket
        new object[] { "^", "%5E" }, // Caret
        new object[] { "`", "%60" }, // Backtick
        new object[] { "{", "%7B" }, // Left brace
        new object[] { "|", "%7C" }, // Pipe
        new object[] { "}", "%7D" }, // Right brace
        
        // Unicode characters should be UTF-8 encoded
        new object[] { "café", "caf%C3%A9" }, // é = C3 A9 in UTF-8
        new object[] { "naïve", "na%C3%AFve" }, // ï = C3 AF in UTF-8
        new object[] { "resumé", "resum%C3%A9" }, // é = C3 A9 in UTF-8
        new object[] { "🚀", "%F0%9F%9A%80" }, // Rocket emoji in UTF-8
        
        // Mixed content
        new object[] { "hello world!", "hello%20world%21" },
        new object[] { "key=value&other=123", "key%3Dvalue%26other%3D123" },
        new object[] { "user@example.com", "user%40example.com" },
        new object[] { "path/to/resource?param=value", "path%2Fto%2Fresource%3Fparam%3Dvalue" },
        
        // Edge cases
        new object[] { "", "" }, // Empty string
        new object[] { null!, "" }, // Null input
    };

    [Theory]
    [MemberData(nameof(EncodingTestData))]
    public void Encode_WithVariousInputs_ReturnsCorrectlyEncodedString(string? input, string expected)
    {
        // Act
        var result = Rfc3986Encoder.Encode(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Encode_WithAllUnreservedCharacters_ReturnsUnchanged()
    {
        // Arrange - All RFC 3986 unreserved characters
        var unreservedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";

        // Act
        var result = Rfc3986Encoder.Encode(unreservedChars);

        // Assert
        Assert.Equal(unreservedChars, result);
    }

    [Fact]
    public void Encode_WithAllReservedCharacters_ReturnsFullyEncoded()
    {
        // Arrange - Common reserved characters
        var reservedChars = " !\"#$%&'()*+,/:;=?@[\\]^`{|}";
        var expectedEncoded = "%20%21%22%23%24%25%26%27%28%29%2A%2B%2C%2F%3A%3B%3D%3F%40%5B%5C%5D%5E%60%7B%7C%7D";

        // Act
        var result = Rfc3986Encoder.Encode(reservedChars);

        // Assert
        Assert.Equal(expectedEncoded, result);
    }

    [Theory]
    [InlineData("ñandú", "ñandú")] // Spanish characters
    [InlineData("日本語", "日本語")] // Japanese characters
    [InlineData("🎉🎊", "🎉🎊")] // Emojis
    public void Encode_WithUnicodeCharacters_ReturnsUtf8Encoded(string input, string inputForDisplay)
    {
        // Act
        var result = Rfc3986Encoder.Encode(input);

        // Assert
        // Verify the result contains percent-encoded UTF-8 bytes
        Assert.Contains("%", result);
        Assert.True(result.Length > input.Length, $"Encoded '{inputForDisplay}' should be longer than original");

        // Verify all percent-encoded sequences are valid (% followed by two hex digits)
        var percentIndices = result.Select((c, i) => c == '%' ? i : -1).Where(i => i != -1);
        foreach (var index in percentIndices)
        {
            Assert.True(index + 2 < result.Length, "Percent encoding should have two hex characters");
            var hexPart = result.Substring(index + 1, 2);
            Assert.True(hexPart.All(c => char.IsDigit(c) || (c >= 'A' && c <= 'F')),
                "Hex characters should be uppercase");
        }
    }

    [Fact]
    public void NormalizeParameters_WithNullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Rfc3986Encoder.NormalizeParameters(null!).ToList());
    }

    [Fact]
    public void NormalizeParameters_WithEmptyCollection_ReturnsEmptyCollection()
    {
        // Arrange
        var emptyParameters = new List<KeyValuePair<string, string>>();

        // Act
        var result = Rfc3986Encoder.NormalizeParameters(emptyParameters);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void NormalizeParameters_WithSingleParameter_ReturnsEncodedParameter()
    {
        // Arrange
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("key with spaces", "value & symbols")
        };

        // Act
        var result = Rfc3986Encoder.NormalizeParameters(parameters).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("key%20with%20spaces", result[0].Key);
        Assert.Equal("value%20%26%20symbols", result[0].Value);
    }

    [Fact]
    public void NormalizeParameters_WithMultipleParameters_ReturnsSortedAndEncodedParameters()
    {
        // Arrange
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("z_param", "last"),
            new("a param", "first value"),
            new("b&param", "second"),
            new("a param", "another first") // Same key, different value
        };

        // Act
        var result = Rfc3986Encoder.NormalizeParameters(parameters).ToList();

        // Assert
        Assert.Equal(4, result.Count);

        // Should be sorted by encoded key, then by encoded value
        Assert.Equal("a%20param", result[0].Key); // "another first" comes before "first value"
        Assert.Equal("another%20first", result[0].Value);

        Assert.Equal("a%20param", result[1].Key);
        Assert.Equal("first%20value", result[1].Value);

        Assert.Equal("b%26param", result[2].Key);
        Assert.Equal("second", result[2].Value);

        Assert.Equal("z_param", result[3].Key);
        Assert.Equal("last", result[3].Value);
    }

    [Fact]
    public void NormalizeParameters_WithOAuthParameters_ReturnsCorrectlyNormalized()
    {
        // Arrange - typical OAuth parameters
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("oauth_signature_method", "HMAC-SHA1"),
            new("oauth_consumer_key", "consumer123"),
            new("oauth_token", "token456"),
            new("oauth_timestamp", "1234567890"),
            new("oauth_nonce", "random_nonce"),
            new("oauth_version", "1.0")
        };

        // Act
        var result = Rfc3986Encoder.NormalizeParameters(parameters).ToList();

        // Assert
        Assert.Equal(6, result.Count);

        // Verify alphabetical sorting by parameter name
        Assert.Equal("oauth_consumer_key", result[0].Key);
        Assert.Equal("oauth_nonce", result[1].Key);
        Assert.Equal("oauth_signature_method", result[2].Key);
        Assert.Equal("oauth_timestamp", result[3].Key);
        Assert.Equal("oauth_token", result[4].Key);
        Assert.Equal("oauth_version", result[5].Key);

        // Values should not need encoding for this typical case
        Assert.Equal("consumer123", result[0].Value);
        Assert.Equal("random_nonce", result[1].Value);
        Assert.Equal("HMAC-SHA1", result[2].Value);
        Assert.Equal("1234567890", result[3].Value);
        Assert.Equal("token456", result[4].Value);
        Assert.Equal("1.0", result[5].Value);
    }

    [Fact]
    public void SortParameters_WithNullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Rfc3986Encoder.SortParameters(null!).ToList());
    }

    [Fact]
    public void SortParameters_WithEmptyCollection_ReturnsEmptyCollection()
    {
        // Arrange
        var emptyParameters = new List<KeyValuePair<string, string>>();

        // Act
        var result = Rfc3986Encoder.SortParameters(emptyParameters);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SortParameters_WithSingleParameter_ReturnsSameParameter()
    {
        // Arrange
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("single_key", "single_value")
        };

        // Act
        var result = Rfc3986Encoder.SortParameters(parameters).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("single_key", result[0].Key);
        Assert.Equal("single_value", result[0].Value);
    }

    [Fact]
    public void SortParameters_WithMultipleParameters_ReturnsSortedByKeyThenValue()
    {
        // Arrange
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("z", "value1"),
            new("a", "value2"),
            new("a", "value1"), // Same key, different value
            new("b", "value3")
        };

        // Act
        var result = Rfc3986Encoder.SortParameters(parameters).ToList();

        // Assert
        Assert.Equal(4, result.Count);

        // Should be sorted by key first, then by value
        Assert.Equal("a", result[0].Key);
        Assert.Equal("value1", result[0].Value); // "value1" comes before "value2"

        Assert.Equal("a", result[1].Key);
        Assert.Equal("value2", result[1].Value);

        Assert.Equal("b", result[2].Key);
        Assert.Equal("value3", result[2].Value);

        Assert.Equal("z", result[3].Key);
        Assert.Equal("value1", result[3].Value);
    }

    [Fact]
    public void SortParameters_WithSpecialCharacters_UsesBytewiseComparison()
    {
        // Arrange - testing ordinal (byte-wise) sorting
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("Z", "uppercase"), // ASCII 90
            new("a", "lowercase"), // ASCII 97
            new("_", "underscore"), // ASCII 95
            new("1", "digit") // ASCII 49
        };

        // Act
        var result = Rfc3986Encoder.SortParameters(parameters).ToList();

        // Assert
        Assert.Equal(4, result.Count);

        // Ordinal sorting: digits < uppercase < underscore < lowercase
        Assert.Equal("1", result[0].Key);
        Assert.Equal("Z", result[1].Key);
        Assert.Equal("_", result[2].Key);
        Assert.Equal("a", result[3].Key);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("simple", "simple")]
    [InlineData("with spaces", "with%20spaces")]
    [InlineData("special!@#$%^&*()", "special%21%40%23%24%25%5E%26%2A%28%29")]
    public void Encode_ConsistentWithOAuthSpecExamples_ReturnsExpectedResults(string input, string expected)
    {
        // This test verifies our implementation matches OAuth specification examples

        // Act
        var result = Rfc3986Encoder.Encode(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Encode_WithLongString_PerformsEfficiently()
    {
        // Arrange - Create a long string with mixed content
        var longString = string.Concat(Enumerable.Repeat("Hello World! 123 café 🚀", 1000));

        // Act & Assert - Should complete without timeout
        var result = Rfc3986Encoder.Encode(longString);

        Assert.NotNull(result);
        Assert.True(result.Length > longString.Length, "Encoded string should be longer due to encoding");
    }
}
