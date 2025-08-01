using BrickLink.Client.Http;

namespace BrickLink.Client.Auth;

/// <summary>
/// Factory for creating HttpClient instances with OAuth 1.0a authentication pre-configured.
/// This factory integrates the AuthenticationHandler into the HttpClient message pipeline
/// to provide transparent authentication for BrickLink API requests.
/// </summary>
public static class AuthenticatedHttpClientFactory
{
    /// <summary>
    /// Creates a new HttpClient instance with OAuth authentication configured using the provided credentials.
    /// </summary>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <returns>A configured HttpClient instance with authentication and BrickLink-specific settings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when credentials is null.</exception>
    /// <remarks>
    /// The returned HttpClient includes:
    /// - OAuth 1.0a authentication handler with automatic signature generation
    /// - SSL/TLS configuration for secure communication
    /// - UTF-8 encoding and compression support
    /// - Proper base URL configuration for BrickLink API
    /// The caller is responsible for disposing the returned HttpClient.
    /// </remarks>
    public static HttpClient CreateAuthenticatedHttpClient(BrickLinkCredentials credentials, string? baseUrl = null)
    {
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        // Create the authentication handler with the provided credentials
        var authHandler = new AuthenticationHandler(credentials);

        // Create the base HttpClient handler for SSL/TLS and compression
        var httpClientHandler = CreateHttpClientHandler();

        // Chain the authentication handler with the base handler
        authHandler.InnerHandler = httpClientHandler;

        // Create HttpClient with the authentication handler in the pipeline
        var httpClient = new HttpClient(authHandler, disposeHandler: true);

        // Configure the HttpClient with BrickLink-specific settings
        ConfigureHttpClientForBrickLink(httpClient, baseUrl);

        return httpClient;
    }

    /// <summary>
    /// Creates a new BrickLinkHttpClient instance with OAuth authentication pre-configured.
    /// </summary>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <returns>A configured BrickLinkHttpClient instance with authentication.</returns>
    /// <exception cref="ArgumentNullException">Thrown when credentials is null.</exception>
    public static BrickLinkHttpClient CreateAuthenticatedBrickLinkHttpClient(BrickLinkCredentials credentials, string? baseUrl = null)
    {
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        // Create authenticated HttpClient
        var httpClient = CreateAuthenticatedHttpClient(credentials, baseUrl);

        // Wrap in BrickLinkHttpClient with disposal responsibility
        return new BrickLinkHttpClient(httpClient, baseUrl, disposeHttpClient: true);
    }

    /// <summary>
    /// Creates a new HttpClient instance with OAuth authentication and additional delegating handlers.
    /// </summary>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <param name="additionalHandlers">Additional DelegatingHandlers to include in the pipeline (e.g., logging, retry).</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    /// <returns>A configured HttpClient instance with authentication and additional handlers.</returns>
    /// <exception cref="ArgumentNullException">Thrown when credentials is null.</exception>
    /// <remarks>
    /// The handlers are chained in the order provided, with the authentication handler as the outermost handler.
    /// This ensures authentication is applied to all requests, including retries and after logging.
    /// </remarks>
    public static HttpClient CreateAuthenticatedHttpClientWithHandlers(
        BrickLinkCredentials credentials,
        IEnumerable<DelegatingHandler>? additionalHandlers,
        string? baseUrl = null)
    {
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        // Create the authentication handler as the outermost handler
        var authHandler = new AuthenticationHandler(credentials);

        // Chain additional handlers if provided
        DelegatingHandler currentHandler = authHandler;
        foreach (var handler in additionalHandlers ?? Enumerable.Empty<DelegatingHandler>())
        {
            currentHandler.InnerHandler = handler;
            currentHandler = handler;
        }

        // Set the final inner handler to the base HTTP handler
        currentHandler.InnerHandler = CreateHttpClientHandler();

        // Create HttpClient with the complete handler pipeline
        var httpClient = new HttpClient(authHandler, disposeHandler: true);

        // Configure the HttpClient with BrickLink-specific settings
        ConfigureHttpClientForBrickLink(httpClient, baseUrl);

        return httpClient;
    }

    /// <summary>
    /// Creates a configured HttpClientHandler with optimal settings for BrickLink API communication.
    /// </summary>
    /// <returns>A configured HttpClientHandler instance.</returns>
    private static HttpClientHandler CreateHttpClientHandler()
    {
        var handler = new HttpClientHandler();

        // Configure SSL/TLS settings for secure communication
        if (handler.SupportsAutomaticDecompression)
        {
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
        }

        return handler;
    }

    /// <summary>
    /// Configures the HttpClient with BrickLink-specific settings.
    /// </summary>
    /// <param name="httpClient">The HttpClient to configure.</param>
    /// <param name="baseUrl">Optional custom base URL. If null, uses the default BrickLink API URL.</param>
    private static void ConfigureHttpClientForBrickLink(HttpClient httpClient, string? baseUrl)
    {
        // Set base address
        var apiBaseUrl = string.IsNullOrWhiteSpace(baseUrl) 
            ? BrickLinkHttpClient.DefaultBaseUrl 
            : baseUrl;
            
        httpClient.BaseAddress = new Uri(apiBaseUrl);

        // Configure default headers
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "BrickLink-Client/1.0");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

        // Configure timeout (30 seconds default)
        httpClient.Timeout = TimeSpan.FromSeconds(30);
    }
}