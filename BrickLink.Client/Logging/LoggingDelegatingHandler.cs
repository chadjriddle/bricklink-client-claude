using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BrickLink.Client.Logging;

/// <summary>
/// A DelegatingHandler that automatically logs HTTP requests and responses using the configured logging handler.
/// </summary>
public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;
    private readonly IHttpLoggingHandler _loggingHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingDelegatingHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <param name="loggingHandler">The HTTP logging handler to use for logging operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger or loggingHandler is null.</exception>
    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger, IHttpLoggingHandler loggingHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggingHandler = loggingHandler ?? throw new ArgumentNullException(nameof(loggingHandler));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingDelegatingHandler"/> class with default logging handler.
    /// </summary>
    /// <param name="logger">The logger instance to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
        : this(logger, new HttpLoggingHandler())
    {
    }

    /// <summary>
    /// Gets the HTTP logging handler being used by this delegating handler.
    /// </summary>
    public IHttpLoggingHandler LoggingHandler => _loggingHandler;

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Log the request
            await _loggingHandler.LogRequestAsync(_logger, request, cancellationToken).ConfigureAwait(false);

            // Send the request
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            stopwatch.Stop();

            // Log the response
            await _loggingHandler.LogResponseAsync(_logger, request, response, stopwatch.Elapsed, cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log the exception
            await _loggingHandler.LogRequestExceptionAsync(_logger, request, ex, stopwatch.Elapsed, cancellationToken).ConfigureAwait(false);

            throw;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && _loggingHandler is IDisposable disposableHandler)
        {
            disposableHandler.Dispose();
        }

        base.Dispose(disposing);
    }
}