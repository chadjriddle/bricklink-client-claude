using System.Security.Cryptography;
using System.Text;

namespace BrickLink.Client.Auth;

/// <summary>
/// Generates OAuth 1.0a signatures for API authentication requests.
/// This implementation follows the OAuth 1.0a specification for signature generation.
/// </summary>
public static class OAuthSignatureGenerator
{
    /// <summary>
    /// Generates an OAuth 1.0a signature for the specified request parameters.
    /// </summary>
    /// <param name="httpMethod">The HTTP method (GET, POST, etc.).</param>
    /// <param name="baseUrl">The base URL of the request (without query parameters).</param>
    /// <param name="parameters">The OAuth and request parameters.</param>
    /// <param name="consumerSecret">The OAuth consumer secret.</param>
    /// <param name="tokenSecret">The OAuth token secret (can be null or empty).</param>
    /// <returns>The Base64-encoded OAuth signature.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <exception cref="ArgumentException">Thrown when required parameters are empty.</exception>
    /// <remarks>
    /// This method implements the OAuth 1.0a signature generation algorithm:
    /// 1. Construct the signature base string from HTTP method, URL, and parameters
    /// 2. Create the signing key from consumer secret and token secret
    /// 3. Generate HMAC-SHA1 signature using the base string and signing key
    /// 4. Return the Base64-encoded signature
    /// </remarks>
    public static string GenerateSignature(
        string httpMethod,
        string baseUrl,
        IEnumerable<KeyValuePair<string, string>> parameters,
        string consumerSecret,
        string? tokenSecret = null)
    {
        if (httpMethod == null)
            throw new ArgumentNullException(nameof(httpMethod));
        if (string.IsNullOrWhiteSpace(httpMethod))
            throw new ArgumentException("HTTP method cannot be empty", nameof(httpMethod));
        if (baseUrl == null)
            throw new ArgumentNullException(nameof(baseUrl));
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL cannot be empty", nameof(baseUrl));
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));
        if (consumerSecret == null)
            throw new ArgumentNullException(nameof(consumerSecret));

        // Step 1: Create the signature base string
        var signatureBaseString = CreateSignatureBaseString(httpMethod, baseUrl, parameters);

        // Step 2: Create the signing key
        var signingKey = CreateSigningKey(consumerSecret, tokenSecret);

        // Step 3: Generate HMAC-SHA1 signature
        return GenerateHmacSha1Signature(signatureBaseString, signingKey);
    }

    /// <summary>
    /// Creates the signature base string according to OAuth 1.0a specification.
    /// </summary>
    /// <param name="httpMethod">The HTTP method.</param>
    /// <param name="baseUrl">The base URL.</param>
    /// <param name="parameters">The parameters to include in the signature.</param>
    /// <returns>The signature base string.</returns>
    /// <remarks>
    /// The signature base string format is:
    /// HTTP_METHOD&amp;ENCODED_BASE_URL&amp;ENCODED_PARAMETER_STRING
    /// 
    /// This follows the OAuth 1.0a specification where:
    /// 1. HTTP method is uppercased
    /// 2. Base URL and parameter string are percent-encoded
    /// 3. Components are joined with '&amp;' characters
    /// </remarks>
    public static string CreateSignatureBaseString(
        string httpMethod,
        string baseUrl,
        IEnumerable<KeyValuePair<string, string>> parameters)
    {
        if (httpMethod == null)
            throw new ArgumentNullException(nameof(httpMethod));
        if (baseUrl == null)
            throw new ArgumentNullException(nameof(baseUrl));
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        // 1. HTTP Method (uppercase)
        var method = httpMethod.ToUpperInvariant();

        // 2. Base URL (percent-encoded)
        var encodedBaseUrl = Rfc3986Encoder.Encode(baseUrl);

        // 3. Parameter string (normalized and percent-encoded)
        var normalizedParameters = Rfc3986Encoder.NormalizeParameters(parameters);
        var parameterString = string.Join("&", normalizedParameters.Select(kvp =>
            $"{kvp.Key}={kvp.Value}"));
        var encodedParameterString = Rfc3986Encoder.Encode(parameterString);

        // 4. Combine components with '&'
        return $"{method}&{encodedBaseUrl}&{encodedParameterString}";
    }

    /// <summary>
    /// Creates the signing key for HMAC-SHA1 signature generation.
    /// </summary>
    /// <param name="consumerSecret">The OAuth consumer secret.</param>
    /// <param name="tokenSecret">The OAuth token secret (optional).</param>
    /// <returns>The signing key for HMAC-SHA1.</returns>
    /// <remarks>
    /// The signing key format is: CONSUMER_SECRET&amp;TOKEN_SECRET
    /// Both secrets are percent-encoded before concatenation.
    /// If token secret is null or empty, it's treated as an empty string.
    /// </remarks>
    public static string CreateSigningKey(string consumerSecret, string? tokenSecret = null)
    {
        if (consumerSecret == null)
            throw new ArgumentNullException(nameof(consumerSecret));

        var encodedConsumerSecret = Rfc3986Encoder.Encode(consumerSecret);
        var encodedTokenSecret = Rfc3986Encoder.Encode(tokenSecret ?? "");

        return $"{encodedConsumerSecret}&{encodedTokenSecret}";
    }

    /// <summary>
    /// Generates an HMAC-SHA1 signature for the given base string and signing key.
    /// </summary>
    /// <param name="baseString">The signature base string.</param>
    /// <param name="signingKey">The signing key.</param>
    /// <returns>The Base64-encoded HMAC-SHA1 signature.</returns>
    /// <exception cref="ArgumentNullException">Thrown when parameters are null.</exception>
    /// <remarks>
    /// This method uses the .NET HMACSHA1 implementation to generate the signature
    /// and returns it as a Base64-encoded string as required by OAuth 1.0a.
    /// </remarks>
    public static string GenerateHmacSha1Signature(string baseString, string signingKey)
    {
        if (baseString == null)
            throw new ArgumentNullException(nameof(baseString));
        if (signingKey == null)
            throw new ArgumentNullException(nameof(signingKey));

        var keyBytes = Encoding.UTF8.GetBytes(signingKey);
        var dataBytes = Encoding.UTF8.GetBytes(baseString);

        using var hmac = new HMACSHA1(keyBytes);
        var signatureBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToBase64String(signatureBytes);
    }

    /// <summary>
    /// Creates a complete OAuth parameter collection with signature for a request.
    /// </summary>
    /// <param name="httpMethod">The HTTP method.</param>
    /// <param name="baseUrl">The base URL.</param>
    /// <param name="oauthParameters">The OAuth parameters (without signature).</param>
    /// <param name="requestParameters">Additional request parameters (optional).</param>
    /// <param name="consumerSecret">The OAuth consumer secret.</param>
    /// <param name="tokenSecret">The OAuth token secret (optional).</param>
    /// <returns>A new OAuth parameter collection including the generated signature.</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null.</exception>
    /// <remarks>
    /// This is a convenience method that:
    /// 1. Combines OAuth parameters with request parameters
    /// 2. Generates the OAuth signature
    /// 3. Returns a complete parameter collection including the signature
    /// 
    /// This method is useful for creating signed requests in a single operation.
    /// </remarks>
    public static OAuthParameterCollection CreateSignedParameters(
        string httpMethod,
        string baseUrl,
        OAuthParameterCollection oauthParameters,
        IEnumerable<KeyValuePair<string, string>>? requestParameters,
        string consumerSecret,
        string? tokenSecret = null)
    {
        if (httpMethod == null)
            throw new ArgumentNullException(nameof(httpMethod));
        if (baseUrl == null)
            throw new ArgumentNullException(nameof(baseUrl));
        if (oauthParameters == null)
            throw new ArgumentNullException(nameof(oauthParameters));
        if (consumerSecret == null)
            throw new ArgumentNullException(nameof(consumerSecret));

        // Create a new parameter collection to avoid modifying the original
        var signedParameters = new OAuthParameterCollection();

        // Copy OAuth parameters
        foreach (var param in oauthParameters)
        {
            signedParameters.Add(param.Key, param.Value);
        }

        // Combine with request parameters for signature calculation
        var allParameters = oauthParameters.AsEnumerable();
        if (requestParameters != null)
        {
            allParameters = allParameters.Concat(requestParameters);
        }

        // Generate signature
        var signature = GenerateSignature(httpMethod, baseUrl, allParameters, consumerSecret, tokenSecret);
        signedParameters.SetSignature(signature);

        return signedParameters;
    }
}
