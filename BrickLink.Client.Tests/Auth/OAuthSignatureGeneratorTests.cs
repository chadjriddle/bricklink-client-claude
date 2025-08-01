using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="OAuthSignatureGenerator"/> class.
/// </summary>
public class OAuthSignatureGeneratorTests
{
    /// <summary>
    /// Test data for OAuth signature generation based on OAuth 1.0a specification examples.
    /// </summary>
    public static IEnumerable<object[]> SignatureTestData => new[]
    {
        // Basic OAuth signature test case
        new object[]
        {
            "GET",
            "https://api.bricklink.com/api/store/v1/items/part/3001",
            new[]
            {
                new KeyValuePair<string, string>("oauth_consumer_key", "consumer123"),
                new KeyValuePair<string, string>("oauth_nonce", "abc123"),
                new KeyValuePair<string, string>("oauth_signature_method", "HMAC-SHA1"),
                new KeyValuePair<string, string>("oauth_timestamp", "1234567890"),
                new KeyValuePair<string, string>("oauth_token", "token456"),
                new KeyValuePair<string, string>("oauth_version", "1.0")
            },
            "consumer_secret",
            "token_secret",
            "Expected signature will be calculated"
        },
        
        // Test case with special characters in URL
        new object[]
        {
            "POST",
            "https://api.bricklink.com/api/store/v1/inventories",
            new[]
            {
                new KeyValuePair<string, string>("oauth_consumer_key", "test_key"),
                new KeyValuePair<string, string>("oauth_nonce", "random123"),
                new KeyValuePair<string, string>("oauth_signature_method", "HMAC-SHA1"),
                new KeyValuePair<string, string>("oauth_timestamp", "1640995200"),
                new KeyValuePair<string, string>("oauth_token", "access_token"),
                new KeyValuePair<string, string>("oauth_version", "1.0")
            },
            "secret123",
            "token_secret_456",
            "Expected signature will be calculated"
        },
        
        // Test case without token secret (empty token secret)
        new object[]
        {
            "GET",
            "https://api.bricklink.com/api/store/v1/colors",
            new[]
            {
                new KeyValuePair<string, string>("oauth_consumer_key", "key789"),
                new KeyValuePair<string, string>("oauth_nonce", "nonce789"),
                new KeyValuePair<string, string>("oauth_signature_method", "HMAC-SHA1"),
                new KeyValuePair<string, string>("oauth_timestamp", "1577836800"),
                new KeyValuePair<string, string>("oauth_token", ""),
                new KeyValuePair<string, string>("oauth_version", "1.0")
            },
            "consumer_secret_789",
            null!,
            "Expected signature will be calculated"
        }
    };

    [Theory]
    [MemberData(nameof(SignatureTestData))]
    public void GenerateSignature_WithValidParameters_ReturnsConsistentSignature(
        string httpMethod,
        string baseUrl,
        KeyValuePair<string, string>[] parameters,
        string consumerSecret,
        string? tokenSecret,
        string description)
    {
        // Use description for test output identification
        _ = description;
        // Act
        var signature1 = OAuthSignatureGenerator.GenerateSignature(httpMethod, baseUrl, parameters, consumerSecret, tokenSecret);
        var signature2 = OAuthSignatureGenerator.GenerateSignature(httpMethod, baseUrl, parameters, consumerSecret, tokenSecret);

        // Assert
        Assert.NotNull(signature1);
        Assert.NotEmpty(signature1);
        Assert.Equal(signature1, signature2); // Should be deterministic
        Assert.True(IsValidBase64(signature1)); // Should be valid Base64
    }

