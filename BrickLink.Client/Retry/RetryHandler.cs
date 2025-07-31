using System.Net;
using BrickLink.Client.Exceptions;

namespace BrickLink.Client.Retry;

/// <summary>
/// Provides retry functionality for HTTP operations using a configurable retry policy.
/// </summary>
public class RetryHandler
{
    private readonly IRetryPolicy _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryHandler"/> class with default policy.
    /// </summary>
    public RetryHandler()
    {
        _retryPolicy = new ExponentialBackoffRetryPolicy();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryHandler"/> class.
    /// </summary>
    /// <param name="retryPolicy">The retry policy to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when retryPolicy is null.</exception>
    public RetryHandler(IRetryPolicy retryPolicy)
    {
        _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
    }

    /// <summary>
    /// Gets the retry policy being used by this handler.
    /// </summary>
    public IRetryPolicy RetryPolicy => _retryPolicy;

    /// <summary>
    /// Executes an operation with retry logic applied.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when operation is null.</exception>
    public virtual async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        var attemptCount = 0;
        Exception? lastException = null;

        while (attemptCount <= _retryPolicy.MaxRetryAttempts)
        {
            attemptCount++;
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch (Exception ex) when (attemptCount <= _retryPolicy.MaxRetryAttempts)
            {
                lastException = ex;

                if (!_retryPolicy.ShouldRetry(ex, attemptCount))
                {
                    throw;
                }

                var delay = _retryPolicy.GetRetryDelay(attemptCount);
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        // If we've exhausted all retries, throw the last exception
        throw lastException ?? new InvalidOperationException("Operation failed without an exception.");
    }

    /// <summary>
    /// Executes an HTTP operation with retry logic applied.
    /// </summary>
    /// <param name="httpOperation">The HTTP operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when httpOperation is null.</exception>
    public virtual async Task<HttpResponseMessage> ExecuteHttpAsync(Func<Task<HttpResponseMessage>> httpOperation, CancellationToken cancellationToken = default)
    {
        if (httpOperation == null)
        {
            throw new ArgumentNullException(nameof(httpOperation));
        }

        var attemptCount = 0;
        Exception? lastException = null;
        HttpStatusCode? lastStatusCode = null;

        while (true)
        {
            attemptCount++;
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var response = await httpOperation().ConfigureAwait(false);

                // Check if the status code indicates we should retry
                if (!response.IsSuccessStatusCode)
                {
                    lastStatusCode = response.StatusCode;

                    // If we've exhausted all attempts, break to throw exception
                    if (attemptCount > _retryPolicy.MaxRetryAttempts)
                    {
                        response.Dispose();
                        break;
                    }

                    // Check if this status code is retriable
                    bool shouldRetry = _retryPolicy.ShouldRetry(response.StatusCode, attemptCount);

                    if (shouldRetry)
                    {
                        response.Dispose(); // Dispose the failed response

                        var delay = _retryPolicy.GetRetryDelay(attemptCount);
                        if (delay > TimeSpan.Zero)
                        {
                            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                        }
                        continue;
                    }
                    else
                    {
                        // This status code is not retriable - return the response immediately
                        return response;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                lastException = ex;

                // Check if we should retry and have attempts remaining
                if (attemptCount <= _retryPolicy.MaxRetryAttempts && _retryPolicy.ShouldRetry(ex, attemptCount))
                {
                    var delay = _retryPolicy.GetRetryDelay(attemptCount);
                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    }
                    continue;
                }
                else
                {
                    // Either not retriable or exhausted retries
                    throw;
                }
            }
        }

        // If we've exhausted all retries, throw an appropriate exception
        if (lastException != null)
        {
            throw lastException;
        }

        if (lastStatusCode.HasValue)
        {
            throw BrickLinkApiException.CreateServerError($"HTTP request failed with status code {lastStatusCode.Value} after {_retryPolicy.MaxRetryAttempts} retry attempts.");
        }

        throw new InvalidOperationException("HTTP operation failed without an exception or status code.");
    }

    /// <summary>
    /// Executes an operation with retry logic applied, without returning a value.
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when operation is null.</exception>
    public virtual async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        await ExecuteAsync(async () =>
        {
            await operation().ConfigureAwait(false);
            return true; // Return a dummy value to satisfy the generic method
        }, cancellationToken).ConfigureAwait(false);
    }
}
