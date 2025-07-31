using Microsoft.Extensions.Logging;

namespace BrickLink.Client.Logging;

/// <summary>
/// Defines a contract for logging HTTP requests and responses.
/// </summary>
public interface IHttpLoggingHandler
{
    /// <summary>
    /// Logs details about an HTTP request before it is sent.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous logging operation.</returns>
    Task LogRequestAsync(ILogger logger, HttpRequestMessage request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs details about an HTTP response after it is received.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="request">The original HTTP request message.</param>
    /// <param name="response">The HTTP response message.</param>
    /// <param name="elapsed">The time elapsed for the request.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous logging operation.</returns>
    Task LogResponseAsync(ILogger logger, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs details about an HTTP request that resulted in an exception.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="elapsed">The time elapsed before the exception occurred.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous logging operation.</returns>
    Task LogRequestExceptionAsync(ILogger logger, HttpRequestMessage request, Exception exception, TimeSpan elapsed, CancellationToken cancellationToken = default);
}