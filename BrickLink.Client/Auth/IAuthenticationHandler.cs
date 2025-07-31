namespace BrickLink.Client.Auth;

/// <summary>
/// Defines the contract for handling OAuth 1.0a-like authentication for BrickLink API requests.
/// </summary>
public interface IAuthenticationHandler
{
    /// <summary>
    /// Applies OAuth authentication headers to the specified HTTP request message.
    /// </summary>
    /// <param name="request">The HTTP request message to authenticate.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous authentication operation.</returns>
    Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the authentication handler has all required credentials.
    /// </summary>
    /// <returns>True if the handler is properly configured; otherwise, false.</returns>
    bool IsConfigured();
}
