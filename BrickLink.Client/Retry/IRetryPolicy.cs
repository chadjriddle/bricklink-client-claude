using System.Net;

namespace BrickLink.Client.Retry;

/// <summary>
/// Defines a contract for retry policies that determine when and how to retry failed operations.
/// </summary>
public interface IRetryPolicy
{
    /// <summary>
    /// Determines whether a retry should be attempted based on the exception and attempt count.
    /// </summary>
    /// <param name="exception">The exception that occurred during the operation.</param>
    /// <param name="attemptCount">The current attempt count (1-based).</param>
    /// <returns>True if the operation should be retried; otherwise, false.</returns>
    bool ShouldRetry(Exception exception, int attemptCount);

    /// <summary>
    /// Determines whether a retry should be attempted based on the HTTP status code and attempt count.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the server.</param>
    /// <param name="attemptCount">The current attempt count (1-based).</param>
    /// <returns>True if the operation should be retried; otherwise, false.</returns>
    bool ShouldRetry(HttpStatusCode statusCode, int attemptCount);

    /// <summary>
    /// Calculates the delay before the next retry attempt.
    /// </summary>
    /// <param name="attemptCount">The current attempt count (1-based).</param>
    /// <returns>The delay to wait before the next retry attempt.</returns>
    TimeSpan GetRetryDelay(int attemptCount);

    /// <summary>
    /// Gets the maximum number of retry attempts allowed.
    /// </summary>
    int MaxRetryAttempts { get; }
}
