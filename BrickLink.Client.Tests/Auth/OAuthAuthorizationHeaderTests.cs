using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="OAuthAuthorizationHeader"/> class.
/// </summary>
public class OAuthAuthorizationHeaderTests
{
    [Fact]
    public void Constructor_WithoutParameters_InitializesEmptyHeader()
    {
        // Act
        var header = new OAuthAuthorizationHeader();

        // Assert
        Assert.NotNull(header.Parameters);
        Assert.Equal(0, header.Parameters.Count);
    }

    [Fact]
    public void Constructor_WithParameters_UsesProvidedParameters()
    {
        // Arrange
        var parameters = new OAuthParameterCollection();
        parameters.SetConsumerKey("test_key");

        // Act
        var header = new OAuthAuthorizationHeader(parameters);

        // Assert
        Assert.Same(parameters, header.Parameters);
        Assert.Equal("test_key", header.Parameters["oauth_consumer_key"]);
    }

    [Fact]
    public void Constructor_WithNullParameters_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new OAuthAuthorizationHeader(null!));

        Assert.Equal("parameters", exception.ParamName);
    }

    [Fact]
    public void WithConsumerKey_ValidKey_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithConsumerKey("consumer_key");

        // Assert
        Assert.Same(header, result); // Should return the same instance for chaining
        Assert.Equal("consumer_key", header.Parameters["oauth_consumer_key"]);
    }

    [Fact]
    public void WithConsumerKey_NullKey_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithConsumerKey(null!));

        Assert.Equal("consumerKey", exception.ParamName);
        Assert.Contains("Consumer key cannot be null", exception.Message);
    }

    [Fact]
    public void WithConsumerKey_EmptyKey_ThrowsArgumentException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            header.WithConsumerKey(""));

        Assert.Equal("consumerKey", exception.ParamName);
        Assert.Contains("Consumer key cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void WithAccessToken_ValidToken_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithAccessToken("access_token");

        // Assert
        Assert.Same(header, result);
        Assert.Equal("access_token", header.Parameters["oauth_token"]);
    }

    [Fact]
    public void WithAccessToken_NullToken_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithAccessToken(null!));

        Assert.Equal("accessToken", exception.ParamName);
        Assert.Contains("Access token cannot be null", exception.Message);
    }

    [Fact]
    public void WithSignatureMethod_ValidMethod_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithSignatureMethod("HMAC-SHA1");

        // Assert
        Assert.Same(header, result);
        Assert.Equal("HMAC-SHA1", header.Parameters["oauth_signature_method"]);
    }

    [Fact]
    public void WithSignatureMethod_NullMethod_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithSignatureMethod(null!));

        Assert.Equal("signatureMethod", exception.ParamName);
        Assert.Contains("Signature method cannot be null", exception.Message);
    }

    [Fact]
    public void WithTimestamp_ValidTimestamp_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithTimestamp("1234567890");

        // Assert
        Assert.Same(header, result);
        Assert.Equal("1234567890", header.Parameters["oauth_timestamp"]);
    }

    [Fact]
    public void WithTimestamp_NullTimestamp_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithTimestamp(null!));

        Assert.Equal("timestamp", exception.ParamName);
        Assert.Contains("Timestamp cannot be null", exception.Message);
    }

    [Fact]
    public void WithNonce_ValidNonce_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithNonce("random_nonce");

        // Assert
        Assert.Same(header, result);
        Assert.Equal("random_nonce", header.Parameters["oauth_nonce"]);
    }

    [Fact]
    public void WithNonce_NullNonce_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithNonce(null!));

        Assert.Equal("nonce", exception.ParamName);
        Assert.Contains("Nonce cannot be null", exception.Message);
    }

    [Fact]
    public void WithVersion_ValidVersion_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithVersion("1.0");

        // Assert
        Assert.Same(header, result);
        Assert.Equal("1.0", header.Parameters["oauth_version"]);
    }

    [Fact]
    public void WithVersion_NullVersion_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithVersion(null!));

        Assert.Equal("version", exception.ParamName);
        Assert.Contains("Version cannot be null", exception.Message);
    }

    [Fact]
    public void WithSignature_ValidSignature_SetsParameterAndReturnsInstance()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.WithSignature("signature_value");

        // Assert
        Assert.Same(header, result);
        Assert.Equal("signature_value", header.Parameters["oauth_signature"]);
    }

    [Fact]
    public void WithSignature_NullSignature_ThrowsArgumentNullException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            header.WithSignature(null!));

        Assert.Equal("signature", exception.ParamName);
        Assert.Contains("Signature cannot be null", exception.Message);
    }

    [Fact]
    public void Build_WithAllParameters_ReturnsFormattedHeader()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Build();

        // Assert
        Assert.StartsWith("OAuth ", result);
        Assert.Contains("oauth_consumer_key=\"consumer_key\"", result);
        Assert.Contains("oauth_token=\"access_token\"", result);
        Assert.Contains("oauth_signature_method=\"HMAC-SHA1\"", result);
        Assert.Contains("oauth_timestamp=\"1234567890\"", result);
        Assert.Contains("oauth_nonce=\"random_nonce\"", result);
        Assert.Contains("oauth_version=\"1.0\"", result);
        Assert.Contains("oauth_signature=\"signature_value\"", result);
    }

    [Fact]
    public void Build_EmptyHeader_ReturnsOAuthPrefix()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act
        var result = header.Build();

        // Assert
        Assert.Equal("OAuth ", result);
    }

    [Fact]
    public void Validate_WithAllRequiredParameters_ReturnsValid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal("", result.ErrorMessage);
    }

    [Fact]
    public void Validate_MissingConsumerKey_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_consumer_key is required", result.Errors);
    }

    [Fact]
    public void Validate_MissingAccessToken_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_token is required", result.Errors);
    }

    [Fact]
    public void Validate_MissingSignatureMethod_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_signature_method is required", result.Errors);
    }

    [Fact]
    public void Validate_MissingTimestamp_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_timestamp is required", result.Errors);
    }

    [Fact]
    public void Validate_MissingNonce_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_nonce is required", result.Errors);
    }

    [Fact]
    public void Validate_MissingVersion_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_version is required", result.Errors);
    }

    [Fact]
    public void Validate_MissingSignature_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_signature is required", result.Errors);
    }

    [Fact]
    public void Validate_InvalidTimestamp_NonNumeric_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("invalid_timestamp")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_timestamp must be a valid Unix timestamp", result.Errors);
    }

    [Fact]
    public void Validate_InvalidTimestamp_Zero_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("0")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_timestamp must be a valid Unix timestamp", result.Errors);
    }

    [Fact]
    public void Validate_InvalidSignatureMethod_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("PLAINTEXT")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_signature_method 'PLAINTEXT' is not supported", result.Errors);
    }

    [Fact]
    public void Validate_InvalidVersion_ReturnsInvalid()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("2.0")
            .WithSignature("signature_value");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("oauth_version '2.0' is not supported (must be '1.0')", result.Errors);
    }

    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithSignatureMethod("PLAINTEXT")
            .WithTimestamp("invalid")
            .WithVersion("2.0");

        // Act
        var result = header.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(7, result.Errors.Count); // 4 missing + 3 invalid
        Assert.Contains("oauth_consumer_key is required", result.Errors);
        Assert.Contains("oauth_token is required", result.Errors);
        Assert.Contains("oauth_nonce is required", result.Errors);
        Assert.Contains("oauth_signature is required", result.Errors);
        Assert.Contains("oauth_timestamp must be a valid Unix timestamp", result.Errors);
        Assert.Contains("oauth_signature_method 'PLAINTEXT' is not supported", result.Errors);
        Assert.Contains("oauth_version '2.0' is not supported (must be '1.0')", result.Errors);
    }

    [Fact]
    public void Parse_ValidHeaderValue_ReturnsPopulatedHeader()
    {
        // Arrange
        var headerValue = "OAuth oauth_consumer_key=\"consumer_key\", oauth_nonce=\"random_nonce\", oauth_signature=\"signature%3Dvalue\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"1234567890\", oauth_token=\"access_token\", oauth_version=\"1.0\"";

        // Act
        var header = OAuthAuthorizationHeader.Parse(headerValue);

        // Assert
        Assert.Equal("consumer_key", header.Parameters["oauth_consumer_key"]);
        Assert.Equal("random_nonce", header.Parameters["oauth_nonce"]);
        Assert.Equal("signature=value", header.Parameters["oauth_signature"]); // Should be unescaped
        Assert.Equal("HMAC-SHA1", header.Parameters["oauth_signature_method"]);
        Assert.Equal("1234567890", header.Parameters["oauth_timestamp"]);
        Assert.Equal("access_token", header.Parameters["oauth_token"]);
        Assert.Equal("1.0", header.Parameters["oauth_version"]);
    }

    [Fact]
    public void Parse_HeaderWithSpaces_HandlesCorrectly()
    {
        // Arrange
        var headerValue = "OAuth oauth_consumer_key=\"consumer key\", oauth_signature=\"sig/value\"";

        // Act
        var header = OAuthAuthorizationHeader.Parse(headerValue);

        // Assert
        Assert.Equal("consumer key", header.Parameters["oauth_consumer_key"]);
        Assert.Equal("sig/value", header.Parameters["oauth_signature"]);
    }

    [Fact]
    public void Parse_NullHeaderValue_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            OAuthAuthorizationHeader.Parse(null!));

        Assert.Equal("headerValue", exception.ParamName);
        Assert.Contains("Header value cannot be null", exception.Message);
    }

    [Fact]
    public void Parse_EmptyHeaderValue_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            OAuthAuthorizationHeader.Parse(""));

        Assert.Equal("headerValue", exception.ParamName);
        Assert.Contains("Header value cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Parse_WhitespaceHeaderValue_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            OAuthAuthorizationHeader.Parse("   "));

        Assert.Equal("headerValue", exception.ParamName);
        Assert.Contains("Header value cannot be empty or whitespace", exception.Message);
    }

    [Fact]
    public void Parse_InvalidHeaderValue_MissingOAuthPrefix_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            OAuthAuthorizationHeader.Parse("Bearer token"));

        Assert.Equal("headerValue", exception.ParamName);
        Assert.Contains("Header value must start with 'OAuth '", exception.Message);
    }

    [Fact]
    public void Create_ReturnsNewInstance()
    {
        // Act
        var header = OAuthAuthorizationHeader.Create();

        // Assert
        Assert.NotNull(header);
        Assert.NotNull(header.Parameters);
        Assert.Equal(0, header.Parameters.Count);
    }

    [Fact]
    public void ToString_ReturnsBuiltHeader()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader()
            .WithConsumerKey("consumer_key")
            .WithNonce("nonce");

        // Act
        var result = header.ToString();

        // Assert
        Assert.Equal(header.Build(), result);
    }

    [Fact]
    public void FluentInterface_MethodChaining_WorksCorrectly()
    {
        // Act
        var header = OAuthAuthorizationHeader.Create()
            .WithConsumerKey("consumer_key")
            .WithAccessToken("access_token")
            .WithSignatureMethod("HMAC-SHA1")
            .WithTimestamp("1234567890")
            .WithNonce("random_nonce")
            .WithVersion("1.0")
            .WithSignature("signature_value");

        // Assert
        Assert.Equal("consumer_key", header.Parameters["oauth_consumer_key"]);
        Assert.Equal("access_token", header.Parameters["oauth_token"]);
        Assert.Equal("HMAC-SHA1", header.Parameters["oauth_signature_method"]);
        Assert.Equal("1234567890", header.Parameters["oauth_timestamp"]);
        Assert.Equal("random_nonce", header.Parameters["oauth_nonce"]);
        Assert.Equal("1.0", header.Parameters["oauth_version"]);
        Assert.Equal("signature_value", header.Parameters["oauth_signature"]);
    }

    [Fact]
    public void ValidationResult_Constructor_InitializesCorrectly()
    {
        // Arrange
        var errors = new List<string> { "error1", "error2" };

        // Act
        var result = new AuthorizationHeaderValidationResult(false, errors);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("error1", result.Errors);
        Assert.Contains("error2", result.Errors);
        Assert.Equal("error1; error2", result.ErrorMessage);
    }

    [Fact]
    public void ValidationResult_WithNullErrors_HandlesCorrectly()
    {
        // Act
        var result = new AuthorizationHeaderValidationResult(true, null!);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal("", result.ErrorMessage);
    }

    // Edge case tests
    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void WithMethods_WhitespaceValues_ThrowArgumentException(string whitespaceValue)
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => header.WithConsumerKey(whitespaceValue));
        Assert.Throws<ArgumentException>(() => header.WithAccessToken(whitespaceValue));
        Assert.Throws<ArgumentException>(() => header.WithSignatureMethod(whitespaceValue));
        Assert.Throws<ArgumentException>(() => header.WithTimestamp(whitespaceValue));
        Assert.Throws<ArgumentException>(() => header.WithNonce(whitespaceValue));
        Assert.Throws<ArgumentException>(() => header.WithVersion(whitespaceValue));
        Assert.Throws<ArgumentException>(() => header.WithSignature(whitespaceValue));
    }

    [Fact]
    public void WithMethods_EmptyString_ThrowsArgumentException()
    {
        // Arrange
        var header = new OAuthAuthorizationHeader();
        var emptyString = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => header.WithConsumerKey(emptyString));
        Assert.Throws<ArgumentException>(() => header.WithAccessToken(emptyString));
        Assert.Throws<ArgumentException>(() => header.WithSignatureMethod(emptyString));
        Assert.Throws<ArgumentException>(() => header.WithTimestamp(emptyString));
        Assert.Throws<ArgumentException>(() => header.WithNonce(emptyString));
        Assert.Throws<ArgumentException>(() => header.WithVersion(emptyString));
        Assert.Throws<ArgumentException>(() => header.WithSignature(emptyString));
    }
}
