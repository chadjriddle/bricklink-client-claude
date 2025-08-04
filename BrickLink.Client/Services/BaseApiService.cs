using System.Net;
using System.Text;
using System.Text.Json;
using BrickLink.Client.Exceptions;
using BrickLink.Client.Http;
using BrickLink.Client.Models;
using BrickLink.Client.Serialization;

namespace BrickLink.Client.Services;

/// <summary>
/// Abstract base class for all BrickLink API service implementations.
/// Provides common functionality for HTTP operations, response handling, and error processing.
/// </summary>
public abstract class BaseApiService : IApiService, IDisposable
{
    private readonly BrickLinkHttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly bool _disposeHttpClient;
    private bool _disposed;

    /// <summary>
    /// Gets the base URL for API requests handled by this service.
    /// </summary>
    public virtual string BaseUrl => _httpClient.BaseUrl.ToString();

    /// <summary>
    /// Gets the health check endpoint used for service health verification.
    /// Derived classes can override this to use a different endpoint.
    /// </summary>
    protected virtual string HealthCheckEndpoint => "colors";

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseApiService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for API requests.</param>
    /// <param name="disposeHttpClient">Whether to dispose the HTTP client when this service is disposed. Default is false.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient is null.</exception>
    protected BaseApiService(BrickLinkHttpClient httpClient, bool disposeHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = disposeHttpClient;
        _jsonOptions = JsonSerializerOptionsFactory.CreateOptions();
    }

