using System.Net;
using System.Net.Sockets;

namespace BrickLink.Client.Retry;

/// <summary>
/// Configuration options for retry policies.
/// </summary>
public sealed class RetryPolicyOptions
{
    /// <summary>
    /// The default maximum number of retry attempts.
    /// </summary>
    public const int DefaultMaxRetryAttempts = 3;

    /// <summary>
    /// The default base delay for exponential backoff.
    /// </summary>
    public static readonly TimeSpan DefaultBaseDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    /// The default maximum delay between retry attempts.
    /// </summary>
    public static readonly TimeSpan DefaultMaxDelay = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = DefaultMaxRetryAttempts;

    /// <summary>
    /// Gets or sets the base delay for exponential backoff calculations.
    /// </summary>
    public TimeSpan BaseDelay { get; set; } = DefaultBaseDelay;

    /// <summary>
    /// Gets or sets the maximum delay between retry attempts.
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = DefaultMaxDelay;

    /// <summary>
    /// Gets or sets the jitter factor for randomizing delays (0.0 to 1.0).
    /// A jitter factor of 0.1 means the delay can vary by ±10%.
    /// </summary>
    public double JitterFactor { get; set; } = 0.1;

    /// <summary>
    /// Gets or sets the HTTP status codes that should trigger a retry.
    /// </summary>
    public HashSet<HttpStatusCode> RetriableStatusCodes { get; set; } = new()
    {
        HttpStatusCode.InternalServerError,      // 500
        HttpStatusCode.BadGateway,               // 502
        HttpStatusCode.ServiceUnavailable,       // 503
        HttpStatusCode.GatewayTimeout,           // 504
        HttpStatusCode.RequestTimeout,           // 408
        HttpStatusCode.TooManyRequests           // 429
    };

    /// <summary>
    /// Gets or sets the exception types that should trigger a retry.
    /// </summary>
    public HashSet<Type> RetriableExceptionTypes { get; set; } = new()
    {
        typeof(HttpRequestException),
        typeof(TaskCanceledException),
        typeof(TimeoutException),
        typeof(SocketException)
    };

    /// <summary>
    /// Creates a copy of the current options.
    /// </summary>
    /// <returns>A new instance with the same configuration values.</returns>
    public RetryPolicyOptions Clone()
    {
        return new RetryPolicyOptions
        {
            MaxRetryAttempts = MaxRetryAttempts,
            BaseDelay = BaseDelay,
            MaxDelay = MaxDelay,
            JitterFactor = JitterFactor,
            RetriableStatusCodes = new HashSet<HttpStatusCode>(RetriableStatusCodes),
            RetriableExceptionTypes = new HashSet<Type>(RetriableExceptionTypes)
        };
    }
}