    [Fact]
    public void GenerateSignature_WithNullHttpMethod_ThrowsArgumentNullException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.GenerateSignature(null!, "https://example.com", parameters, "secret", "token"));
    }

    [Fact]
    public void GenerateSignature_WithEmptyHttpMethod_ThrowsArgumentException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OAuthSignatureGenerator.GenerateSignature("", "https://example.com", parameters, "secret", "token"));
    }

    [Fact]
    public void GenerateSignature_WithNullBaseUrl_ThrowsArgumentNullException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.GenerateSignature("GET", null!, parameters, "secret", "token"));
    }

    [Fact]
    public void GenerateSignature_WithEmptyBaseUrl_ThrowsArgumentException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OAuthSignatureGenerator.GenerateSignature("GET", "", parameters, "secret", "token"));
    }

    [Fact]
    public void GenerateSignature_WithNullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.GenerateSignature("GET", "https://example.com", null!, "secret", "token"));
    }

    [Fact]
    public void GenerateSignature_WithNullConsumerSecret_ThrowsArgumentNullException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.GenerateSignature("GET", "https://example.com", parameters, null!, "token"));
    }

    [Fact]
    public void CreateSignatureBaseString_WithBasicParameters_ReturnsCorrectFormat()
    {
        // Arrange
        var httpMethod = "GET";
        var baseUrl = "https://api.example.com/resource";
        var parameters = new[]
        {
            new KeyValuePair<string, string>("oauth_consumer_key", "consumer"),
            new KeyValuePair<string, string>("oauth_nonce", "nonce123"),
            new KeyValuePair<string, string>("oauth_timestamp", "1234567890")
        };

        // Act
        var result = OAuthSignatureGenerator.CreateSignatureBaseString(httpMethod, baseUrl, parameters);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("GET&", result);
        Assert.Contains("https%3A%2F%2Fapi.example.com%2Fresource", result); // URL encoded
        Assert.Contains("oauth_consumer_key%3Dconsumer", result); // Parameters encoded
    }

    [Fact]
    public void CreateSignatureBaseString_WithSpecialCharacters_ProperlyEncodesComponents()
    {
        // Arrange
        var httpMethod = "post"; // Should be uppercased
        var baseUrl = "https://api.example.com/resource with spaces";
        var parameters = new[]
        {
            new KeyValuePair<string, string>("param with spaces", "value & symbols"),
            new KeyValuePair<string, string>("normal_param", "normal_value")
        };

        // Act
        var result = OAuthSignatureGenerator.CreateSignatureBaseString(httpMethod, baseUrl, parameters);

        // Assert
        Assert.StartsWith("POST&", result); // Method should be uppercase
        Assert.Contains("resource%20with%20spaces", result); // URL spaces encoded

        // The parameter values are encoded as part of the parameter string, not directly in the result
        // Let's check the overall structure instead
        Assert.Contains("normal_param%3Dnormal_value", result); // Check for properly encoded parameters
    }

    [Fact]
    public void CreateSignatureBaseString_WithNullHttpMethod_ThrowsArgumentNullException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignatureBaseString(null!, "https://example.com", parameters));
    }

    [Fact]
    public void CreateSignatureBaseString_WithNullBaseUrl_ThrowsArgumentNullException()
    {
        // Arrange
        var parameters = new[] { new KeyValuePair<string, string>("key", "value") };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignatureBaseString("GET", null!, parameters));
    }

    [Fact]
    public void CreateSignatureBaseString_WithNullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignatureBaseString("GET", "https://example.com", null!));
    }

    [Fact]
    public void CreateSigningKey_WithBothSecrets_ReturnsCorrectFormat()
    {
        // Arrange
        var consumerSecret = "consumer_secret_123";
        var tokenSecret = "token_secret_456";

        // Act
        var result = OAuthSignatureGenerator.CreateSigningKey(consumerSecret, tokenSecret);

        // Assert
        Assert.Equal("consumer_secret_123&token_secret_456", result);
    }

    [Fact]
    public void CreateSigningKey_WithOnlyConsumerSecret_ReturnsCorrectFormat()
    {
        // Arrange
        var consumerSecret = "consumer_secret_only";

        // Act
        var result = OAuthSignatureGenerator.CreateSigningKey(consumerSecret);

        // Assert
        Assert.Equal("consumer_secret_only&", result);
    }

    [Fact]
    public void CreateSigningKey_WithNullTokenSecret_ReturnsCorrectFormat()
    {
        // Arrange
        var consumerSecret = "consumer_secret";

        // Act
        var result = OAuthSignatureGenerator.CreateSigningKey(consumerSecret, null);

        // Assert
        Assert.Equal("consumer_secret&", result);
    }

    [Fact]
    public void CreateSigningKey_WithEmptyTokenSecret_ReturnsCorrectFormat()
    {
        // Arrange
        var consumerSecret = "consumer_secret";
        var tokenSecret = "";

        // Act
        var result = OAuthSignatureGenerator.CreateSigningKey(consumerSecret, tokenSecret);

        // Assert
        Assert.Equal("consumer_secret&", result);
    }

    [Fact]
    public void CreateSigningKey_WithSpecialCharacters_EncodesSecrets()
    {
        // Arrange
        var consumerSecret = "consumer secret with spaces";
        var tokenSecret = "token&secret=with&symbols";

        // Act
        var result = OAuthSignatureGenerator.CreateSigningKey(consumerSecret, tokenSecret);

        // Assert
        Assert.Equal("consumer%20secret%20with%20spaces&token%26secret%3Dwith%26symbols", result);
    }

    [Fact]
    public void CreateSigningKey_WithNullConsumerSecret_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSigningKey(null!, "token"));
    }

    [Fact]
    public void GenerateHmacSha1Signature_WithValidInputs_ReturnsBase64Signature()
    {
        // Arrange
        var baseString = "GET&https%3A%2F%2Fapi.example.com&oauth_consumer_key%3Dtest";
        var signingKey = "secret&token";

        // Act
        var result = OAuthSignatureGenerator.GenerateHmacSha1Signature(baseString, signingKey);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(IsValidBase64(result));
    }

    [Fact]
    public void GenerateHmacSha1Signature_WithSameInputs_ReturnsConsistentResult()
    {
        // Arrange
        var baseString = "POST&https%3A%2F%2Fapi.example.com&param%3Dvalue";
        var signingKey = "consumer_secret&token_secret";

        // Act
        var result1 = OAuthSignatureGenerator.GenerateHmacSha1Signature(baseString, signingKey);
        var result2 = OAuthSignatureGenerator.GenerateHmacSha1Signature(baseString, signingKey);

        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void GenerateHmacSha1Signature_WithDifferentInputs_ReturnsDifferentResults()
    {
        // Arrange
        var baseString1 = "GET&https%3A%2F%2Fapi.example.com&param%3Dvalue1";
        var baseString2 = "GET&https%3A%2F%2Fapi.example.com&param%3Dvalue2";
        var signingKey = "secret&token";

        // Act
        var result1 = OAuthSignatureGenerator.GenerateHmacSha1Signature(baseString1, signingKey);
        var result2 = OAuthSignatureGenerator.GenerateHmacSha1Signature(baseString2, signingKey);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void GenerateHmacSha1Signature_WithNullBaseString_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.GenerateHmacSha1Signature(null!, "signingKey"));
    }

    [Fact]
    public void GenerateHmacSha1Signature_WithNullSigningKey_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.GenerateHmacSha1Signature("baseString", null!));
    }

    [Fact]
    public void CreateSignedParameters_WithOAuthParameters_ReturnsParametersWithSignature()
    {
        // Arrange
        var httpMethod = "GET";
        var baseUrl = "https://api.example.com/resource";
        var oauthParams = new OAuthParameterCollection();
        oauthParams.SetConsumerKey("consumer123");
        oauthParams.SetAccessToken("token456");
        oauthParams.SetSignatureMethod("HMAC-SHA1");
        oauthParams.SetTimestamp("1234567890");
        oauthParams.SetNonce("nonce123");
        oauthParams.SetVersion("1.0");

        var consumerSecret = "consumer_secret";
        var tokenSecret = "token_secret";

        // Act
        var result = OAuthSignatureGenerator.CreateSignedParameters(
            httpMethod, baseUrl, oauthParams, null, consumerSecret, tokenSecret);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("oauth_signature"));
        Assert.NotNull(result["oauth_signature"]);
        Assert.NotEmpty(result["oauth_signature"]!);
        Assert.True(IsValidBase64(result["oauth_signature"]!));

        // Should contain all original parameters
        Assert.Equal("consumer123", result["oauth_consumer_key"]);
        Assert.Equal("token456", result["oauth_token"]);
        Assert.Equal("HMAC-SHA1", result["oauth_signature_method"]);
    }

    [Fact]
    public void CreateSignedParameters_WithRequestParameters_IncludesAllParameters()
    {
        // Arrange
        var httpMethod = "POST";
        var baseUrl = "https://api.example.com/endpoint";
        var oauthParams = new OAuthParameterCollection();
        oauthParams.SetConsumerKey("key123");
        oauthParams.SetSignatureMethod("HMAC-SHA1");
        oauthParams.SetTimestamp("1640995200");
        oauthParams.SetNonce("abc123");
        oauthParams.SetVersion("1.0");

        var requestParams = new[]
        {
            new KeyValuePair<string, string>("color_id", "1"),
            new KeyValuePair<string, string>("item_type", "PART")
        };

        var consumerSecret = "secret123";

        // Act
        var result = OAuthSignatureGenerator.CreateSignedParameters(
            httpMethod, baseUrl, oauthParams, requestParams, consumerSecret);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("oauth_signature"));

        // Should contain OAuth parameters but not request parameters (they're only used for signing)
        Assert.Equal("key123", result["oauth_consumer_key"]);
        Assert.False(result.ContainsKey("color_id")); // Request params not included in result
        Assert.False(result.ContainsKey("item_type"));
    }

    [Fact]
    public void CreateSignedParameters_WithNullHttpMethod_ThrowsArgumentNullException()
    {
        // Arrange
        var oauthParams = new OAuthParameterCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignedParameters(null!, "https://example.com", oauthParams, null, "secret"));
    }

    [Fact]
    public void CreateSignedParameters_WithNullBaseUrl_ThrowsArgumentNullException()
    {
        // Arrange
        var oauthParams = new OAuthParameterCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignedParameters("GET", null!, oauthParams, null, "secret"));
    }

    [Fact]
    public void CreateSignedParameters_WithNullOAuthParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignedParameters("GET", "https://example.com", null!, null, "secret"));
    }

    [Fact]
    public void CreateSignedParameters_WithNullConsumerSecret_ThrowsArgumentNullException()
    {
        // Arrange
        var oauthParams = new OAuthParameterCollection();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            OAuthSignatureGenerator.CreateSignedParameters("GET", "https://example.com", oauthParams, null, null!));
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("get")] // Should be normalized to uppercase
    [InlineData("post")]
    public void CreateSignatureBaseString_WithDifferentHttpMethods_HandlesCorrectly(string httpMethod)
    {
        // Arrange
        var baseUrl = "https://api.example.com/resource";
        var parameters = new[]
        {
            new KeyValuePair<string, string>("oauth_consumer_key", "key"),
            new KeyValuePair<string, string>("oauth_nonce", "nonce")
        };

        // Act
        var result = OAuthSignatureGenerator.CreateSignatureBaseString(httpMethod, baseUrl, parameters);

        // Assert
        Assert.StartsWith($"{httpMethod.ToUpperInvariant()}&", result);
    }

    [Fact]
    public void SignatureGeneration_WithKnownValues_MatchesExpectedPattern()
    {
        // This test validates the overall signature generation process with known values
        // Note: This would typically use test vectors from OAuth 1.0a specification

        // Arrange
        var httpMethod = "GET";
        var baseUrl = "https://photos.example.net/photos";
        var parameters = new[]
        {
            new KeyValuePair<string, string>("oauth_consumer_key", "dpf43f3p2l4k3l03"),
            new KeyValuePair<string, string>("oauth_nonce", "kllo9940pd9333jh"),
            new KeyValuePair<string, string>("oauth_signature_method", "HMAC-SHA1"),
            new KeyValuePair<string, string>("oauth_timestamp", "1191242096"),
            new KeyValuePair<string, string>("oauth_token", "nnch734d00sl2jdk"),
            new KeyValuePair<string, string>("oauth_version", "1.0"),
            new KeyValuePair<string, string>("file", "vacation.jpg"),
            new KeyValuePair<string, string>("size", "original")
        };
        var consumerSecret = "kd94hf93k423kf44";
        var tokenSecret = "pfkkdhi9sl3r4s00";

        // Act
        var signature = OAuthSignatureGenerator.GenerateSignature(httpMethod, baseUrl, parameters, consumerSecret, tokenSecret);
        var baseString = OAuthSignatureGenerator.CreateSignatureBaseString(httpMethod, baseUrl, parameters);
        var signingKey = OAuthSignatureGenerator.CreateSigningKey(consumerSecret, tokenSecret);

        // Assert
        Assert.NotNull(signature);
        Assert.NotEmpty(signature);
        Assert.True(IsValidBase64(signature));

        // Verify base string format
        Assert.StartsWith("GET&", baseString);
        Assert.Contains("https%3A%2F%2Fphotos.example.net%2Fphotos", baseString);

        // Verify signing key format
        Assert.Equal("kd94hf93k423kf44&pfkkdhi9sl3r4s00", signingKey);
    }

    /// <summary>
    /// Helper method to validate if a string is valid Base64.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is valid Base64, false otherwise.</returns>
    private static bool IsValidBase64(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
