using System.Text;
using Microsoft.Extensions.Logging;

namespace BrickLink.Client.Logging;

/// <summary>
/// Provides structured logging for HTTP requests and responses with configurable options.
/// </summary>
public class HttpLoggingHandler : IHttpLoggingHandler
{
    private readonly HttpLoggingOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpLoggingHandler"/> class with default options.
    /// </summary>
    public HttpLoggingHandler()
        : this(new HttpLoggingOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpLoggingHandler"/> class.
    /// </summary>
    /// <param name="options">The logging configuration options.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    public HttpLoggingHandler(HttpLoggingOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Gets the logging options being used by this handler.
    /// </summary>
    public HttpLoggingOptions Options => _options;

    /// <inheritdoc />
    public async Task LogRequestAsync(ILogger logger, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!_options.LogRequests)
        {
            return;
        }

        try
        {
            var logData = new Dictionary<string, object?>
            {
                ["HttpMethod"] = request.Method.Method,
                ["RequestUri"] = request.RequestUri?.ToString(),
                ["HttpVersion"] = request.Version?.ToString()
            };

            if (_options.LogRequestHeaders && request.Headers.Any())
            {
                logData["RequestHeaders"] = await FormatHeadersAsync(request.Headers, cancellationToken).ConfigureAwait(false);
            }

            if (request.Content != null)
            {
                if (request.Content.Headers.Any())
                {
                    logData["ContentHeaders"] = await FormatHeadersAsync(request.Content.Headers, cancellationToken).ConfigureAwait(false);
                }

                if (_options.LogRequestContent)
                {
                    var content = await GetContentStringAsync(request.Content, cancellationToken).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(content))
                    {
                        logData["RequestContent"] = content;
                    }
                }
            }

            logger.Log(_options.SuccessLogLevel, "HTTP Request: {HttpMethod} {RequestUri}", request.Method.Method, request.RequestUri);

            using var scope = logger.BeginScope(logData);
            logger.Log(_options.SuccessLogLevel, "Sending HTTP request");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to log HTTP request details");
        }
    }

    /// <inheritdoc />
    public async Task LogResponseAsync(ILogger logger, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed, CancellationToken cancellationToken = default)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        if (response == null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        if (!_options.LogResponses)
        {
            return;
        }

        try
        {
            var logLevel = GetLogLevelForStatusCode(response.StatusCode);
            var statusCode = (int)response.StatusCode;

            var logData = new Dictionary<string, object?>
            {
                ["HttpMethod"] = request.Method.Method,
                ["RequestUri"] = request.RequestUri?.ToString(),
                ["StatusCode"] = statusCode,
                ["ReasonPhrase"] = response.ReasonPhrase,
                ["ElapsedMs"] = elapsed.TotalMilliseconds,
                ["HttpVersion"] = response.Version?.ToString()
            };

            if (_options.LogResponseHeaders && response.Headers.Any())
            {
                logData["ResponseHeaders"] = await FormatHeadersAsync(response.Headers, cancellationToken).ConfigureAwait(false);
            }

            if (response.Content != null)
            {
                if (response.Content.Headers.Any())
                {
                    logData["ContentHeaders"] = await FormatHeadersAsync(response.Content.Headers, cancellationToken).ConfigureAwait(false);
                }

                if (_options.LogResponseContent)
                {
                    var content = await GetContentStringAsync(response.Content, cancellationToken).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(content))
                    {
                        logData["ResponseContent"] = content;
                    }
                }
            }

            logger.Log(logLevel, "HTTP Response: {HttpMethod} {RequestUri} responded {StatusCode} {ReasonPhrase} in {ElapsedMs}ms",
                request.Method.Method, request.RequestUri, statusCode, response.ReasonPhrase, elapsed.TotalMilliseconds);

            using var scope = logger.BeginScope(logData);
            logger.Log(logLevel, "Received HTTP response");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to log HTTP response details");
        }
    }

    /// <inheritdoc />
    public async Task LogRequestExceptionAsync(ILogger logger, HttpRequestMessage request, Exception exception, TimeSpan elapsed, CancellationToken cancellationToken = default)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        try
        {
            var logData = new Dictionary<string, object?>
            {
                ["HttpMethod"] = request.Method.Method,
                ["RequestUri"] = request.RequestUri?.ToString(),
                ["ElapsedMs"] = elapsed.TotalMilliseconds,
                ["ExceptionType"] = exception.GetType().Name,
                ["ExceptionMessage"] = exception.Message
            };

            if (_options.LogRequestHeaders && request.Headers.Any())
            {
                logData["RequestHeaders"] = await FormatHeadersAsync(request.Headers, cancellationToken).ConfigureAwait(false);
            }

            logger.Log(_options.ExceptionLogLevel, exception, "HTTP Request Exception: {HttpMethod} {RequestUri} failed after {ElapsedMs}ms",
                request.Method.Method, request.RequestUri, elapsed.TotalMilliseconds);

            using var scope = logger.BeginScope(logData);
            logger.Log(_options.ExceptionLogLevel, exception, "HTTP request failed with exception");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to log HTTP request exception details");
        }
    }

    /// <summary>
    /// Determines the appropriate log level based on the HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>The appropriate log level.</returns>
    private LogLevel GetLogLevelForStatusCode(System.Net.HttpStatusCode statusCode)
    {
        var statusCodeValue = (int)statusCode;

        return statusCodeValue switch
        {
            >= 200 and < 400 => _options.SuccessLogLevel,
            >= 400 and < 500 => _options.ClientErrorLogLevel,
            >= 500 => _options.ServerErrorLogLevel,
            _ => _options.SuccessLogLevel
        };
    }

    /// <summary>
    /// Formats HTTP headers for logging, redacting sensitive values.
    /// </summary>
    /// <param name="headers">The headers to format.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A dictionary of formatted headers.</returns>
    private async Task<Dictionary<string, object?>> FormatHeadersAsync(System.Net.Http.Headers.HttpHeaders headers, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, object?>();

        await Task.Run(() =>
        {
            foreach (var header in headers)
            {
                var headerName = header.Key;
                var headerValues = header.Value;

                if (_options.RedactedHeaders.Contains(headerName))
                {
                    result[headerName] = _options.RedactedValue;
                }
                else
                {
                    result[headerName] = headerValues?.Count() == 1
                        ? headerValues.First()
                        : headerValues?.ToArray();
                }
            }
        }, cancellationToken).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Gets the content of an HttpContent as a string, respecting size limits.
    /// </summary>
    /// <param name="content">The HTTP content.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The content as a string, possibly truncated.</returns>
    private async Task<string?> GetContentStringAsync(HttpContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            var contentString = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrEmpty(contentString))
            {
                return null;
            }

            if (contentString.Length <= _options.MaxContentLogSize)
            {
                return contentString;
            }

            return contentString.Substring(0, _options.MaxContentLogSize) + "... [TRUNCATED]";
        }
        catch (Exception)
        {
            return "[CONTENT READ ERROR]";
        }
    }
}
