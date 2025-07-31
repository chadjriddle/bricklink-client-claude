using System.Text;

namespace BrickLink.Client.Http;

/// <summary>
/// Provides a configured HttpClient wrapper specifically designed for BrickLink API operations.
/// Encapsulates SSL/TLS configuration, UTF-8 encoding, base URL handling, and other HTTP-specific settings.
/// </summary>
public sealed class BrickLinkHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    /// <summary>
    /// The base URL for the BrickLink API.
    /// </summary>
    public const string DefaultBaseUrl = "https://api.bricklink.com/api/store/v1/";

    /// <summary>
    /// Gets the base URL being used for API requests.
    /// </summary>
    public Uri BaseUrl { get; }

    /// <summary>
    /// Gets the underlying HttpClient instance.
    /// </summary>
    public HttpClient HttpClient => _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkHttpClient"/> class with default configuration.
    /// </summary>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    public BrickLinkHttpClient(string? baseUrl = null)
        : this(CreateConfiguredHttpClient(), baseUrl, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkHttpClient"/> class with a provided HttpClient.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <param name="disposeHttpClient">Whether to dispose the HttpClient when this instance is disposed.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient is null.</exception>
    public BrickLinkHttpClient(HttpClient httpClient, string? baseUrl = null, bool disposeHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = disposeHttpClient;

        // Validate and set base URL
        var urlToUse = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl;
        if (!Uri.TryCreate(urlToUse, UriKind.Absolute, out var baseUri))
        {
            throw new ArgumentException($"Invalid base URL: {urlToUse}", nameof(baseUrl));
        }

        BaseUrl = baseUri;

        // Configure the HttpClient with our base URL if not already set
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = BaseUrl;
        }
    }

    /// <summary>
    /// Creates a properly configured HttpClient instance for BrickLink API operations.
    /// </summary>
    /// <returns>A configured HttpClient instance.</returns>
    private static HttpClient CreateConfiguredHttpClient()
    {
        var handler = new HttpClientHandler();

        // Configure SSL/TLS settings for secure communication
        if (handler.SupportsAutomaticDecompression)
        {
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
        }

        var httpClient = new HttpClient(handler, disposeHandler: true);

        // Configure default headers and settings
        ConfigureHttpClient(httpClient);

        return httpClient;
    }

    /// <summary>
    /// Configures an HttpClient instance with BrickLink API-specific settings.
    /// </summary>
    /// <param name="httpClient">The HttpClient to configure.</param>
    private static void ConfigureHttpClient(HttpClient httpClient)
    {
        // Set default timeout (30 seconds)
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        // Configure default headers
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "BrickLink.Client/1.0");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Accept-Charset", "utf-8");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

        // Ensure UTF-8 encoding is used for all text content
        var mediaType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
        {
            CharSet = "utf-8"
        };
        httpClient.DefaultRequestHeaders.Accept.Add(mediaType);
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.GetAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken = default)
    {
        if (requestUri == null)
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.GetAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request with JSON content to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.PostAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request with JSON content to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent? content, CancellationToken cancellationToken = default)
    {
        if (requestUri == null)
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.PostAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request with content to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.PutAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request with content to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent? content, CancellationToken cancellationToken = default)
    {
        if (requestUri == null)
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.PutAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.DeleteAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request to the specified URI.
    /// </summary>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when requestUri is null.</exception>
    public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken = default)
    {
        if (requestUri == null)
        {
            throw new ArgumentNullException(nameof(requestUri));
        }

        return _httpClient.DeleteAsync(requestUri, cancellationToken);
    }

    /// <summary>
    /// Creates StringContent with UTF-8 encoding and JSON media type.
    /// </summary>
    /// <param name="content">The string content.</param>
    /// <returns>StringContent configured for JSON with UTF-8 encoding.</returns>
    public static StringContent CreateJsonContent(string content)
    {
        return new StringContent(content, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Releases all resources used by the <see cref="BrickLinkHttpClient"/>.
    /// </summary>
    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient?.Dispose();
        }
    }
}
