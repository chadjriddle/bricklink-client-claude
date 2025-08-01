using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="BrickLinkAuthenticationOptions"/> class.
/// </summary>
public class BrickLinkAuthenticationOptionsTests
{
    private const string ValidConsumerKey = "test-consumer-key";
    private const string ValidConsumerSecret = "test-consumer-secret";
    private const string ValidAccessToken = "test-access-token";
    private const string ValidAccessTokenSecret = "test-access-token-secret";

    #region Constructor and Properties Tests

    [Fact]
    public void Constructor_InitializesPropertiesWithEmptyStrings()
    {
        // Act
        var options = new BrickLinkAuthenticationOptions();

        // Assert
        Assert.Equal(string.Empty, options.ConsumerKey);
        Assert.Equal(string.Empty, options.ConsumerSecret);
        Assert.Equal(string.Empty, options.AccessToken);
        Assert.Equal(string.Empty, options.AccessTokenSecret);
        Assert.Null(options.BaseUrl);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions();
        const string customBaseUrl = "https://custom.api.example.com/api/v1/";

        // Act
        options.ConsumerKey = ValidConsumerKey;
        options.ConsumerSecret = ValidConsumerSecret;
        options.AccessToken = ValidAccessToken;
        options.AccessTokenSecret = ValidAccessTokenSecret;
        options.BaseUrl = customBaseUrl;

        // Assert
        Assert.Equal(ValidConsumerKey, options.ConsumerKey);
        Assert.Equal(ValidConsumerSecret, options.ConsumerSecret);
        Assert.Equal(ValidAccessToken, options.AccessToken);
        Assert.Equal(ValidAccessTokenSecret, options.AccessTokenSecret);
        Assert.Equal(customBaseUrl, options.BaseUrl);
    }

    #endregion

    #region SectionName Tests

    [Fact]
    public void SectionName_HasExpectedValue()
    {
        // Assert
        Assert.Equal("BrickLinkAuthentication", BrickLinkAuthenticationOptions.SectionName);
    }

    #endregion

    #region IsConfigured Tests

