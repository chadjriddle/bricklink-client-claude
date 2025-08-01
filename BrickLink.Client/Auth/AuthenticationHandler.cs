namespace BrickLink.Client.Auth;

/// <summary>
/// A DelegatingHandler that automatically applies OAuth 1.0a-like authentication headers to HTTP requests.
/// This handler integrates all OAuth components to provide transparent authentication for BrickLink API calls.
/// </summary>
public class AuthenticationHandler : DelegatingHandler, IAuthenticationHandler
{
    private readonly BrickLinkCredentials _credentials;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class.
    /// </summary>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <exception cref="ArgumentNullException">Thrown when credentials is null.</exception>
    public AuthenticationHandler(BrickLinkCredentials credentials)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHandler"/> class with an inner handler.
    /// </summary>
    /// <param name="credentials">The BrickLink OAuth credentials to use for authentication.</param>
    /// <param name="innerHandler">The inner HTTP message handler.</param>
    /// <exception cref="ArgumentNullException">Thrown when credentials is null.</exception>
    public AuthenticationHandler(BrickLinkCredentials credentials, HttpMessageHandler innerHandler)
        : base(innerHandler)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    /// <summary>
    /// Applies OAuth authentication headers to the specified HTTP request message.
    /// </summary>
    /// <param name="request">The HTTP request message to authenticate.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous authentication operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    public Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return AuthenticateRequestCoreAsync(request, cancellationToken);
    }

    /// <summary>
    /// Validates that the authentication handler has all required credentials.
    /// </summary>
    /// <returns>True if the handler is properly configured; otherwise, false.</returns>
    public bool IsConfigured()
    {
        return _credentials != null;
    }

    /// <summary>
    /// Intercepts HTTP requests and applies OAuth authentication headers before sending them.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the HTTP response message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        await AuthenticateRequestCoreAsync(request, cancellationToken).ConfigureAwait(false);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task AuthenticateRequestCoreAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Extract the base URL (without query parameters)
        var baseUrl = GetBaseUrl(request.RequestUri);

        // Extract query parameters from the URL
        var queryParameters = ExtractQueryParameters(request.RequestUri);

        // Create base OAuth parameters with nonce and timestamp
        var oauthParameters = new OAuthParameterCollection();
        oauthParameters.SetConsumerKey(_credentials.ConsumerKey);
        oauthParameters.SetAccessToken(_credentials.AccessToken);
        oauthParameters.SetNonce(NonceGenerator.Generate());
        oauthParameters.SetTimestamp(TimestampGenerator.Generate());
        oauthParameters.SetSignatureMethod("HMAC-SHA1");
        oauthParameters.SetVersion("1.0");

        // Generate signature and create signed parameter collection
        var signedParameters = OAuthSignatureGenerator.CreateSignedParameters(
            request.Method.Method.ToUpperInvariant(),
            baseUrl,
            oauthParameters,
            queryParameters,
            _credentials.ConsumerSecret,
            _credentials.AccessTokenSecret);

        // Build and set the Authorization header
        var authHeader = new OAuthAuthorizationHeader()
            .WithConsumerKey(signedParameters["oauth_consumer_key"]!)
            .WithAccessToken(signedParameters["oauth_token"]!)
            .WithNonce(signedParameters["oauth_nonce"]!)
            .WithTimestamp(signedParameters["oauth_timestamp"]!)
            .WithSignatureMethod(signedParameters["oauth_signature_method"]!)
            .WithVersion(signedParameters["oauth_version"]!)
            .WithSignature(signedParameters["oauth_signature"]!);

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authHeader.ToString());

        await Task.CompletedTask.ConfigureAwait(false);
    }

    private static string GetBaseUrl(Uri? requestUri)
    {
        if (requestUri == null)
            throw new ArgumentException("Request URI cannot be null");

        // Return the URL without query parameters
        return $"{requestUri.Scheme}://{requestUri.Authority}{requestUri.AbsolutePath}";
    }

    private static IEnumerable<KeyValuePair<string, string>> ExtractQueryParameters(Uri? requestUri)
    {
        if (requestUri == null || string.IsNullOrEmpty(requestUri.Query))
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        var query = requestUri.Query.TrimStart('?');
        var parameters = new List<KeyValuePair<string, string>>();

        foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            var key = Uri.UnescapeDataString(parts[0]);
            var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
            parameters.Add(new KeyValuePair<string, string>(key, value));
        }

        return parameters;
    }

    /// <summary>
    /// Releases the unmanaged resources and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // No additional managed resources to dispose
        }

        base.Dispose(disposing);
    }
}
