namespace BrickLink.Client.Auth;

/// <summary>
/// Configuration options for BrickLink OAuth 1.0a authentication.
/// Used for dependency injection and configuration-based credential management.
/// </summary>
public class BrickLinkAuthenticationOptions
{
    /// <summary>
    /// The configuration section name for BrickLink authentication settings.
    /// </summary>
    public const string SectionName = "BrickLinkAuthentication";

    /// <summary>
    /// Gets or sets the OAuth consumer key obtained from BrickLink developer registration.
    /// </summary>
    /// <remarks>
    /// This is the public identifier for your application registered with BrickLink.
    /// </remarks>
    public string ConsumerKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth consumer secret obtained from BrickLink developer registration.
    /// </summary>
    /// <remarks>
    /// This is the private secret for your application. Keep this secure and never expose it in client-side code.
    /// </remarks>
    public string ConsumerSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth access token obtained after IP address registration.
    /// </summary>
    /// <remarks>
    /// This token is obtained after registering your server's IP address with BrickLink.
    /// It identifies your specific application instance.
    /// </remarks>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth access token secret obtained after IP address registration.
    /// </summary>
    /// <remarks>
    /// This is the private secret for your access token. Keep this secure and never expose it in client-side code.
    /// </remarks>
    public string AccessTokenSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for the BrickLink API.
    /// </summary>
    /// <remarks>
    /// If not specified, the default BrickLink API URL will be used.
    /// This can be useful for testing with mock servers or different API versions.
    /// </remarks>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Validates that all required authentication parameters are provided.
    /// </summary>
    /// <returns>True if all required fields are provided; otherwise, false.</returns>
    public bool IsConfigured()
    {
        return !string.IsNullOrWhiteSpace(ConsumerKey)
            && !string.IsNullOrWhiteSpace(ConsumerSecret)
            && !string.IsNullOrWhiteSpace(AccessToken)
            && !string.IsNullOrWhiteSpace(AccessTokenSecret);
    }

    /// <summary>
    /// Creates BrickLinkCredentials from the configuration options.
    /// </summary>
    /// <returns>A BrickLinkCredentials instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required configuration is missing.</exception>
    public BrickLinkCredentials ToCredentials()
    {
        if (!IsConfigured())
        {
            throw new InvalidOperationException(
                "BrickLink authentication options are not fully configured. " +
                "Please ensure ConsumerKey, ConsumerSecret, AccessToken, and AccessTokenSecret are all provided.");
        }

        return new BrickLinkCredentials(ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
    }

    /// <summary>
    /// Returns a string representation of the configuration options with sensitive data redacted.
    /// </summary>
    /// <returns>A string representation with sensitive information masked.</returns>
    public override string ToString()
    {
        return $"BrickLinkAuthenticationOptions {{ " +
               $"ConsumerKey: {MaskSensitiveValue(ConsumerKey)}, " +
               $"ConsumerSecret: {MaskSensitiveValue(ConsumerSecret)}, " +
               $"AccessToken: {MaskSensitiveValue(AccessToken)}, " +
               $"AccessTokenSecret: {MaskSensitiveValue(AccessTokenSecret)}, " +
               $"BaseUrl: {BaseUrl ?? "default"} }}";
    }

    private static string MaskSensitiveValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "<not set>";

        if (value.Length <= 8)
            return new string('*', value.Length);

        return $"{value[..4]}...{value[^4..]}";
    }
}