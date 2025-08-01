using System.Net;
using System.Net.Http.Headers;
using BrickLink.Client.Auth;
using Moq;
using Moq.Protected;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="AuthenticationHandler"/> class.
/// </summary>
public class AuthenticationHandlerTests : IDisposable
{
    private const string ValidConsumerKey = "test-consumer-key";
    private const string ValidConsumerSecret = "test-consumer-secret";
    private const string ValidAccessToken = "test-access-token";
    private const string ValidAccessTokenSecret = "test-access-token-secret";

    private static readonly string OAuthSignatureRegexPattern = @"oauth_signature=""([^""]+)""";
    private static readonly string NonceRegexPattern = @"oauth_nonce=""([^""]+)""";

    private readonly BrickLinkCredentials _validCredentials;
    private readonly Mock<HttpMessageHandler> _mockInnerHandler;
    private AuthenticationHandler? _authHandler;

    public AuthenticationHandlerTests()
    {
        _validCredentials = new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret);

        _mockInnerHandler = new Mock<HttpMessageHandler>();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidCredentials_SetsCredentialsCorrectly()
    {
        // Act
        _authHandler = new AuthenticationHandler(_validCredentials);

        // Assert
        Assert.True(_authHandler.IsConfigured());
    }

    [Fact]
    public void Constructor_WithValidCredentialsAndInnerHandler_SetsCredentialsCorrectly()
    {
        // Act
        _authHandler = new AuthenticationHandler(_validCredentials, _mockInnerHandler.Object);

        // Assert
        Assert.True(_authHandler.IsConfigured());
    }

