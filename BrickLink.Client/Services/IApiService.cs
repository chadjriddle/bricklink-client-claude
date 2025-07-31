namespace BrickLink.Client.Services;

/// <summary>
/// Defines the base contract for all API service implementations.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Gets the base URL for API requests handled by this service.
    /// </summary>
    string BaseUrl { get; }

    /// <summary>
    /// Performs a health check to verify the service can communicate with the API.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous health check operation.</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}