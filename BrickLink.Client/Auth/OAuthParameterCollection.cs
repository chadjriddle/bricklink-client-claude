using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace BrickLink.Client.Auth;

/// <summary>
/// Represents a collection of OAuth parameters used in OAuth 1.0a authentication.
/// This class manages the standard OAuth parameters required for request signing.
/// </summary>
public class OAuthParameterCollection : IEnumerable<KeyValuePair<string, string>>
{
    private readonly NameValueCollection _parameters;

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthParameterCollection"/> class.
    /// </summary>
    public OAuthParameterCollection()
    {
        _parameters = new NameValueCollection();
    }

    /// <summary>
    /// Gets or sets the value of the parameter with the specified key.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <returns>The parameter value, or null if the key is not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public string? this[string key]
    {
        get => _parameters[key ?? throw new ArgumentNullException(nameof(key))];
        set => _parameters[key ?? throw new ArgumentNullException(nameof(key))] = value;
    }

    /// <summary>
    /// Gets the number of parameters in the collection.
    /// </summary>
    public int Count => _parameters.Count;

    /// <summary>
    /// Gets all parameter keys in the collection.
    /// </summary>
    public IEnumerable<string> Keys => _parameters.AllKeys.OfType<string>();

    /// <summary>
    /// Adds a parameter to the collection.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">The parameter value.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public void Add(string key, string? value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        _parameters[key] = value;
    }

    /// <summary>
    /// Removes a parameter from the collection.
    /// </summary>
    /// <param name="key">The parameter key to remove.</param>
    /// <returns>true if the parameter was removed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public bool Remove(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (_parameters[key] == null)
            return false;

        _parameters.Remove(key);
        return true;
    }

    /// <summary>
    /// Determines whether the collection contains a parameter with the specified key.
    /// </summary>
    /// <param name="key">The parameter key to locate.</param>
    /// <returns>true if the collection contains a parameter with the key; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is null.</exception>
    public bool ContainsKey(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        return _parameters[key] != null;
    }

    /// <summary>
    /// Removes all parameters from the collection.
    /// </summary>
    public void Clear()
    {
        _parameters.Clear();
    }

    /// <summary>
    /// Sets the standard OAuth consumer key parameter.
    /// </summary>
    /// <param name="consumerKey">The OAuth consumer key.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="consumerKey"/> is null.</exception>
    public void SetConsumerKey(string consumerKey)
    {
        Add("oauth_consumer_key", consumerKey ?? throw new ArgumentNullException(nameof(consumerKey)));
    }

    /// <summary>
    /// Sets the standard OAuth access token parameter.
    /// </summary>
    /// <param name="accessToken">The OAuth access token.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="accessToken"/> is null.</exception>
    public void SetAccessToken(string accessToken)
    {
        Add("oauth_token", accessToken ?? throw new ArgumentNullException(nameof(accessToken)));
    }

    /// <summary>
    /// Sets the standard OAuth signature method parameter.
    /// </summary>
    /// <param name="signatureMethod">The OAuth signature method (typically "HMAC-SHA1").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="signatureMethod"/> is null.</exception>
    public void SetSignatureMethod(string signatureMethod)
    {
        Add("oauth_signature_method", signatureMethod ?? throw new ArgumentNullException(nameof(signatureMethod)));
    }

    /// <summary>
    /// Sets the standard OAuth timestamp parameter.
    /// </summary>
    /// <param name="timestamp">The OAuth timestamp.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="timestamp"/> is null.</exception>
    public void SetTimestamp(string timestamp)
    {
        Add("oauth_timestamp", timestamp ?? throw new ArgumentNullException(nameof(timestamp)));
    }

    /// <summary>
    /// Sets the standard OAuth nonce parameter.
    /// </summary>
    /// <param name="nonce">The OAuth nonce.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="nonce"/> is null.</exception>
    public void SetNonce(string nonce)
    {
        Add("oauth_nonce", nonce ?? throw new ArgumentNullException(nameof(nonce)));
    }

    /// <summary>
    /// Sets the standard OAuth version parameter.
    /// </summary>
    /// <param name="version">The OAuth version (typically "1.0").</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="version"/> is null.</exception>
    public void SetVersion(string version)
    {
        Add("oauth_version", version ?? throw new ArgumentNullException(nameof(version)));
    }

    /// <summary>
    /// Sets the OAuth signature parameter.
    /// </summary>
    /// <param name="signature">The OAuth signature.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="signature"/> is null.</exception>
    public void SetSignature(string signature)
    {
        Add("oauth_signature", signature ?? throw new ArgumentNullException(nameof(signature)));
    }

    /// <summary>
    /// Converts the OAuth parameters to a query string format.
    /// </summary>
    /// <returns>A query string representation of the OAuth parameters.</returns>
    /// <remarks>
    /// The parameters are URL-encoded and sorted alphabetically by key as required
    /// by the OAuth 1.0a specification for signature generation.
    /// </remarks>
    public string ToQueryString()
    {
        var sortedParams = this.OrderBy(kvp => kvp.Key, StringComparer.Ordinal);
        var encodedParams = sortedParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? "")}");

        return string.Join("&", encodedParams);
    }

    /// <summary>
    /// Converts the OAuth parameters to an Authorization header value format.
    /// </summary>
    /// <returns>An Authorization header value containing the OAuth parameters.</returns>
    /// <remarks>
    /// The returned string is formatted as per the OAuth 1.0a specification for
    /// the Authorization header, with parameters comma-separated and values quoted.
    /// </remarks>
    public string ToAuthorizationHeaderValue()
    {
        var sortedParams = this.OrderBy(kvp => kvp.Key, StringComparer.Ordinal);
        var formattedParams = sortedParams.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}=\"{Uri.EscapeDataString(kvp.Value ?? "")}\"");

        return $"OAuth {string.Join(", ", formattedParams)}";
    }

    /// <summary>
    /// Returns an enumerator that iterates through the OAuth parameters.
    /// </summary>
    /// <returns>An enumerator for the OAuth parameters.</returns>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        foreach (string? key in _parameters.AllKeys)
        {
            if (key != null)
            {
                var value = _parameters[key];
                yield return new KeyValuePair<string, string>(key, value ?? "");
            }
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the OAuth parameters.
    /// </summary>
    /// <returns>An enumerator for the OAuth parameters.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Returns a string representation of the OAuth parameters in query string format.
    /// </summary>
    /// <returns>A query string representation of the OAuth parameters.</returns>
    public override string ToString()
    {
        return ToQueryString();
    }
}
