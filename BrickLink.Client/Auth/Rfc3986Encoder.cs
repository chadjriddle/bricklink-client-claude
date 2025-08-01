using System.Text;

namespace BrickLink.Client.Auth;

/// <summary>
/// Provides RFC 3986 compliant percent encoding for OAuth 1.0a signature generation.
/// This implementation follows the strict percent-encoding rules required by the OAuth specification.
/// </summary>
public static class Rfc3986Encoder
{
    /// <summary>
    /// Characters that are unreserved according to RFC 3986 and should not be percent-encoded.
    /// These include: ALPHA / DIGIT / "-" / "." / "_" / "~"
    /// </summary>
    private static readonly bool[] UnreservedChars = new bool[128];

    /// <summary>
    /// Initializes the unreserved character lookup table.
    /// </summary>
    static Rfc3986Encoder()
    {
        // Initialize all characters as reserved (requiring encoding)
        for (int i = 0; i < UnreservedChars.Length; i++)
        {
            UnreservedChars[i] = false;
        }

        // Mark unreserved characters as per RFC 3986:
        // ALPHA (A-Z, a-z)
        for (int i = 'A'; i <= 'Z'; i++)
        {
            UnreservedChars[i] = true;
        }
        for (int i = 'a'; i <= 'z'; i++)
        {
            UnreservedChars[i] = true;
        }

        // DIGIT (0-9)
        for (int i = '0'; i <= '9'; i++)
        {
            UnreservedChars[i] = true;
        }

        // Other unreserved characters: "-" / "." / "_" / "~"
        UnreservedChars['-'] = true;
        UnreservedChars['.'] = true;
        UnreservedChars['_'] = true;
        UnreservedChars['~'] = true;
    }

    /// <summary>
    /// Encodes a string using RFC 3986 percent encoding.
    /// </summary>
    /// <param name="value">The string to encode. If null, returns an empty string.</param>
    /// <returns>The percent-encoded string according to RFC 3986 rules.</returns>
    /// <remarks>
    /// This method encodes all characters except those defined as unreserved in RFC 3986:
    /// - ALPHA (A-Z, a-z)
    /// - DIGIT (0-9)  
    /// - Hyphen (-), Period (.), Underscore (_), Tilde (~)
    /// 
    /// All other characters, including spaces and special characters, are percent-encoded
    /// using UTF-8 encoding as required by the OAuth 1.0a specification.
    /// </remarks>
    public static string Encode(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var result = new StringBuilder(value.Length * 2); // Pre-allocate assuming some encoding needed
        var utf8Bytes = Encoding.UTF8.GetBytes(value);

        foreach (byte b in utf8Bytes)
        {
            // Check if this byte represents an unreserved ASCII character
            if (b < 128 && UnreservedChars[b])
            {
                result.Append((char)b);
            }
            else
            {
                // Percent-encode the byte
                result.Append('%');
                result.Append(b.ToString("X2")); // Uppercase hex as required by RFC 3986
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Normalizes OAuth parameters by percent-encoding keys and values, then sorting by key.
    /// </summary>
    /// <param name="parameters">The parameters to normalize.</param>
    /// <returns>A sorted collection of percent-encoded key-value pairs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is null.</exception>
    /// <remarks>
    /// This method implements the OAuth 1.0a parameter normalization algorithm:
    /// 1. Percent-encode parameter names and values
    /// 2. Sort parameters by encoded name (then by encoded value if names are equal)
    /// 3. Return the normalized parameter collection
    /// </remarks>
    public static IEnumerable<KeyValuePair<string, string>> NormalizeParameters(
        IEnumerable<KeyValuePair<string, string>> parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        return parameters
            .Select(kvp => new KeyValuePair<string, string>(
                Encode(kvp.Key),
                Encode(kvp.Value)))
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ThenBy(kvp => kvp.Value, StringComparer.Ordinal);
    }

    /// <summary>
    /// Sorts parameters by key, then by value, using ordinal string comparison.
    /// </summary>
    /// <param name="parameters">The parameters to sort.</param>
    /// <returns>Parameters sorted by key, then by value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameters"/> is null.</exception>
    /// <remarks>
    /// This method implements the OAuth 1.0a parameter sorting requirements:
    /// - Sort by parameter name using ordinal (byte-wise) comparison
    /// - If parameter names are equal, sort by parameter value using ordinal comparison
    /// - This ensures consistent ordering required for signature generation
    /// </remarks>
    public static IEnumerable<KeyValuePair<string, string>> SortParameters(
        IEnumerable<KeyValuePair<string, string>> parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        return parameters
            .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
            .ThenBy(kvp => kvp.Value, StringComparer.Ordinal);
    }
}
