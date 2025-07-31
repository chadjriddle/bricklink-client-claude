using System.Net;
using System.Text;
using BrickLink.Client.Http;
using Xunit;

namespace BrickLink.Client.Tests.Http;

/// <summary>
/// Unit tests for the BrickLinkHttpClient class.
/// </summary>
public class BrickLinkHttpClientTests : IDisposable
{
    private readonly HttpMessageHandler _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly BrickLinkHttpClient _brickLinkClient;

    public BrickLinkHttpClientTests()
    {
        _mockHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHandler);
        _brickLinkClient = new BrickLinkHttpClient(_httpClient, disposeHttpClient: true);
    }

    [Fact]
    public void Constructor_WithDefaultParameters_SetsDefaultBaseUrl()
    {
        // Arrange & Act
        using var client = new BrickLinkHttpClient();

        // Assert
        Assert.Equal(BrickLinkHttpClient.DefaultBaseUrl, client.BaseUrl.ToString());
    }

    [Fact]
    public void Constructor_WithCustomBaseUrl_SetsCustomBaseUrl()
    {
        // Arrange
        const string customBaseUrl = "https://api.example.com/v2/";

        // Act
        using var client = new BrickLinkHttpClient(customBaseUrl);

        // Assert
        Assert.Equal(customBaseUrl, client.BaseUrl.ToString());
    }

    [Fact]
    public void Constructor_WithNullBaseUrl_SetsDefaultBaseUrl()
    {
        // Arrange & Act
        using var client = new BrickLinkHttpClient(baseUrl: null);

        // Assert
        Assert.Equal(BrickLinkHttpClient.DefaultBaseUrl, client.BaseUrl.ToString());
    }

    [Fact]
    public void Constructor_WithEmptyBaseUrl_SetsDefaultBaseUrl()
    {
        // Arrange & Act
        using var client = new BrickLinkHttpClient(string.Empty);

        // Assert
        Assert.Equal(BrickLinkHttpClient.DefaultBaseUrl, client.BaseUrl.ToString());
    }

    [Fact]
    public void Constructor_WithWhitespaceBaseUrl_SetsDefaultBaseUrl()
    {
        // Arrange & Act
        using var client = new BrickLinkHttpClient("   ");

        // Assert
        Assert.Equal(BrickLinkHttpClient.DefaultBaseUrl, client.BaseUrl.ToString());
    }

    [Fact]
    public void Constructor_WithInvalidBaseUrl_ThrowsArgumentException()
    {
        // Arrange
        const string invalidUrl = "not-a-valid-url";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new BrickLinkHttpClient(invalidUrl));
        Assert.Contains("Invalid base URL", exception.Message);
        Assert.Equal("baseUrl", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithHttpClient_AndNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new BrickLinkHttpClient((HttpClient)null!));
        Assert.Equal("httpClient", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithHttpClient_SetsHttpClientProperty()
    {
        // Arrange
        using var httpClient = new HttpClient();

        // Act
        using var client = new BrickLinkHttpClient(httpClient);

        // Assert
        Assert.Same(httpClient, client.HttpClient);
    }

    [Fact]
    public void Constructor_WithHttpClient_WithExistingBaseAddress_KeepsExistingBaseAddress()
    {
        // Arrange
        var existingBaseAddress = new Uri("https://existing.example.com/");
        using var httpClient = new HttpClient { BaseAddress = existingBaseAddress };

        // Act
        using var client = new BrickLinkHttpClient(httpClient, "https://custom.example.com/");

        // Assert
        Assert.Equal(existingBaseAddress, client.HttpClient.BaseAddress);
        Assert.Equal("https://custom.example.com/", client.BaseUrl.ToString());
    }

    [Fact]
    public void Constructor_WithHttpClient_WithoutBaseAddress_SetsBaseAddress()
    {
        // Arrange
        using var httpClient = new HttpClient();
        const string customUrl = "https://custom.example.com/";

        // Act
        using var client = new BrickLinkHttpClient(httpClient, customUrl);

        // Assert
        Assert.Equal(customUrl, client.HttpClient.BaseAddress?.ToString());
        Assert.Equal(customUrl, client.BaseUrl.ToString());
    }

    [Fact]
    public void DefaultBaseUrl_IsCorrectBrickLinkApiUrl()
    {
        // Assert
        Assert.Equal("https://api.bricklink.com/api/store/v1/", BrickLinkHttpClient.DefaultBaseUrl);
    }

    [Fact]
    public async Task GetAsync_WithStringUri_CallsHttpClientGet()
    {
        // Arrange
        const string requestUri = "items/part/3001";
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var response = await _brickLinkClient.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Get, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task GetAsync_WithUri_CallsHttpClientGet()
    {
        // Arrange
        var requestUri = new Uri("items/part/3001", UriKind.Relative);
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var response = await _brickLinkClient.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Get, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task GetAsync_WithNullStringUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.GetAsync((string)null!));
    }

    [Fact]
    public async Task GetAsync_WithEmptyStringUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.GetAsync(string.Empty));
    }

    [Fact]
    public async Task GetAsync_WithNullUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.GetAsync((Uri)null!));
    }

    [Fact]
    public async Task PostAsync_WithStringUri_CallsHttpClientPost()
    {
        // Arrange
        const string requestUri = "items";
        var content = new StringContent("test content");
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var response = await _brickLinkClient.PostAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Post, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task PostAsync_WithUri_CallsHttpClientPost()
    {
        // Arrange
        var requestUri = new Uri("items", UriKind.Relative);
        var content = new StringContent("test content");
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.Created));

        // Act
        var response = await _brickLinkClient.PostAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Post, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task PostAsync_WithNullStringUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.PostAsync((string)null!, null));
    }

    [Fact]
    public async Task PostAsync_WithNullUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.PostAsync((Uri)null!, null));
    }

    [Fact]
    public async Task PutAsync_WithStringUri_CallsHttpClientPut()
    {
        // Arrange
        const string requestUri = "items/123";
        var content = new StringContent("updated content");
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var response = await _brickLinkClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Put, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task PutAsync_WithUri_CallsHttpClientPut()
    {
        // Arrange
        var requestUri = new Uri("items/123", UriKind.Relative);
        var content = new StringContent("updated content");
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var response = await _brickLinkClient.PutAsync(requestUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Put, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task PutAsync_WithNullStringUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.PutAsync((string)null!, null));
    }

    [Fact]
    public async Task PutAsync_WithNullUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.PutAsync((Uri)null!, null));
    }

    [Fact]
    public async Task DeleteAsync_WithStringUri_CallsHttpClientDelete()
    {
        // Arrange
        const string requestUri = "items/123";
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act
        var response = await _brickLinkClient.DeleteAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Delete, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task DeleteAsync_WithUri_CallsHttpClientDelete()
    {
        // Arrange
        var requestUri = new Uri("items/123", UriKind.Relative);
        var mockHandler = (MockHttpMessageHandler)_mockHandler;
        mockHandler.SetResponse(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act
        var response = await _brickLinkClient.DeleteAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Single(mockHandler.Requests);
        Assert.Equal(HttpMethod.Delete, mockHandler.Requests[0].Method);
    }

    [Fact]
    public async Task DeleteAsync_WithNullStringUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.DeleteAsync((string)null!));
    }

    [Fact]
    public async Task DeleteAsync_WithNullUri_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _brickLinkClient.DeleteAsync((Uri)null!));
    }

    [Fact]
    public void CreateJsonContent_WithValidString_ReturnsStringContentWithCorrectProperties()
    {
        // Arrange
        const string jsonContent = """{"test": "value"}""";

        // Act
        var result = BrickLinkHttpClient.CreateJsonContent(jsonContent);

        // Assert
        Assert.Equal("application/json", result.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", result.Headers.ContentType?.CharSet);
    }

    [Fact]
    public async Task CreateJsonContent_WithValidString_ContainsCorrectContent()
    {
        // Arrange
        const string jsonContent = """{"test": "value"}""";

        // Act
        var result = BrickLinkHttpClient.CreateJsonContent(jsonContent);
        var actualContent = await result.ReadAsStringAsync();

        // Assert
        Assert.Equal(jsonContent, actualContent);
    }

    [Fact]
    public void CreateJsonContent_WithNullString_ThrowsArgumentNullException()
    {
        // Act & Assert - StringContent throws on null content
        Assert.Throws<ArgumentNullException>(() => BrickLinkHttpClient.CreateJsonContent(null!));
    }

    [Fact]
    public void CreateJsonContent_WithEmptyString_ReturnsValidContent()
    {
        // Act
        var result = BrickLinkHttpClient.CreateJsonContent(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("application/json", result.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", result.Headers.ContentType?.CharSet);
    }

    [Fact]
    public void Dispose_WithDefaultConstructor_DisposesHttpClient()
    {
        // Arrange
        var client = new BrickLinkHttpClient();

        // Act & Assert - Should not throw
        client.Dispose();
    }

    [Fact]
    public async Task Dispose_WithProvidedHttpClient_AndDisposeTrue_DisposesHttpClient()
    {
        // Arrange
        var httpClient = new HttpClient();
        var client = new BrickLinkHttpClient(httpClient, disposeHttpClient: true);

        // Act
        client.Dispose();

        // Assert - HttpClient should be disposed (will throw if we try to use it)
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await httpClient.GetAsync("test"));
    }

    [Fact]
    public void Dispose_WithProvidedHttpClient_AndDisposeFalse_DoesNotDisposeHttpClient()
    {
        // Arrange
        var httpClient = new HttpClient();
        var client = new BrickLinkHttpClient(httpClient, disposeHttpClient: false);

        // Act
        client.Dispose();

        // Assert - HttpClient should still be usable
        Assert.NotNull(httpClient.BaseAddress); // Should not throw

        // Clean up
        httpClient.Dispose();
    }

    [Fact]
    public void HttpClientConfiguration_HasCorrectDefaultHeaders()
    {
        // Arrange & Act
        using var client = new BrickLinkHttpClient();
        var headers = client.HttpClient.DefaultRequestHeaders;

        // Assert
        Assert.Contains(headers, h => h.Key == "User-Agent" && h.Value.Contains("BrickLink.Client/1.0"));
        Assert.Contains(headers, h => h.Key == "Accept" && h.Value.Contains("application/json"));
        Assert.Contains(headers, h => h.Key == "Accept-Charset" && h.Value.Contains("utf-8"));
        Assert.Contains(headers, h => h.Key == "Accept-Encoding" && h.Value.Contains("gzip"));
    }

    [Fact]
    public void HttpClientConfiguration_HasCorrectTimeout()
    {
        // Arrange & Act
        using var client = new BrickLinkHttpClient();

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(30), client.HttpClient.Timeout);
    }

    public void Dispose()
    {
        _brickLinkClient?.Dispose();
        _httpClient?.Dispose();
        _mockHandler?.Dispose();
    }

    /// <summary>
    /// Mock HTTP message handler for testing HTTP client operations.
    /// </summary>
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        private readonly List<HttpRequestMessage> _requests = new();

        public IReadOnlyList<HttpRequestMessage> Requests => _requests.AsReadOnly();

        public void SetResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requests.Add(request);
            return Task.FromResult(_response ?? new HttpResponseMessage(HttpStatusCode.OK));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _response?.Dispose();
                foreach (var request in _requests)
                {
                    request?.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
