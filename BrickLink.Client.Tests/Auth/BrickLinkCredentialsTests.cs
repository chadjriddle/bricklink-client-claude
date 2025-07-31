using BrickLink.Client.Auth;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="BrickLinkCredentials"/> class.
/// </summary>
public class BrickLinkCredentialsTests
{
    private const string ValidConsumerKey = "test-consumer-key";
    private const string ValidConsumerSecret = "test-consumer-secret";
    private const string ValidAccessToken = "test-access-token";
    private const string ValidAccessTokenSecret = "test-access-token-secret";

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidCredentials_SetsPropertiesCorrectly()
    {
        // Act
        var credentials = new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret);

        // Assert
        Assert.Equal(ValidConsumerKey, credentials.ConsumerKey);
        Assert.Equal(ValidConsumerSecret, credentials.ConsumerSecret);
        Assert.Equal(ValidAccessToken, credentials.AccessToken);
        Assert.Equal(ValidAccessTokenSecret, credentials.AccessTokenSecret);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithInvalidConsumerKey_ThrowsArgumentException(string? invalidConsumerKey)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BrickLinkCredentials(
            invalidConsumerKey!,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret));

        Assert.Equal("consumerKey", exception.ParamName);
        Assert.Contains("Credential cannot be null, empty, or whitespace.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithInvalidConsumerSecret_ThrowsArgumentException(string? invalidConsumerSecret)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BrickLinkCredentials(
            ValidConsumerKey,
            invalidConsumerSecret!,
            ValidAccessToken,
            ValidAccessTokenSecret));

        Assert.Equal("consumerSecret", exception.ParamName);
        Assert.Contains("Credential cannot be null, empty, or whitespace.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithInvalidAccessToken_ThrowsArgumentException(string? invalidAccessToken)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            invalidAccessToken!,
            ValidAccessTokenSecret));

        Assert.Equal("accessToken", exception.ParamName);
        Assert.Contains("Credential cannot be null, empty, or whitespace.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithInvalidAccessTokenSecret_ThrowsArgumentException(string? invalidAccessTokenSecret)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            invalidAccessTokenSecret!));

        Assert.Equal("accessTokenSecret", exception.ParamName);
        Assert.Contains("Credential cannot be null, empty, or whitespace.", exception.Message);
    }

    #endregion

    #region IsValid Tests

    [Fact]
    public void IsValid_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        var credentials = new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret);

        // Act
        var result = credentials.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_AfterValidConstruction_AlwaysReturnsTrue()
    {
        // Arrange - Since constructor validates, any successfully constructed instance should be valid
        var credentials = new BrickLinkCredentials("key", "secret", "token", "tokenSecret");

        // Act
        var result = credentials.IsValid();

        // Assert
        Assert.True(result);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_WithValidCredentials_RedactsSensitiveInformation()
    {
        // Arrange
        var credentials = new BrickLinkCredentials(
            "consumerKey123",
            "consumerSecret456",
            "accessToken789",
            "accessTokenSecret012");

        // Act
        var result = credentials.ToString();

        // Assert
        var expected = "BrickLinkCredentials { ConsumerKey: cons**********, " +
                      "ConsumerSecret: cons*************, " +
                      "AccessToken: acce**********, " +
                      "AccessTokenSecret: acce**************** }";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_WithShortCredentials_RedactsWithAsterisks()
    {
        // Arrange
        var credentials = new BrickLinkCredentials("key1", "sec2", "tok3", "ts45");

        // Act
        var result = credentials.ToString();

        // Assert
        Assert.Contains("ConsumerKey: ****", result);
        Assert.Contains("ConsumerSecret: ****", result);
        Assert.Contains("AccessToken: ****", result);
        Assert.Contains("AccessTokenSecret: ****", result);
    }

    [Fact]
    public void ToString_WithEightCharacterCredentials_ShowsFirstFourCharacters()
    {
        // Arrange
        var credentials = new BrickLinkCredentials("12345678", "abcdefgh", "qwertyui", "zxcvbnma");

        // Act
        var result = credentials.ToString();

        // Assert
        Assert.Contains("ConsumerKey: ********", result);
        Assert.Contains("ConsumerSecret: ********", result);
        Assert.Contains("AccessToken: ********", result);
        Assert.Contains("AccessTokenSecret: ********", result);
    }

    [Fact]
    public void ToString_WithLongCredentials_ShowsFirstFourCharactersFollowedByAsterisks()
    {
        // Arrange
        var credentials = new BrickLinkCredentials(
            "1234567890123456",
            "abcdefghijklmnop",
            "qwertyuiopasdfgh",
            "zxcvbnmasdfghjkl");

        // Act
        var result = credentials.ToString();

        // Assert
        Assert.Contains("ConsumerKey: 1234************", result);
        Assert.Contains("ConsumerSecret: abcd************", result);
        Assert.Contains("AccessToken: qwer************", result);
        Assert.Contains("AccessTokenSecret: zxcv************", result);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Properties_AreReadOnly()
    {
        // Arrange
        var credentials = new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret);

        // Act & Assert - Properties should only have getters
        var consumerKeyProperty = typeof(BrickLinkCredentials).GetProperty(nameof(BrickLinkCredentials.ConsumerKey));
        var consumerSecretProperty = typeof(BrickLinkCredentials).GetProperty(nameof(BrickLinkCredentials.ConsumerSecret));
        var accessTokenProperty = typeof(BrickLinkCredentials).GetProperty(nameof(BrickLinkCredentials.AccessToken));
        var accessTokenSecretProperty = typeof(BrickLinkCredentials).GetProperty(nameof(BrickLinkCredentials.AccessTokenSecret));

        Assert.NotNull(consumerKeyProperty);
        Assert.NotNull(consumerSecretProperty);
        Assert.NotNull(accessTokenProperty);
        Assert.NotNull(accessTokenSecretProperty);

        Assert.True(consumerKeyProperty.CanRead);
        Assert.False(consumerKeyProperty.CanWrite);
        Assert.True(consumerSecretProperty.CanRead);
        Assert.False(consumerSecretProperty.CanWrite);
        Assert.True(accessTokenProperty.CanRead);
        Assert.False(accessTokenProperty.CanWrite);
        Assert.True(accessTokenSecretProperty.CanRead);
        Assert.False(accessTokenSecretProperty.CanWrite);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Constructor_WithCredentialsContainingSpecialCharacters_AcceptsValues()
    {
        // Arrange
        var specialConsumerKey = "key!@#$%^&*()";
        var specialConsumerSecret = "secret+={}[]|\\:;\"'<>?,./";
        var specialAccessToken = "token~`";
        var specialAccessTokenSecret = "tokenSecret-_";

        // Act
        var credentials = new BrickLinkCredentials(
            specialConsumerKey,
            specialConsumerSecret,
            specialAccessToken,
            specialAccessTokenSecret);

        // Assert
        Assert.Equal(specialConsumerKey, credentials.ConsumerKey);
        Assert.Equal(specialConsumerSecret, credentials.ConsumerSecret);
        Assert.Equal(specialAccessToken, credentials.AccessToken);
        Assert.Equal(specialAccessTokenSecret, credentials.AccessTokenSecret);
    }

    [Fact]
    public void Constructor_WithUnicodeCredentials_AcceptsValues()
    {
        // Arrange
        var unicodeConsumerKey = "키-🔑";
        var unicodeConsumerSecret = "秘密-🤐";
        var unicodeAccessToken = "令牌-🎫";
        var unicodeAccessTokenSecret = "秘密令牌-🔐";

        // Act
        var credentials = new BrickLinkCredentials(
            unicodeConsumerKey,
            unicodeConsumerSecret,
            unicodeAccessToken,
            unicodeAccessTokenSecret);

        // Assert
        Assert.Equal(unicodeConsumerKey, credentials.ConsumerKey);
        Assert.Equal(unicodeConsumerSecret, credentials.ConsumerSecret);
        Assert.Equal(unicodeAccessToken, credentials.AccessToken);
        Assert.Equal(unicodeAccessTokenSecret, credentials.AccessTokenSecret);
    }

    [Fact]
    public void ToString_WithUnicodeCredentials_RedactsCorrectly()
    {
        // Arrange
        var credentials = new BrickLinkCredentials(
            "키-🔑-test",
            "秘密-🤐-test",
            "令牌-🎫-test",
            "秘密令牌-🔐-test");

        // Act
        var result = credentials.ToString();

        // Assert
        Assert.Contains("BrickLinkCredentials", result);
        // Should show first 4 characters followed by asterisks
        Assert.DoesNotContain("키-🔑-test", result);
        Assert.DoesNotContain("秘密-🤐-test", result);
        Assert.DoesNotContain("令牌-🎫-test", result);
        Assert.DoesNotContain("秘密령牌-🔐-test", result);
    }

    #endregion

    #region Security Tests

    [Fact]
    public void ToString_NeverExposesFullSecrets()
    {
        // Arrange
        var credentials = new BrickLinkCredentials(
            "veryLongConsumerKeyThatShouldBeRedacted",
            "veryLongConsumerSecretThatShouldBeRedacted",
            "veryLongAccessTokenThatShouldBeRedacted",
            "veryLongAccessTokenSecretThatShouldBeRedacted");

        // Act
        var result = credentials.ToString();

        // Assert - Full secrets should never appear in ToString output
        Assert.DoesNotContain("veryLongConsumerKeyThatShouldBeRedacted", result);
        Assert.DoesNotContain("veryLongConsumerSecretThatShouldBeRedacted", result);
        Assert.DoesNotContain("veryLongAccessTokenThatShouldBeRedacted", result);
        Assert.DoesNotContain("veryLongAccessTokenSecretThatShouldBeRedacted", result);

        // Should contain redacted versions
        Assert.Contains("very****************************", result);
    }

    #endregion
}
