using System.Security.Cryptography;
using System.Text;

namespace BrickLink.Client.Auth;

/// <summary>
/// Provides cryptographically secure nonce generation for OAuth 1.0a authentication.
/// A nonce (number used once) is a random value that prevents replay attacks.
/// </summary>
public static class NonceGenerator
{
    /// <summary>
    /// Generates a cryptographically secure random nonce string.
    /// </summary>
    /// <returns>A base64-encoded random string suitable for use as an OAuth nonce.</returns>
    /// <remarks>
    /// The nonce is generated using <see cref="RandomNumberGenerator"/> to ensure 
    /// cryptographic security. The resulting string is base64-encoded and URL-safe.
    /// </remarks>
    public static string Generate()
    {
        // Generate 16 bytes (128 bits) of random data for sufficient entropy
        var randomBytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        // Convert to base64 and make URL-safe by replacing problematic characters
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }

    /// <summary>
    /// Generates a cryptographically secure random nonce string with the specified length.
    /// </summary>
    /// <param name="length">The desired length of the nonce in bytes. Must be between 8 and 64.</param>
    /// <returns>A base64-encoded random string suitable for use as an OAuth nonce.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="length"/> is less than 8 or greater than 64.
    /// </exception>
    /// <remarks>
    /// The nonce is generated using <see cref="RandomNumberGenerator"/> to ensure 
    /// cryptographic security. The resulting string is base64-encoded and URL-safe.
    /// </remarks>
    public static string Generate(int length)
    {
        if (length < 8 || length > 64)
        {
            throw new ArgumentOutOfRangeException(nameof(length),
                "Nonce length must be between 8 and 64 bytes.");
        }

        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        // Convert to base64 and make URL-safe by replacing problematic characters
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }
}