    [Fact]
    public void IsConfigured_WithAllValidProperties_ReturnsTrue()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = ValidConsumerKey,
            ConsumerSecret = ValidConsumerSecret,
            AccessToken = ValidAccessToken,
            AccessTokenSecret = ValidAccessTokenSecret
        };

        // Act
        var result = options.IsConfigured();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("", ValidConsumerSecret, ValidAccessToken, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, "", ValidAccessToken, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, ValidConsumerSecret, "", ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, ValidConsumerSecret, ValidAccessToken, "")]
    public void IsConfigured_WithMissingRequiredProperty_ReturnsFalse(
        string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = consumerKey,
            ConsumerSecret = consumerSecret,
            AccessToken = accessToken,
            AccessTokenSecret = accessTokenSecret
        };

        // Act
        var result = options.IsConfigured();

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(null, ValidConsumerSecret, ValidAccessToken, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, null, ValidAccessToken, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, ValidConsumerSecret, null, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, ValidConsumerSecret, ValidAccessToken, null)]
    public void IsConfigured_WithNullRequiredProperty_ReturnsFalse(
        string? consumerKey, string? consumerSecret, string? accessToken, string? accessTokenSecret)
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = consumerKey ?? string.Empty,
            ConsumerSecret = consumerSecret ?? string.Empty,
            AccessToken = accessToken ?? string.Empty,
            AccessTokenSecret = accessTokenSecret ?? string.Empty
        };

        // Act
        var result = options.IsConfigured();

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("   ", ValidConsumerSecret, ValidAccessToken, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, "   ", ValidAccessToken, ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, ValidConsumerSecret, "   ", ValidAccessTokenSecret)]
    [InlineData(ValidConsumerKey, ValidConsumerSecret, ValidAccessToken, "   ")]
    public void IsConfigured_WithWhitespaceRequiredProperty_ReturnsFalse(
        string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = consumerKey,
            ConsumerSecret = consumerSecret,
            AccessToken = accessToken,
            AccessTokenSecret = accessTokenSecret
        };

        // Act
        var result = options.IsConfigured();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsConfigured_WithMissingBaseUrl_ReturnsTrue()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = ValidConsumerKey,
            ConsumerSecret = ValidConsumerSecret,
            AccessToken = ValidAccessToken,
            AccessTokenSecret = ValidAccessTokenSecret,
            BaseUrl = null // BaseUrl is optional
        };

        // Act
        var result = options.IsConfigured();

        // Assert
        Assert.True(result);
    }

    #endregion

    #region ToCredentials Tests

    [Fact]
    public void ToCredentials_WithValidConfiguration_ReturnsCredentials()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = ValidConsumerKey,
            ConsumerSecret = ValidConsumerSecret,
            AccessToken = ValidAccessToken,
            AccessTokenSecret = ValidAccessTokenSecret
        };

        // Act
        var credentials = options.ToCredentials();

        // Assert
        Assert.NotNull(credentials);
        Assert.Equal(ValidConsumerKey, credentials.ConsumerKey);
        Assert.Equal(ValidConsumerSecret, credentials.ConsumerSecret);
        Assert.Equal(ValidAccessToken, credentials.AccessToken);
        Assert.Equal(ValidAccessTokenSecret, credentials.AccessTokenSecret);
    }

    [Fact]
    public void ToCredentials_WithInvalidConfiguration_ThrowsInvalidOperationException()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = ValidConsumerKey,
            ConsumerSecret = "", // Missing required field
            AccessToken = ValidAccessToken,
            AccessTokenSecret = ValidAccessTokenSecret
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => options.ToCredentials());
        Assert.Contains("BrickLink authentication options are not fully configured", exception.Message);
        Assert.Contains("ConsumerKey, ConsumerSecret, AccessToken, and AccessTokenSecret", exception.Message);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithValidConfiguration_MasksSensitiveData()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = "123456789012", // 12 characters
            ConsumerSecret = "abcdefghijklmnop", // 16 characters
            AccessToken = "token123456789", // 14 characters
            AccessTokenSecret = "secret123456789", // 15 characters
            BaseUrl = "https://custom.api.example.com/api/v1/"
        };

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("1234...9012", result); // ConsumerKey masked
        Assert.Contains("abcd...mnop", result); // ConsumerSecret masked
        Assert.Contains("toke...6789", result); // AccessToken masked
        Assert.Contains("secr...6789", result); // AccessTokenSecret masked
        Assert.Contains("https://custom.api.example.com/api/v1/", result); // BaseUrl not masked
    }

    [Fact]
    public void ToString_WithShortValues_MasksCompletely()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = "abc", // 3 characters (≤8)
            ConsumerSecret = "def123", // 6 characters (≤8)
            AccessToken = "xyz", // 3 characters (≤8)
            AccessTokenSecret = "12345678" // 8 characters (≤8)
        };

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("***", result); // ConsumerKey completely masked
        Assert.Contains("******", result); // ConsumerSecret completely masked
        Assert.Contains("***", result); // AccessToken completely masked
        Assert.Contains("********", result); // AccessTokenSecret completely masked
    }

    [Fact]
    public void ToString_WithEmptyValues_ShowsNotSetMessage()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions(); // All empty

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("<not set>", result);
        Assert.Contains("BaseUrl: default", result);
    }

    [Fact]
    public void ToString_WithNullBaseUrl_ShowsDefault()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = ValidConsumerKey,
            ConsumerSecret = ValidConsumerSecret,
            AccessToken = ValidAccessToken,
            AccessTokenSecret = ValidAccessTokenSecret,
            BaseUrl = null
        };

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("BaseUrl: default", result);
    }

    [Fact]
    public void ToString_ContainsAllProperties()
    {
        // Arrange
        var options = new BrickLinkAuthenticationOptions
        {
            ConsumerKey = ValidConsumerKey,
            ConsumerSecret = ValidConsumerSecret,
            AccessToken = ValidAccessToken,
            AccessTokenSecret = ValidAccessTokenSecret,
            BaseUrl = "https://custom.api.example.com/"
        };

        // Act
        var result = options.ToString();

        // Assert
        Assert.Contains("BrickLinkAuthenticationOptions", result);
        Assert.Contains("ConsumerKey:", result);
        Assert.Contains("ConsumerSecret:", result);
        Assert.Contains("AccessToken:", result);
        Assert.Contains("AccessTokenSecret:", result);
        Assert.Contains("BaseUrl:", result);
    }

    #endregion
}
