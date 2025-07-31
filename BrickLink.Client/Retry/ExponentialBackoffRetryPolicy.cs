using System.Net;
using System.Net.Sockets;
using BrickLink.Client.Exceptions;

namespace BrickLink.Client.Retry;

/// <summary>
/// Implements an exponential backoff retry policy with configurable jitter.
/// </summary>
public sealed class ExponentialBackoffRetryPolicy : IRetryPolicy
{
    private readonly RetryPolicyOptions _options;
    private readonly Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialBackoffRetryPolicy"/> class.
    /// </summary>
    /// <param name="options">The retry policy configuration options.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    /// <exception cref="ArgumentException">Thrown when options contain invalid values.</exception>
    public ExponentialBackoffRetryPolicy(RetryPolicyOptions? options = null)
    {
        _options = options?.Clone() ?? new RetryPolicyOptions();
        ValidateOptions(_options);
        _random = new Random();
    }

    /// <inheritdoc />
    public int MaxRetryAttempts => _options.MaxRetryAttempts;

    /// <inheritdoc />
    public bool ShouldRetry(Exception exception, int attemptCount)
    {
        if (exception == null)
        {
            return false;
        }

        if (attemptCount > MaxRetryAttempts)
        {
            return false;
        }

        // Check for BrickLinkApiException and examine the status code
        if (exception is BrickLinkApiException apiException)
        {
            return ShouldRetry(apiException.StatusCode, attemptCount);
        }

        // Check if the exception type is retriable (excluding SocketException which needs special handling)
        var exceptionType = exception.GetType();
        if (_options.RetriableExceptionTypes.Contains(exceptionType) && exceptionType != typeof(SocketException))
        {
            return true;
        }

        // Check base types for inherited exceptions (excluding SocketException)
        var currentType = exceptionType.BaseType;
        while (currentType != null && currentType != typeof(object))
        {
            if (_options.RetriableExceptionTypes.Contains(currentType) && currentType != typeof(SocketException))
            {
                return true;
            }
            currentType = currentType.BaseType;
        }

        // Special handling for specific exception scenarios
        return exception switch
        {
            HttpRequestException httpEx when IsTransientHttpException(httpEx) => true,
            TaskCanceledException tcEx when IsTimeoutException(tcEx) => true,
            SocketException sockEx when IsTransientSocketException(sockEx) => true,
            _ => false
        };
    }

    /// <inheritdoc />
    public bool ShouldRetry(HttpStatusCode statusCode, int attemptCount)
    {
        if (attemptCount > MaxRetryAttempts)
        {
            return false;
        }

        return _options.RetriableStatusCodes.Contains(statusCode);
    }

    /// <inheritdoc />
    public TimeSpan GetRetryDelay(int attemptCount)
    {
        if (attemptCount <= 0)
        {
            return TimeSpan.Zero;
        }

        // Calculate exponential backoff: baseDelay * 2^(attemptCount-1)
        var delay = TimeSpan.FromMilliseconds(
            _options.BaseDelay.TotalMilliseconds * Math.Pow(2, attemptCount - 1));

        // Cap the delay at the maximum
        if (delay > _options.MaxDelay)
        {
            delay = _options.MaxDelay;
        }

        // Apply jitter if configured
        if (_options.JitterFactor > 0)
        {
            delay = ApplyJitter(delay, _options.JitterFactor);
        }

        return delay;
    }

    /// <summary>
    /// Validates the retry policy options.
    /// </summary>
    /// <param name="options">The options to validate.</param>
    /// <exception cref="ArgumentException">Thrown when options contain invalid values.</exception>
    private static void ValidateOptions(RetryPolicyOptions options)
    {
        if (options.MaxRetryAttempts < 0)
        {
            throw new ArgumentException("MaxRetryAttempts cannot be negative.", nameof(options));
        }

        if (options.BaseDelay < TimeSpan.Zero)
        {
            throw new ArgumentException("BaseDelay cannot be negative.", nameof(options));
        }

        if (options.MaxDelay < TimeSpan.Zero)
        {
            throw new ArgumentException("MaxDelay cannot be negative.", nameof(options));
        }

        if (options.BaseDelay > options.MaxDelay)
        {
            throw new ArgumentException("BaseDelay cannot be greater than MaxDelay.", nameof(options));
        }

        if (options.JitterFactor < 0.0 || options.JitterFactor > 1.0)
        {
            throw new ArgumentException("JitterFactor must be between 0.0 and 1.0.", nameof(options));
        }
    }

    /// <summary>
    /// Applies jitter to the delay to avoid thundering herd problems.
    /// </summary>
    /// <param name="delay">The base delay.</param>
    /// <param name="jitterFactor">The jitter factor (0.0 to 1.0).</param>
    /// <returns>The delay with jitter applied.</returns>
    private TimeSpan ApplyJitter(TimeSpan delay, double jitterFactor)
    {
        // Generate a random multiplier between (1 - jitterFactor) and (1 + jitterFactor)
        var jitterRange = jitterFactor * 2;
        var jitterMultiplier = (1 - jitterFactor) + (_random.NextDouble() * jitterRange);

        var jitteredDelayMs = delay.TotalMilliseconds * jitterMultiplier;
        return TimeSpan.FromMilliseconds(Math.Max(0, jitteredDelayMs));
    }

    /// <summary>
    /// Determines if an HttpRequestException represents a transient error.
    /// </summary>
    /// <param name="exception">The HttpRequestException to examine.</param>
    /// <returns>True if the exception represents a transient error; otherwise, false.</returns>
    private static bool IsTransientHttpException(HttpRequestException exception)
    {
        // Check the message for common transient error indicators
        var message = exception.Message.ToLowerInvariant();
        return message.Contains("timeout") ||
               message.Contains("connection") ||
               message.Contains("network") ||
               message.Contains("host") ||
               message.Contains("dns");
    }

    /// <summary>
    /// Determines if a TaskCanceledException represents a timeout (rather than user cancellation).
    /// </summary>
    /// <param name="exception">The TaskCanceledException to examine.</param>
    /// <returns>True if the exception represents a timeout; otherwise, false.</returns>
    private static bool IsTimeoutException(TaskCanceledException exception)
    {
        // If there's an inner exception, it might provide more context
        if (exception.InnerException is TimeoutException)
        {
            return true;
        }

        // Check the message for timeout indicators
        var message = exception.Message.ToLowerInvariant();
        return message.Contains("timeout") || message.Contains("timed out");
    }

    /// <summary>
    /// Determines if a SocketException represents a transient error.
    /// </summary>
    /// <param name="exception">The SocketException to examine.</param>
    /// <returns>True if the exception represents a transient error; otherwise, false.</returns>
    private static bool IsTransientSocketException(SocketException exception)
    {
        // Common transient socket errors
        return exception.SocketErrorCode switch
        {
            SocketError.ConnectionRefused => true,
            SocketError.ConnectionReset => true,
            SocketError.ConnectionAborted => true,
            SocketError.NetworkDown => true,
            SocketError.NetworkUnreachable => true,
            SocketError.HostDown => true,
            SocketError.HostUnreachable => true,
            SocketError.TimedOut => true,
            SocketError.TryAgain => true,
            _ => false
        };
    }
}
