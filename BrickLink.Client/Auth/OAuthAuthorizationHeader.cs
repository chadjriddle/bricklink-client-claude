using System.Text;
using System.Text.RegularExpressions;

namespace BrickLink.Client.Auth;

/// <summary>
/// Represents an OAuth 1.0a Authorization header builder and validator.
/// This class provides specialized functionality for constructing and validating
/// OAuth Authorization headers according to RFC 5849.
/// </summary>
public class OAuthAuthorizationHeader
{
    private readonly OAuthParameterCollection _parameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthAuthorizationHeader"/> class.
    /// </summary>
    public OAuthAuthorizationHeader()
    {
        _parameters = new OAuthParameterCollection();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthAuthorizationHeader"/> class
    /// with an existing parameter collection.
    /// </summary>
    /// <param name="parameters">The OAuth parameter collection to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is null.</exception>
    public OAuthAuthorizationHeader(OAuthParameterCollection parameters)
    {
        _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    /// <summary>
    /// Gets the OAuth parameter collection.
    /// </summary>
    public OAuthParameterCollection Parameters => _parameters;

    /// <summary>
    /// Sets the OAuth consumer key parameter.
    /// </summary>
    /// <param name="consumerKey">The OAuth consumer key.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="consumerKey"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithConsumerKey(string consumerKey)
    {
        if (string.IsNullOrWhiteSpace(consumerKey))
            throw new ArgumentNullException(nameof(consumerKey), "Consumer key cannot be null or empty.");

        _parameters.SetConsumerKey(consumerKey);
        return this;
    }

    /// <summary>
    /// Sets the OAuth access token parameter.
    /// </summary>
    /// <param name="accessToken">The OAuth access token.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="accessToken"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithAccessToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentNullException(nameof(accessToken), "Access token cannot be null or empty.");

        _parameters.SetAccessToken(accessToken);
        return this;
    }

    /// <summary>
    /// Sets the OAuth signature method parameter.
    /// </summary>
    /// <param name="signatureMethod">The OAuth signature method (typically "HMAC-SHA1").</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="signatureMethod"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithSignatureMethod(string signatureMethod)
    {
        if (string.IsNullOrWhiteSpace(signatureMethod))
            throw new ArgumentNullException(nameof(signatureMethod), "Signature method cannot be null or empty.");

        _parameters.SetSignatureMethod(signatureMethod);
        return this;
    }

    /// <summary>
    /// Sets the OAuth timestamp parameter.
    /// </summary>
    /// <param name="timestamp">The OAuth timestamp.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="timestamp"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithTimestamp(string timestamp)
    {
        if (string.IsNullOrWhiteSpace(timestamp))
            throw new ArgumentNullException(nameof(timestamp), "Timestamp cannot be null or empty.");

        _parameters.SetTimestamp(timestamp);
        return this;
    }

    /// <summary>
    /// Sets the OAuth nonce parameter.
    /// </summary>
    /// <param name="nonce">The OAuth nonce.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="nonce"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithNonce(string nonce)
    {
        if (string.IsNullOrWhiteSpace(nonce))
            throw new ArgumentNullException(nameof(nonce), "Nonce cannot be null or empty.");

        _parameters.SetNonce(nonce);
        return this;
    }

    /// <summary>
    /// Sets the OAuth version parameter.
    /// </summary>
    /// <param name="version">The OAuth version (typically "1.0").</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="version"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentNullException(nameof(version), "Version cannot be null or empty.");

        _parameters.SetVersion(version);
        return this;
    }

    /// <summary>
    /// Sets the OAuth signature parameter.
    /// </summary>
    /// <param name="signature">The OAuth signature.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="signature"/> is null or empty.</exception>
    public OAuthAuthorizationHeader WithSignature(string signature)
    {
        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentNullException(nameof(signature), "Signature cannot be null or empty.");

        _parameters.SetSignature(signature);
        return this;
    }

    /// <summary>
    /// Builds the complete Authorization header value.
    /// </summary>
    /// <returns>A properly formatted OAuth Authorization header value.</returns>
    public string Build()
    {
        return _parameters.ToAuthorizationHeaderValue();
    }

