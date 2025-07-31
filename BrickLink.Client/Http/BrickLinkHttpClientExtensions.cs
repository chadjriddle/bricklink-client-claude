using BrickLink.Client.Retry;

namespace BrickLink.Client.Http;

/// <summary>
/// Extension methods for <see cref="BrickLinkHttpClient"/> to add retry functionality.
/// </summary>
public static class BrickLinkHttpClientExtensions
{
    /// <summary>
    /// Sends an HTTP GET request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> GetWithRetryAsync(
        this BrickLinkHttpClient client,
        string requestUri,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.GetAsync(requestUri, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP GET request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> GetWithRetryAsync(
        this BrickLinkHttpClient client,
        Uri requestUri,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.GetAsync(requestUri, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> PostWithRetryAsync(
        this BrickLinkHttpClient client,
        string requestUri,
        HttpContent? content,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.PostAsync(requestUri, content, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP POST request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> PostWithRetryAsync(
        this BrickLinkHttpClient client,
        Uri requestUri,
        HttpContent? content,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.PostAsync(requestUri, content, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> PutWithRetryAsync(
        this BrickLinkHttpClient client,
        string requestUri,
        HttpContent? content,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.PutAsync(requestUri, content, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP PUT request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="content">The HTTP request content sent to the server.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> PutWithRetryAsync(
        this BrickLinkHttpClient client,
        Uri requestUri,
        HttpContent? content,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.PutAsync(requestUri, content, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> DeleteWithRetryAsync(
        this BrickLinkHttpClient client,
        string requestUri,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.DeleteAsync(requestUri, cancellationToken), cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP DELETE request with retry logic applied.
    /// </summary>
    /// <param name="client">The BrickLinkHttpClient instance.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="retryHandler">The retry handler to use. If null, uses default retry handler.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when client is null.</exception>
    public static Task<HttpResponseMessage> DeleteWithRetryAsync(
        this BrickLinkHttpClient client,
        Uri requestUri,
        RetryHandler? retryHandler = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        var handler = retryHandler ?? new RetryHandler();
        return handler.ExecuteHttpAsync(() => client.DeleteAsync(requestUri, cancellationToken), cancellationToken);
    }
}
