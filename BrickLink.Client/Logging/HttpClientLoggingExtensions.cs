using Microsoft.Extensions.Logging;

namespace BrickLink.Client.Logging;

/// <summary>
/// Extension methods for adding HTTP logging capabilities to HttpClient configurations.
/// </summary>
public static class HttpClientLoggingExtensions
{
    /// <summary>
    /// Creates a LoggingDelegatingHandler with the specified logger and logging handler.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="loggingHandler">The HTTP logging handler to use for logging operations.</param>
    /// <returns>A configured LoggingDelegatingHandler instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logger or loggingHandler is null.</exception>
    public static LoggingDelegatingHandler CreateLoggingHandler(ILogger<LoggingDelegatingHandler> logger, IHttpLoggingHandler loggingHandler)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (loggingHandler == null)
        {
            throw new ArgumentNullException(nameof(loggingHandler));
        }

        return new LoggingDelegatingHandler(logger, loggingHandler);
    }

    /// <summary>
    /// Creates a LoggingDelegatingHandler with the specified logger and default logging options.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <returns>A configured LoggingDelegatingHandler instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public static LoggingDelegatingHandler CreateLoggingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        return new LoggingDelegatingHandler(logger);
    }

    /// <summary>
    /// Creates a LoggingDelegatingHandler with the specified logger and logging options.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="options">The logging configuration options.</param>
    /// <returns>A configured LoggingDelegatingHandler instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logger or options is null.</exception>
    public static LoggingDelegatingHandler CreateLoggingHandler(ILogger<LoggingDelegatingHandler> logger, HttpLoggingOptions options)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var loggingHandler = new HttpLoggingHandler(options);
        return new LoggingDelegatingHandler(logger, loggingHandler);
    }

    /// <summary>
    /// Creates an HttpClientHandler with logging capabilities integrated into the pipeline.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="loggingHandler">The HTTP logging handler to use for logging operations.</param>
    /// <param name="innerHandler">Optional inner handler to wrap. If null, creates a new HttpClientHandler.</param>
    /// <returns>A configured DelegatingHandler that includes logging capabilities.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logger or loggingHandler is null.</exception>
    public static DelegatingHandler CreateLoggingPipeline(ILogger<LoggingDelegatingHandler> logger, IHttpLoggingHandler loggingHandler, HttpMessageHandler? innerHandler = null)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (loggingHandler == null)
        {
            throw new ArgumentNullException(nameof(loggingHandler));
        }

        var loggingDelegatingHandler = new LoggingDelegatingHandler(logger, loggingHandler);

        if (innerHandler != null)
        {
            loggingDelegatingHandler.InnerHandler = innerHandler;
        }
        else
        {
            loggingDelegatingHandler.InnerHandler = new HttpClientHandler();
        }

        return loggingDelegatingHandler;
    }

    /// <summary>
    /// Creates an HttpClientHandler with logging capabilities and default logging options.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="innerHandler">Optional inner handler to wrap. If null, creates a new HttpClientHandler.</param>
    /// <returns>A configured DelegatingHandler that includes logging capabilities.</returns>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public static DelegatingHandler CreateLoggingPipeline(ILogger<LoggingDelegatingHandler> logger, HttpMessageHandler? innerHandler = null)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        var loggingHandler = new HttpLoggingHandler();
        return CreateLoggingPipeline(logger, loggingHandler, innerHandler);
    }
}