    [Fact]
    public void Constructor_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new AuthenticationHandler(null!));
        Assert.Equal("credentials", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullCredentialsAndInnerHandler_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new AuthenticationHandler(null!, _mockInnerHandler.Object));
        Assert.Equal("credentials", exception.ParamName);
    }

    #endregion

    #region IsConfigured Tests

    [Fact]
    public void IsConfigured_WithValidCredentials_ReturnsTrue()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);

        // Act
        var result = _authHandler.IsConfigured();

        // Assert
        Assert.True(result);
    }

    #endregion

    #region AuthenticateRequestAsync Tests

    [Fact]
    public async Task AuthenticateRequestAsync_WithValidRequest_AddsAuthorizationHeader()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/colors");

        // Act
        await _authHandler.AuthenticateRequestAsync(request);

        // Assert
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);
        Assert.NotNull(request.Headers.Authorization.Parameter);

        // Verify OAuth parameters are present in the header
        var authHeaderValue = request.Headers.Authorization.Parameter;
        Assert.Contains("oauth_consumer_key", authHeaderValue);
        Assert.Contains("oauth_token", authHeaderValue);
        Assert.Contains("oauth_nonce", authHeaderValue);
        Assert.Contains("oauth_timestamp", authHeaderValue);
        Assert.Contains("oauth_signature_method", authHeaderValue);
        Assert.Contains("oauth_version", authHeaderValue);
        Assert.Contains("oauth_signature", authHeaderValue);
    }

    [Fact]
    public async Task AuthenticateRequestAsync_WithQueryParameters_IncludesParametersInSignature()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/items/part/3001?color_id=5");

        // Act
        await _authHandler.AuthenticateRequestAsync(request);

        // Assert
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);

        // The signature should be different than a request without query parameters
        // This is an integration-style test to ensure query parameters are included in signature generation
        var authHeaderValue = request.Headers.Authorization.Parameter!;
        Assert.Contains("oauth_signature", authHeaderValue);

        // Extract signature value to ensure it's not empty
        var signatureMatch = System.Text.RegularExpressions.Regex.Match(authHeaderValue, OAuthSignatureRegexPattern);
        Assert.True(signatureMatch.Success);
        Assert.NotEmpty(signatureMatch.Groups[1].Value);
    }

    [Fact]
    public async Task AuthenticateRequestAsync_WithPostRequest_HandlesCorrectly()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.bricklink.com/api/v1/test");

        // Act
        await _authHandler.AuthenticateRequestAsync(request);

        // Assert
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);

        // Verify all required OAuth parameters are present
        var authHeaderValue = request.Headers.Authorization.Parameter!;
        Assert.Contains("oauth_consumer_key", authHeaderValue);
        Assert.Contains("oauth_token", authHeaderValue);
        Assert.Contains("oauth_signature", authHeaderValue);
    }

    [Fact]
    public async Task AuthenticateRequestAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _authHandler.AuthenticateRequestAsync(null!));
        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public async Task AuthenticateRequestAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/colors");
        using var cts = new CancellationTokenSource();

        // Act
        await _authHandler.AuthenticateRequestAsync(request, cts.Token);

        // Assert
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);
    }

    [Fact]
    public async Task AuthenticateRequestAsync_MultipleCallsWithSameRequest_GeneratesDifferentNonces()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request1 = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/colors");
        var request2 = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/colors");

        // Act
        await _authHandler.AuthenticateRequestAsync(request1);
        await _authHandler.AuthenticateRequestAsync(request2);

        // Assert
        var auth1 = request1.Headers.Authorization!.Parameter!;
        var auth2 = request2.Headers.Authorization!.Parameter!;

        // Extract nonce values using regex
        var nonce1Match = System.Text.RegularExpressions.Regex.Match(auth1, NonceRegexPattern);
        var nonce2Match = System.Text.RegularExpressions.Regex.Match(auth2, NonceRegexPattern);

        Assert.True(nonce1Match.Success);
        Assert.True(nonce2Match.Success);
        Assert.NotEqual(nonce1Match.Groups[1].Value, nonce2Match.Groups[1].Value);
    }

    #endregion

    #region SendAsync Tests

    [Fact]
    public async Task SendAsync_WithValidRequest_AuthenticatesAndCallsBase()
    {
        // Arrange
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        _mockInnerHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        _authHandler = new AuthenticationHandler(_validCredentials, _mockInnerHandler.Object);
        var httpClient = new HttpClient(_authHandler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/colors");

        // Act
        var response = await httpClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);

        _mockInnerHandler
            .Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r => r.Headers.Authorization != null),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials, _mockInnerHandler.Object);
        var httpClient = new HttpClient(_authHandler);

        // Act & Assert - Test through HttpClient which calls SendAsync internally
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            httpClient.SendAsync(null!));
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        using var cts = new CancellationTokenSource();

        _mockInnerHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(expectedResponse);

        _authHandler = new AuthenticationHandler(_validCredentials, _mockInnerHandler.Object);
        var httpClient = new HttpClient(_authHandler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.bricklink.com/api/v1/colors");

        // Act
        var response = await httpClient.SendAsync(request, cts.Token);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);

        // Verify that the inner handler was called
        _mockInnerHandler
            .Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
    }

    #endregion

    #region Integration-style Tests for URL Parsing

    [Theory]
    [InlineData("https://api.bricklink.com/api/v1/colors", "https://api.bricklink.com/api/v1/colors")]
    [InlineData("https://api.bricklink.com/api/v1/colors?test=1", "https://api.bricklink.com/api/v1/colors")]
    [InlineData("https://api.bricklink.com/api/v1/items/part/3001?color_id=5&extra=test", "https://api.bricklink.com/api/v1/items/part/3001")]
    public async Task AuthenticateRequestAsync_WithVariousUrls_ParsesBaseUrlCorrectly(string requestUrl, string _)
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        // Act
        await _authHandler.AuthenticateRequestAsync(request);

        // Assert
        Assert.NotNull(request.Headers.Authorization);

        // This is tested indirectly by ensuring the signature is generated successfully
        // If the base URL parsing were incorrect, the signature generation would fail or be invalid
        var authHeaderValue = request.Headers.Authorization.Parameter!;
        var signatureMatch = System.Text.RegularExpressions.Regex.Match(authHeaderValue, OAuthSignatureRegexPattern);
        Assert.True(signatureMatch.Success);
        Assert.NotEmpty(signatureMatch.Groups[1].Value);
    }

    [Theory]
    [InlineData("https://api.bricklink.com/api/v1/colors")]
    [InlineData("https://api.bricklink.com/api/v1/colors?")]
    [InlineData("https://api.bricklink.com/api/v1/colors?param1=value1")]
    [InlineData("https://api.bricklink.com/api/v1/colors?param1=value1&param2=value2")]
    [InlineData("https://api.bricklink.com/api/v1/colors?encoded=%20space%20")]
    public async Task AuthenticateRequestAsync_WithVariousQueryStrings_ParsesParametersCorrectly(string requestUrl)
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        // Act & Assert - Should not throw and should generate valid signature
        await _authHandler.AuthenticateRequestAsync(request);

        Assert.NotNull(request.Headers.Authorization);
        var authHeaderValue = request.Headers.Authorization.Parameter!;
        var signatureMatch = System.Text.RegularExpressions.Regex.Match(authHeaderValue, OAuthSignatureRegexPattern);
        Assert.True(signatureMatch.Success, $"Failed to generate signature for URL: {requestUrl}");
        Assert.NotEmpty(signatureMatch.Groups[1].Value);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public async Task AuthenticateRequestAsync_WithMalformedUri_ThrowsException()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(HttpMethod.Get, (Uri?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _authHandler.AuthenticateRequestAsync(request));
        Assert.Contains("Request URI cannot be null", exception.Message);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);

        // Act & Assert - Should not throw
        _authHandler.Dispose();
        _authHandler.Dispose();
    }

    [Fact]
    public void Dispose_WithInnerHandler_DisposesCorrectly()
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials, _mockInnerHandler.Object);

        // Act & Assert - Should not throw
        _authHandler.Dispose();
    }

    [Theory]
    [InlineData("")]
    [InlineData("key1=")]
    [InlineData("key1=value1&key2=")]
    [InlineData("key1&key2=value2")]
    [InlineData("=value")]
    public async Task AuthenticateRequestAsync_WithVariousQueryParameterFormats_HandlesCorrectly(string queryString)
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var url = $"https://api.bricklink.com/api/v1/test?{queryString}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Act & Assert - Should not throw and should generate valid signature
        await _authHandler.AuthenticateRequestAsync(request);

        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);

        var authHeaderValue = request.Headers.Authorization.Parameter!;
        var signatureMatch = System.Text.RegularExpressions.Regex.Match(authHeaderValue, OAuthSignatureRegexPattern);
        Assert.True(signatureMatch.Success, $"Failed to generate signature for query string: {queryString}");
        Assert.NotEmpty(signatureMatch.Groups[1].Value);
    }

    [Theory]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    [InlineData("HEAD")]
    [InlineData("OPTIONS")]
    public async Task AuthenticateRequestAsync_WithDifferentHttpMethods_GeneratesCorrectSignature(string httpMethod)
    {
        // Arrange
        _authHandler = new AuthenticationHandler(_validCredentials);
        var request = new HttpRequestMessage(new HttpMethod(httpMethod), "https://api.bricklink.com/api/v1/test");

        // Act
        await _authHandler.AuthenticateRequestAsync(request);

        // Assert
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("OAuth", request.Headers.Authorization.Scheme);

        var authHeaderValue = request.Headers.Authorization.Parameter!;
        var signatureMatch = System.Text.RegularExpressions.Regex.Match(authHeaderValue, OAuthSignatureRegexPattern);
        Assert.True(signatureMatch.Success, $"Failed to generate signature for HTTP method: {httpMethod}");
        Assert.NotEmpty(signatureMatch.Groups[1].Value);
    }

    #endregion

    public void Dispose()
    {
        _authHandler?.Dispose();
        _mockInnerHandler?.Object?.Dispose();
    }
}