    /// <summary>
    /// Validates the Authorization header to ensure it contains all required OAuth parameters.
    /// </summary>
    /// <returns>A validation result indicating whether the header is valid and any validation errors.</returns>
    public AuthorizationHeaderValidationResult Validate()
    {
        var errors = new List<string>();

        // Check for required OAuth parameters
        if (!_parameters.ContainsKey("oauth_consumer_key") || string.IsNullOrEmpty(_parameters["oauth_consumer_key"]))
            errors.Add("oauth_consumer_key is required");

        if (!_parameters.ContainsKey("oauth_token") || string.IsNullOrEmpty(_parameters["oauth_token"]))
            errors.Add("oauth_token is required");

        if (!_parameters.ContainsKey("oauth_signature_method") || string.IsNullOrEmpty(_parameters["oauth_signature_method"]))
            errors.Add("oauth_signature_method is required");

        if (!_parameters.ContainsKey("oauth_timestamp") || string.IsNullOrEmpty(_parameters["oauth_timestamp"]))
            errors.Add("oauth_timestamp is required");

        if (!_parameters.ContainsKey("oauth_nonce") || string.IsNullOrEmpty(_parameters["oauth_nonce"]))
            errors.Add("oauth_nonce is required");

        if (!_parameters.ContainsKey("oauth_version") || string.IsNullOrEmpty(_parameters["oauth_version"]))
            errors.Add("oauth_version is required");

        if (!_parameters.ContainsKey("oauth_signature") || string.IsNullOrEmpty(_parameters["oauth_signature"]))
            errors.Add("oauth_signature is required");

        // Validate timestamp format (should be numeric)
        if (_parameters.ContainsKey("oauth_timestamp") && !string.IsNullOrEmpty(_parameters["oauth_timestamp"]))
        {
            if (!IsValidTimestamp(_parameters["oauth_timestamp"]!))
                errors.Add("oauth_timestamp must be a valid Unix timestamp");
        }

        // Validate signature method
        if (_parameters.ContainsKey("oauth_signature_method") && !string.IsNullOrEmpty(_parameters["oauth_signature_method"]))
        {
            var signatureMethod = _parameters["oauth_signature_method"]!;
            if (!IsValidSignatureMethod(signatureMethod))
                errors.Add($"oauth_signature_method '{signatureMethod}' is not supported");
        }

        // Validate version
        if (_parameters.ContainsKey("oauth_version") && !string.IsNullOrEmpty(_parameters["oauth_version"]))
        {
            var version = _parameters["oauth_version"]!;
            if (version != "1.0")
                errors.Add($"oauth_version '{version}' is not supported (must be '1.0')");
        }

        return new AuthorizationHeaderValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Parses an existing Authorization header value into an OAuthAuthorizationHeader instance.
    /// </summary>
    /// <param name="headerValue">The Authorization header value to parse.</param>
    /// <returns>An OAuthAuthorizationHeader instance populated with the parsed parameters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="headerValue"/> is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="headerValue"/> is not a valid OAuth header.</exception>
    public static OAuthAuthorizationHeader Parse(string headerValue)
    {
        if (string.IsNullOrWhiteSpace(headerValue))
            throw new ArgumentNullException(nameof(headerValue), "Header value cannot be null or empty.");

        if (!headerValue.StartsWith("OAuth ", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Header value must start with 'OAuth '", nameof(headerValue));

        var parameters = new OAuthParameterCollection();
        var oauthParams = headerValue.Substring(6); // Remove "OAuth " prefix

        // Parse OAuth parameters using regex to handle quoted values
        var parameterRegex = new Regex(@"(\w+)=""([^""]*)""\s*(?:,\s*)?", RegexOptions.Compiled);
        var matches = parameterRegex.Matches(oauthParams);

        foreach (Match match in matches)
        {
            if (match.Groups.Count == 3)
            {
                var key = match.Groups[1].Value;
                var value = Uri.UnescapeDataString(match.Groups[2].Value);
                parameters.Add(key, value);
            }
        }

        return new OAuthAuthorizationHeader(parameters);
    }

    /// <summary>
    /// Validates whether a timestamp string is a valid Unix timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp string to validate.</param>
    /// <returns>true if the timestamp is valid; otherwise, false.</returns>
    private static bool IsValidTimestamp(string timestamp)
    {
        return long.TryParse(timestamp, out var unixTime) && unixTime > 0;
    }

    /// <summary>
    /// Validates whether a signature method is supported.
    /// </summary>
    /// <param name="signatureMethod">The signature method to validate.</param>
    /// <returns>true if the signature method is supported; otherwise, false.</returns>
    private static bool IsValidSignatureMethod(string signatureMethod)
    {
        // For now, only support HMAC-SHA1 as per BrickLink API requirements
        return string.Equals(signatureMethod, "HMAC-SHA1", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a new instance of the <see cref="OAuthAuthorizationHeader"/> class.
    /// </summary>
    /// <returns>A new OAuthAuthorizationHeader instance.</returns>
    public static OAuthAuthorizationHeader Create()
    {
        return new OAuthAuthorizationHeader();
    }

    /// <summary>
    /// Returns the Authorization header value as a string.
    /// </summary>
    /// <returns>The OAuth Authorization header value.</returns>
    public override string ToString()
    {
        return Build();
    }
}

/// <summary>
/// Represents the result of validating an OAuth Authorization header.
/// </summary>
public class AuthorizationHeaderValidationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationHeaderValidationResult"/> class.
    /// </summary>
    /// <param name="isValid">Whether the header is valid.</param>
    /// <param name="errors">Any validation errors found.</param>
    public AuthorizationHeaderValidationResult(bool isValid, IEnumerable<string> errors)
    {
        IsValid = isValid;
        Errors = errors?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Gets a value indicating whether the Authorization header is valid.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>
    /// Gets a formatted error message containing all validation errors.
    /// </summary>
    public string ErrorMessage => string.Join("; ", Errors);
}
