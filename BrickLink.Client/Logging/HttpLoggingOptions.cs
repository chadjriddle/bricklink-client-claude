using Microsoft.Extensions.Logging;

namespace BrickLink.Client.Logging;

/// <summary>
/// Configuration options for HTTP request and response logging.
/// </summary>
public class HttpLoggingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether request logging is enabled.
    /// Default is true.
    /// </summary>
    public bool LogRequests { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether response logging is enabled.
    /// Default is true.
    /// </summary>
    public bool LogResponses { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether request headers should be logged.
    /// Default is true.
    /// </summary>
    public bool LogRequestHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether response headers should be logged.
    /// Default is true.
    /// </summary>
    public bool LogResponseHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether request content/body should be logged.
    /// Default is false for security reasons.
    /// </summary>
    public bool LogRequestContent { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether response content/body should be logged.
    /// Default is false to avoid logging large responses.
    /// </summary>
    public bool LogResponseContent { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum size of content to log in bytes.
    /// Content larger than this will be truncated.
    /// Default is 1024 bytes (1KB).
    /// </summary>
    public int MaxContentLogSize { get; set; } = 1024;

    /// <summary>
    /// Gets or sets the log level for successful requests.
    /// Default is Information.
    /// </summary>
    public LogLevel SuccessLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Gets or sets the log level for client error requests (4xx status codes).
    /// Default is Warning.
    /// </summary>
    public LogLevel ClientErrorLogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    /// Gets or sets the log level for server error requests (5xx status codes).
    /// Default is Error.
    /// </summary>
    public LogLevel ServerErrorLogLevel { get; set; } = LogLevel.Error;

    /// <summary>
    /// Gets or sets the log level for exceptions during requests.
    /// Default is Error.
    /// </summary>
    public LogLevel ExceptionLogLevel { get; set; } = LogLevel.Error;

    /// <summary>
    /// Gets or sets a set of header names that should be redacted from logs for security.
    /// Default includes Authorization, X-API-Key, and other sensitive headers.
    /// </summary>
    public HashSet<string> RedactedHeaders { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "X-API-Key",
        "X-Auth-Token",
        "Cookie",
        "Set-Cookie",
        "Proxy-Authorization",
        "WWW-Authenticate"
    };

    /// <summary>
    /// Gets or sets the value used to replace redacted header values.
    /// Default is "[REDACTED]".
    /// </summary>
    public string RedactedValue { get; set; } = "[REDACTED]";
}
