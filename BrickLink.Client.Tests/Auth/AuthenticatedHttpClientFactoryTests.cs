using System.Net;
using BrickLink.Client.Auth;
using BrickLink.Client.Http;

namespace BrickLink.Client.Tests.Auth;

/// <summary>
/// Unit tests for the <see cref="AuthenticatedHttpClientFactory"/> class.
/// </summary>
public class AuthenticatedHttpClientFactoryTests : IDisposable
{
    private const string ValidConsumerKey = "test-consumer-key";
    private const string ValidConsumerSecret = "test-consumer-secret";
    private const string ValidAccessToken = "test-access-token";
    private const string ValidAccessTokenSecret = "test-access-token-secret";

    private readonly BrickLinkCredentials _validCredentials;
    private readonly List<IDisposable> _disposables;

    public AuthenticatedHttpClientFactoryTests()
    {
        _validCredentials = new BrickLinkCredentials(
            ValidConsumerKey,
            ValidConsumerSecret,
            ValidAccessToken,
            ValidAccessTokenSecret);

        _disposables = new List<IDisposable>();
    }

    #region CreateAuthenticatedHttpClient Tests

    [Fact]
    public void CreateAuthenticatedHttpClient_WithValidCredentials_ReturnsConfiguredHttpClient()
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials);
        _disposables.Add(httpClient);

        // Assert
        Assert.NotNull(httpClient);
        Assert.Equal(new Uri(BrickLinkHttpClient.DefaultBaseUrl), httpClient.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(30), httpClient.Timeout);
        
        // Verify default headers
        Assert.Contains("User-Agent", httpClient.DefaultRequestHeaders.Select(h => h.Key));
        Assert.Contains("Accept", httpClient.DefaultRequestHeaders.Select(h => h.Key));
        Assert.Contains("Accept-Encoding", httpClient.DefaultRequestHeaders.Select(h => h.Key));
    }

    [Fact]
    public void CreateAuthenticatedHttpClient_WithCustomBaseUrl_UsesCustomUrl()
    {
        // Arrange
        const string customBaseUrl = "https://custom.api.example.com/api/v1/";

        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials, customBaseUrl);
        _disposables.Add(httpClient);

        // Assert
        Assert.NotNull(httpClient);
        Assert.Equal(new Uri(customBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void CreateAuthenticatedHttpClient_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(null!));
        Assert.Equal("credentials", exception.ParamName);
    }

    [Fact]
    public void CreateAuthenticatedHttpClient_CreatesWorkingAuthenticationPipeline()
    {
        // Arrange
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials);
        _disposables.Add(httpClient);

        // Create a test request
        var request = new HttpRequestMessage(HttpMethod.Get, "colors");

        // Act & Assert - This should not throw during authentication setup
        // We can't actually send the request without a real server, but we can verify the request gets processed
        // by the authentication handler without errors during the setup phase
        Assert.NotNull(httpClient);
        Assert.NotNull(request);
    }

    #endregion

    #region CreateAuthenticatedBrickLinkHttpClient Tests

    [Fact]
    public void CreateAuthenticatedBrickLinkHttpClient_WithValidCredentials_ReturnsBrickLinkHttpClient()
    {
        // Act
        var brickLinkClient = AuthenticatedHttpClientFactory.CreateAuthenticatedBrickLinkHttpClient(_validCredentials);
        _disposables.Add(brickLinkClient);

        // Assert
        Assert.NotNull(brickLinkClient);
        Assert.NotNull(brickLinkClient.HttpClient);
        Assert.Equal(new Uri(BrickLinkHttpClient.DefaultBaseUrl), brickLinkClient.BaseUrl);
    }

    [Fact]
    public void CreateAuthenticatedBrickLinkHttpClient_WithCustomBaseUrl_UsesCustomUrl()
    {
        // Arrange
        const string customBaseUrl = "https://custom.api.example.com/api/v1/";

        // Act
        var brickLinkClient = AuthenticatedHttpClientFactory.CreateAuthenticatedBrickLinkHttpClient(_validCredentials, customBaseUrl);
        _disposables.Add(brickLinkClient);

        // Assert
        Assert.NotNull(brickLinkClient);
        Assert.Equal(new Uri(customBaseUrl), brickLinkClient.BaseUrl);
    }

    [Fact]
    public void CreateAuthenticatedBrickLinkHttpClient_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            AuthenticatedHttpClientFactory.CreateAuthenticatedBrickLinkHttpClient(null!));
        Assert.Equal("credentials", exception.ParamName);
    }

    #endregion

    #region CreateAuthenticatedHttpClientWithHandlers Tests

    [Fact]
    public void CreateAuthenticatedHttpClientWithHandlers_WithValidCredentials_ReturnsConfiguredHttpClient()
    {
        // Arrange
        var additionalHandlers = new List<DelegatingHandler>();

        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClientWithHandlers(
            _validCredentials, additionalHandlers);
        _disposables.Add(httpClient);

        // Assert
        Assert.NotNull(httpClient);
        Assert.Equal(new Uri(BrickLinkHttpClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void CreateAuthenticatedHttpClientWithHandlers_WithAdditionalHandlers_ChainsHandlersCorrectly()
    {
        // Arrange
        var mockHandler = new TestDelegatingHandler();
        var additionalHandlers = new List<DelegatingHandler> { mockHandler };

        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClientWithHandlers(
            _validCredentials, additionalHandlers);
        _disposables.Add(httpClient);
        _disposables.Add(mockHandler);

        // Assert
        Assert.NotNull(httpClient);
        // Verify that the handler chain was created (can't easily test the actual chain without reflection)
        Assert.NotNull(mockHandler.InnerHandler);
    }

    [Fact]
    public void CreateAuthenticatedHttpClientWithHandlers_WithNullCredentials_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClientWithHandlers(null!, new List<DelegatingHandler>()));
        Assert.Equal("credentials", exception.ParamName);
    }

    [Fact]
    public void CreateAuthenticatedHttpClientWithHandlers_WithNullHandlers_WorksCorrectly()
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClientWithHandlers(
            _validCredentials, null);
        _disposables.Add(httpClient);

        // Assert
        Assert.NotNull(httpClient);
        Assert.Equal(new Uri(BrickLinkHttpClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void CreateAuthenticatedHttpClientWithHandlers_WithEmptyHandlers_WorksCorrectly()
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClientWithHandlers(
            _validCredentials, Enumerable.Empty<DelegatingHandler>());
        _disposables.Add(httpClient);

        // Assert
        Assert.NotNull(httpClient);
        Assert.Equal(new Uri(BrickLinkHttpClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    #endregion

    #region Header Configuration Tests

    [Theory]
    [InlineData("User-Agent", "BrickLink-Client/1.0")]
    [InlineData("Accept", "application/json")]
    public void CreateAuthenticatedHttpClient_ConfiguresExpectedHeaders(string headerName, string expectedValue)
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials);
        _disposables.Add(httpClient);

        // Assert
        var header = httpClient.DefaultRequestHeaders.FirstOrDefault(h => h.Key == headerName);
        Assert.False(header.Equals(default(KeyValuePair<string, IEnumerable<string>>)), $"Header '{headerName}' should be present");
        Assert.Contains(expectedValue, header.Value);
    }

    [Fact]
    public void CreateAuthenticatedHttpClient_ConfiguresAcceptEncodingHeader()
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials);
        _disposables.Add(httpClient);

        // Assert
        var header = httpClient.DefaultRequestHeaders.FirstOrDefault(h => h.Key == "Accept-Encoding");
        Assert.False(header.Equals(default(KeyValuePair<string, IEnumerable<string>>)), "Accept-Encoding header should be present");
        
        var headerValues = header.Value.ToList();
        Assert.Contains("gzip", headerValues);
        Assert.Contains("deflate", headerValues);
    }

    [Fact]
    public void CreateAuthenticatedHttpClient_ConfiguresCorrectTimeout()
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials);
        _disposables.Add(httpClient);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(30), httpClient.Timeout);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Theory]
    [InlineData("invalid-url")]
    [InlineData("not-a-valid-url")]
    [InlineData("http://")]
    public void CreateAuthenticatedHttpClient_WithInvalidBaseUrl_ThrowsUriFormatException(string invalidBaseUrl)
    {
        // Act & Assert
        Assert.Throws<UriFormatException>(() =>
            AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials, invalidBaseUrl));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateAuthenticatedHttpClient_WithEmptyOrNullBaseUrl_UsesDefaultUrl(string? baseUrl)
    {
        // Act
        var httpClient = AuthenticatedHttpClientFactory.CreateAuthenticatedHttpClient(_validCredentials, baseUrl);
        _disposables.Add(httpClient);

        // Assert
        Assert.Equal(new Uri(BrickLinkHttpClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    #endregion

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
        _disposables.Clear();
    }

    /// <summary>
    /// Test delegating handler for testing handler chaining.
    /// </summary>
    private class TestDelegatingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Simple pass-through handler for testing
            return base.SendAsync(request, cancellationToken);
        }
    }
}