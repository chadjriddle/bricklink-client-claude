namespace BrickLink.Client.Auth;

/// <summary>
/// Represents the OAuth 1.0a-like credentials required for BrickLink API authentication.
/// </summary>
public sealed class BrickLinkCredentials
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkCredentials"/> class.
    /// </summary>
    /// <param name="consumerKey">The consumer key obtained from BrickLink developer registration.</param>
    /// <param name="consumerSecret">The consumer secret obtained from BrickLink developer registration.</param>
    /// <param name="accessToken">The access token obtained after IP address registration.</param>
    /// <param name="accessTokenSecret">The access token secret obtained after IP address registration.</param>
    /// <exception cref="ArgumentException">Thrown when any credential parameter is null, empty, or whitespace.</exception>
    public BrickLinkCredentials(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
    {
        ConsumerKey = ValidateCredential(consumerKey, nameof(consumerKey));
        ConsumerSecret = ValidateCredential(consumerSecret, nameof(consumerSecret));
        AccessToken = ValidateCredential(accessToken, nameof(accessToken));
        AccessTokenSecret = ValidateCredential(accessTokenSecret, nameof(accessTokenSecret));
    }

    /// <summary>
    /// Gets the consumer key obtained from BrickLink developer registration.
    /// </summary>
    public string ConsumerKey { get; }

    /// <summary>
    /// Gets the consumer secret obtained from BrickLink developer registration.
    /// </summary>
    public string ConsumerSecret { get; }

    /// <summary>
    /// Gets the access token obtained after IP address registration.
    /// </summary>
    public string AccessToken { get; }

    /// <summary>
    /// Gets the access token secret obtained after IP address registration.
    /// </summary>
    public string AccessTokenSecret { get; }

    /// <summary>
    /// Validates that all credential fields are properly configured with non-empty values.
    /// </summary>
    /// <returns>True if all credentials are valid; otherwise, false.</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ConsumerKey) &&
               !string.IsNullOrWhiteSpace(ConsumerSecret) &&
               !string.IsNullOrWhiteSpace(AccessToken) &&
               !string.IsNullOrWhiteSpace(AccessTokenSecret);
    }

    /// <summary>
    /// Returns a string representation of the credentials with sensitive information redacted.
    /// </summary>
    /// <returns>A string representation suitable for logging and debugging.</returns>
    public override string ToString()
    {
        return $"BrickLinkCredentials {{ ConsumerKey: {RedactSensitiveValue(ConsumerKey)}, " +
               $"ConsumerSecret: {RedactSensitiveValue(ConsumerSecret)}, " +
               $"AccessToken: {RedactSensitiveValue(AccessToken)}, " +
               $"AccessTokenSecret: {RedactSensitiveValue(AccessTokenSecret)} }}";
    }

    /// <summary>
    /// Validates a credential field to ensure it's not null, empty, or whitespace.
    /// </summary>
    /// <param name="value">The credential value to validate.</param>
    /// <param name="parameterName">The name of the parameter for exception messages.</param>
    /// <returns>The validated credential value.</returns>
    /// <exception cref="ArgumentException">Thrown when the credential is null, empty, or whitespace.</exception>
    private static string ValidateCredential(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Credential cannot be null, empty, or whitespace.", parameterName);
        }

        return value;
    }

    /// <summary>
    /// Redacts sensitive information from credential values for safe logging.
    /// Shows first 4 characters followed by asterisks for values longer than 8 characters,
    /// or just asterisks for shorter values.
    /// </summary>
    /// <param name="sensitiveValue">The sensitive value to redact.</param>
    /// <returns>A redacted version of the sensitive value.</returns>
    private static string RedactSensitiveValue(string sensitiveValue)
    {
        if (string.IsNullOrEmpty(sensitiveValue))
        {
            return "****";
        }

        if (sensitiveValue.Length <= 8)
        {
            return new string('*', sensitiveValue.Length);
        }

        return sensitiveValue[..4] + new string('*', sensitiveValue.Length - 4);
    }
}
