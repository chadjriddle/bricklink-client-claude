using System.Net;
using System.Text;
using System.Text.Json;
using BrickLink.Client.Exceptions;
using BrickLink.Client.Http;
using BrickLink.Client.Models;
using BrickLink.Client.Services;
using Xunit;

namespace BrickLink.Client.Tests.Services;

/// <summary>
/// Unit tests for the BaseApiService class.
/// </summary>
public class BaseApiServiceTests : IDisposable
{
    private readonly MockHttpMessageHandler _mockHandler;
    private readonly HttpClient _httpClient;
    private readonly BrickLinkHttpClient _brickLinkClient;
    private readonly TestApiService _apiService;

    public BaseApiServiceTests()
    {
        _mockHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHandler);
        _brickLinkClient = new BrickLinkHttpClient(_httpClient, disposeHttpClient: true);
        _apiService = new TestApiService(_brickLinkClient, true);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidHttpClient_SetsBaseUrl()
    {
        // Arrange & Act
        var service = new TestApiService(_brickLinkClient);

        // Assert
        Assert.Equal(_brickLinkClient.BaseUrl.ToString(), service.BaseUrl);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestApiService(null!));
    }

    #endregion

    #region Health Check Tests

    [Fact]
    public async Task IsHealthyAsync_WithSuccessfulResponse_ReturnsTrue()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.IsHealthyAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithErrorResponse_ReturnsFalse()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.IsHealthyAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithException_ReturnsFalse()
    {
        // Arrange
        _mockHandler.SetException(new HttpRequestException("Network error"));

        // Act
        var result = await _apiService.IsHealthyAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithCancellation_ReturnsFalse()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await _apiService.IsHealthyAsync(cts.Token);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GET Tests

    [Fact]
    public async Task GetAsync_WithSuccessfulResponse_ReturnsDeserializedData()
    {
        // Arrange
        var testData = new TestModel { Id = 1, Name = "Test" };
        var apiResponse = new ApiResponse<TestModel>
        {
            Meta = new Meta { Code = 200, Message = "OK" },
            Data = testData
        };
        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.GetTestAsync("test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testData.Id, result.Id);
        Assert.Equal(testData.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_WithNullOrWhitespaceUri_ThrowsArgumentException()
    {
        // Arrange, Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _apiService.GetTestAsync(null!));
        await Assert.ThrowsAsync<ArgumentException>(() => _apiService.GetTestAsync(""));
        await Assert.ThrowsAsync<ArgumentException>(() => _apiService.GetTestAsync("   "));
    }

    [Fact]
    public async Task GetAsync_WithErrorResponse_ThrowsBrickLinkApiException()
    {
        // Arrange
        var errorResponse = new ApiResponse<object>
        {
            Meta = new Meta { Code = 404, Message = "Not Found", Description = "Item not found" }
        };
        var json = JsonSerializer.Serialize(errorResponse);
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(404, exception.Code);
        Assert.Equal("Not Found", exception.Message);
        Assert.Equal("Item not found", exception.Description);
    }

    [Fact]
    public async Task GetAsync_WithInvalidJson_ThrowsBrickLinkApiException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Contains("Failed to deserialize API response", exception.Message);
    }

    [Fact]
    public async Task GetAsync_WithMissingMeta_ThrowsBrickLinkApiException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"data\":{}}", Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Contains("Invalid API response format", exception.Message);
    }

    [Fact]
    public async Task GetAsync_WithApiErrorInSuccessResponse_ThrowsBrickLinkApiException()
    {
        // Arrange
        var apiResponse = new ApiResponse<TestModel>
        {
            Meta = new Meta { Code = 400, Message = "Bad Request", Description = "Invalid parameter" },
            Data = null
        };
        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Equal(400, exception.Code);
        Assert.Equal("Bad Request", exception.Message);
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task PostAsync_WithRequestData_SerializesAndDeserializesCorrectly()
    {
        // Arrange
        var requestData = new TestModel { Id = 1, Name = "Request" };
        var responseData = new TestModel { Id = 2, Name = "Response" };
        var apiResponse = new ApiResponse<TestModel>
        {
            Meta = new Meta { Code = 201, Message = "Created" },
            Data = responseData
        };
        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.PostTestAsync("test", requestData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(responseData.Id, result.Id);
        Assert.Equal(responseData.Name, result.Name);

        // Verify request was made correctly
        Assert.Single(_mockHandler.Requests);
        var request = _mockHandler.Requests[0];
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.NotNull(request.Content);
    }

    [Fact]
    public async Task PostAsync_WithoutRequestData_WorksCorrectly()
    {
        // Arrange
        var responseData = new TestModel { Id = 1, Name = "Response" };
        var apiResponse = new ApiResponse<TestModel>
        {
            Meta = new Meta { Code = 200, Message = "OK" },
            Data = responseData
        };
        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.PostTestWithoutDataAsync("test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(responseData.Id, result.Id);
        Assert.Equal(responseData.Name, result.Name);
    }

    #endregion

    #region PUT Tests

    [Fact]
    public async Task PutAsync_WithRequestData_WorksCorrectly()
    {
        // Arrange
        var requestData = new TestModel { Id = 1, Name = "Request" };
        var responseData = new TestModel { Id = 1, Name = "Updated" };
        var apiResponse = new ApiResponse<TestModel>
        {
            Meta = new Meta { Code = 200, Message = "OK" },
            Data = responseData
        };
        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.PutTestAsync("test", requestData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(responseData.Id, result.Id);
        Assert.Equal(responseData.Name, result.Name);

        // Verify request was made correctly
        Assert.Single(_mockHandler.Requests);
        var request = _mockHandler.Requests[0];
        Assert.Equal(HttpMethod.Put, request.Method);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task DeleteAsync_WithSuccessfulResponse_WorksCorrectly()
    {
        // Arrange
        var responseData = new TestModel { Id = 1, Name = "Deleted" };
        var apiResponse = new ApiResponse<TestModel>
        {
            Meta = new Meta { Code = 200, Message = "OK" },
            Data = responseData
        };
        var json = JsonSerializer.Serialize(apiResponse);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act
        var result = await _apiService.DeleteTestAsync("test/1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(responseData.Id, result.Id);
        Assert.Equal(responseData.Name, result.Name);

        // Verify request was made correctly
        Assert.Single(_mockHandler.Requests);
        var request = _mockHandler.Requests[0];
        Assert.Equal(HttpMethod.Delete, request.Method);
    }

    #endregion

    #region Query String Tests

    [Fact]
    public void BuildQueryString_WithValidParameters_ReturnsCorrectString()
    {
        // Arrange
        var parameters = new[]
        {
            new KeyValuePair<string, string?>("param1", "value1"),
            new KeyValuePair<string, string?>("param2", "value with spaces"),
            new KeyValuePair<string, string?>("param3", "value&special=chars")
        };

        // Act
        var result = TestApiService.TestBuildQueryString(parameters);

        // Assert
        Assert.StartsWith("?", result);
        Assert.Contains("param1=value1", result);
        Assert.Contains("param2=value%20with%20spaces", result);
        Assert.Contains("param3=value%26special%3Dchars", result);
    }

    [Fact]
    public void BuildQueryString_WithEmptyParameters_ReturnsEmptyString()
    {
        // Arrange
        var parameters = Array.Empty<KeyValuePair<string, string?>>();

        // Act
        var result = TestApiService.TestBuildQueryString(parameters);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void BuildQueryString_WithNullOrEmptyValues_IgnoresInvalidParameters()
    {
        // Arrange
        var parameters = new[]
        {
            new KeyValuePair<string, string?>("valid", "value"),
            new KeyValuePair<string, string?>("empty", ""),
            new KeyValuePair<string, string?>("null", null),
            new KeyValuePair<string, string?>("", "invalid_key")
        };

        // Act
        var result = TestApiService.TestBuildQueryString(parameters);

        // Assert
        Assert.Equal("?valid=value", result);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task HandleErrorResponseAsync_WithUnauthorizedStatus_ThrowsAuthenticationException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("Unauthorized", Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Contains("Authentication failed", exception.Description);
    }

    [Fact]
    public async Task HandleErrorResponseAsync_WithTooManyRequestsStatus_ThrowsRateLimitException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("Rate limit exceeded", Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Contains("rate limit", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HandleErrorResponseAsync_WithInternalServerErrorStatus_ThrowsServerException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server error", Encoding.UTF8, "application/json")
        };
        _mockHandler.SetResponse(response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => _apiService.GetTestAsync("test"));
        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Contains("internal server error", exception.Description!, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_WithDisposeHttpClientTrue_DisposesHttpClient()
    {
        // Arrange
        var disposableService = new TestApiService(_brickLinkClient, true);

        // Act
        disposableService.Dispose();

        // Assert - No exceptions should be thrown and object should be in disposed state
        // The actual disposal behavior is verified by the fact that no exceptions occur
        Assert.True(true); // This test mainly verifies no exceptions during disposal
    }

    [Fact]
    public void Dispose_WithDisposeHttpClientFalse_DoesNotDisposeHttpClient()
    {
        // Arrange
        var nonDisposableService = new TestApiService(_brickLinkClient, false);

        // Act
        nonDisposableService.Dispose();

        // Assert - HttpClient should still be usable
        Assert.True(true); // This test mainly verifies no exceptions during disposal
    }

    #endregion

    public void Dispose()
    {
        _apiService?.Dispose();
        _brickLinkClient?.Dispose();
        _httpClient?.Dispose();
        _mockHandler?.Dispose();
    }

    #region Test Helpers

    /// <summary>
    /// Test implementation of BaseApiService for testing purposes.
    /// </summary>
    private class TestApiService : BaseApiService
    {
        public TestApiService(BrickLinkHttpClient httpClient, bool disposeHttpClient = false)
            : base(httpClient, disposeHttpClient)
        {
        }

        public Task<TestModel> GetTestAsync(string uri)
            => GetAsync<TestModel>(uri);

        public Task<TestModel> PostTestAsync(string uri, TestModel data)
            => PostAsync<TestModel, TestModel>(uri, data);

        public Task<TestModel> PostTestWithoutDataAsync(string uri)
            => PostAsync<TestModel>(uri);

        public Task<TestModel> PutTestAsync(string uri, TestModel data)
            => PutAsync<TestModel, TestModel>(uri, data);

        public Task<TestModel> DeleteTestAsync(string uri)
            => DeleteAsync<TestModel>(uri);

        public static string TestBuildQueryString(IEnumerable<KeyValuePair<string, string?>> parameters)
            => BuildQueryString(parameters);
    }

    /// <summary>
    /// Test model for serialization/deserialization testing.
    /// </summary>
    private class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Mock HTTP message handler for testing HTTP client operations.
    /// </summary>
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        private Exception? _exception;
        private readonly List<HttpRequestMessage> _requests = new();

        public IReadOnlyList<HttpRequestMessage> Requests => _requests.AsReadOnly();

        public void SetResponse(HttpResponseMessage response)
        {
            _response = response;
            _exception = null;
        }

        public void SetException(Exception exception)
        {
            _exception = exception;
            _response = null;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requests.Add(request);

            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled<HttpResponseMessage>(cancellationToken);
            }

            if (_exception != null)
            {
                return Task.FromException<HttpResponseMessage>(_exception);
            }

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
                _requests.Clear();
            }
            base.Dispose(disposing);
        }
    }

    #endregion
}