    /// <summary>
    /// Performs a health check to verify the service can communicate with the API.
    /// This implementation attempts to make a simple GET request to verify connectivity.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous health check operation. Returns true if healthy, false otherwise.</returns>
    public virtual async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Make a simple request to check connectivity
            // Use configurable health check endpoint for flexibility
            using var response = await _httpClient.GetAsync(HealthCheckEndpoint, cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            // Any exception means the service is not healthy
            return false;
        }
    }

    /// <summary>
    /// Sends a GET request to the specified URI and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data to.</typeparam>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when requestUri is null or whitespace.</exception>
    /// <exception cref="BrickLinkApiException">Thrown when an API error occurs.</exception>
    protected async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        ValidateRequestUri(requestUri);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request with JSON content to the specified URI and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object to serialize.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response data to.</typeparam>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="requestData">The data to serialize and send in the request body.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when requestUri is null or whitespace.</exception>
    /// <exception cref="BrickLinkApiException">Thrown when an API error occurs.</exception>
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string requestUri, TRequest requestData, CancellationToken cancellationToken = default)
    {
        ValidateRequestUri(requestUri);

        var jsonContent = SerializeRequest(requestData);
        using var content = BrickLinkHttpClient.CreateJsonContent(jsonContent);
        using var response = await _httpClient.PostAsync(requestUri, content, cancellationToken).ConfigureAwait(false);

        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request to the specified URI and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data to.</typeparam>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when requestUri is null or whitespace.</exception>
    /// <exception cref="BrickLinkApiException">Thrown when an API error occurs.</exception>
    protected async Task<T> PostAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        ValidateRequestUri(requestUri);

        using var response = await _httpClient.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request with JSON content to the specified URI and deserializes the response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object to serialize.</typeparam>
    /// <typeparam name="TResponse">The type to deserialize the response data to.</typeparam>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="requestData">The data to serialize and send in the request body.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when requestUri is null or whitespace.</exception>
    /// <exception cref="BrickLinkApiException">Thrown when an API error occurs.</exception>
    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string requestUri, TRequest requestData, CancellationToken cancellationToken = default)
    {
        ValidateRequestUri(requestUri);

        var jsonContent = SerializeRequest(requestData);
        using var content = BrickLinkHttpClient.CreateJsonContent(jsonContent);
        using var response = await _httpClient.PutAsync(requestUri, content, cancellationToken).ConfigureAwait(false);

        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request to the specified URI and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data to.</typeparam>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    /// <exception cref="ArgumentException">Thrown when requestUri is null or whitespace.</exception>
    /// <exception cref="BrickLinkApiException">Thrown when an API error occurs.</exception>
    protected async Task<T> DeleteAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        ValidateRequestUri(requestUri);

        using var response = await _httpClient.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Constructs a query string from the provided parameters.
    /// </summary>
    /// <param name="parameters">The parameters to include in the query string.</param>
    /// <returns>A formatted query string, or empty string if no parameters are provided.</returns>
    protected static string BuildQueryString(IEnumerable<KeyValuePair<string, string?>> parameters)
    {
        var validParameters = parameters
            .Where(p => !string.IsNullOrEmpty(p.Key) && !string.IsNullOrEmpty(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}")
            .ToList();

        return validParameters.Count > 0 ? "?" + string.Join("&", validParameters) : string.Empty;
    }

    /// <summary>
    /// Validates that the request URI is not null or whitespace.
    /// </summary>
    /// <param name="requestUri">The request URI to validate.</param>
    /// <exception cref="ArgumentException">Thrown when requestUri is null or whitespace.</exception>
    private static void ValidateRequestUri(string requestUri)
    {
        if (string.IsNullOrWhiteSpace(requestUri))
        {
            throw new ArgumentException("Request URI cannot be null or whitespace.", nameof(requestUri));
        }
    }

    /// <summary>
    /// Serializes the request object to JSON.
    /// </summary>
    /// <typeparam name="T">The type of the request object.</typeparam>
    /// <param name="requestData">The request data to serialize.</param>
    /// <returns>The serialized JSON string.</returns>
    /// <exception cref="BrickLinkApiException">Thrown when serialization fails.</exception>
    private string SerializeRequest<T>(T requestData)
    {
        try
        {
            return JsonSerializer.Serialize(requestData, _jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new BrickLinkApiException("Failed to serialize request data to JSON.", HttpStatusCode.BadRequest,
                (int)HttpStatusCode.BadRequest, "Request serialization failed.", ex);
        }
    }

    /// <summary>
    /// Processes the HTTP response and deserializes the content.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response data to.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized response data.</returns>
    /// <exception cref="BrickLinkApiException">Thrown when the response indicates an error or deserialization fails.</exception>
    private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            HandleErrorResponse(response, content);
        }

        try
        {
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);

            if (apiResponse?.Meta == null)
            {
                throw new BrickLinkApiException("Invalid API response format: missing metadata.",
                    response.StatusCode, 0, "The API response does not contain the expected metadata structure.");
            }

            // Check if the API response indicates an error (even with success HTTP status)
            if (apiResponse.Meta.Code < 200 || apiResponse.Meta.Code >= 300)
            {
                throw BrickLinkApiException.FromApiResponse(response.StatusCode, apiResponse.Meta);
            }

            // Ensure data is not null before returning
            if (apiResponse.Data == null)
            {
                throw new BrickLinkApiException("Invalid API response format: missing data.",
                    response.StatusCode, apiResponse.Meta.Code, "The API response does not contain the expected data structure.");
            }

            return apiResponse.Data;
        }
        catch (JsonException ex)
        {
            throw new BrickLinkApiException("Failed to deserialize API response.", response.StatusCode,
                (int)response.StatusCode, "Response deserialization failed.", ex);
        }
    }

    /// <summary>
    /// Handles error responses from the API by parsing error information and throwing appropriate exceptions.
    /// </summary>
    /// <param name="response">The HTTP response message containing the error.</param>
    /// <param name="content">The response content as a string.</param>
    /// <exception cref="BrickLinkApiException">Always throws with appropriate error information.</exception>
    private void HandleErrorResponse(HttpResponseMessage response, string content)
    {
        try
        {
            // Try to parse the error response to get detailed error information
            var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
            if (errorResponse?.Meta != null)
            {
                throw BrickLinkApiException.FromApiResponse(response.StatusCode, errorResponse.Meta);
            }
        }
        catch (JsonException)
        {
            // If we can't parse the error response, fall back to status-based error handling
        }

        // Create appropriate exception based on status code
        var message = $"API request failed with status {(int)response.StatusCode} ({response.StatusCode})";

        throw response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => BrickLinkApiException.CreateAuthenticationError(message),
            HttpStatusCode.NotFound => new BrickLinkApiException(message, response.StatusCode, (int)response.StatusCode, content),
            HttpStatusCode.TooManyRequests => BrickLinkApiException.CreateRateLimitError(),
            HttpStatusCode.InternalServerError => BrickLinkApiException.CreateServerError(message),
            _ => new BrickLinkApiException(message, response.StatusCode, (int)response.StatusCode, content)
        };
    }

    /// <summary>
    /// Releases all resources used by the <see cref="BaseApiService"/>.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            if (_disposeHttpClient)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Releases all resources used by the <see cref="BaseApiService"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
