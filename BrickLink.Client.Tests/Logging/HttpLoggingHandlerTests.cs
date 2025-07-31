using System.Net;
using System.Text;
using BrickLink.Client.Logging;
using Microsoft.Extensions.Logging;
using Moq;

namespace BrickLink.Client.Tests.Logging;

public class HttpLoggingHandlerTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly HttpLoggingHandler _handler;

    public HttpLoggingHandlerTests()
    {
        _mockLogger = new Mock<ILogger>();
        _handler = new HttpLoggingHandler();
    }

    [Fact]
    public void Constructor_WithDefaultOptions_SetsDefaultOptions()
    {
        // Act
        var handler = new HttpLoggingHandler();

        // Assert
        Assert.NotNull(handler.Options);
        Assert.True(handler.Options.LogRequests);
        Assert.True(handler.Options.LogResponses);
    }

    [Fact]
    public void Constructor_WithCustomOptions_SetsCustomOptions()
    {
        // Arrange
        var options = new HttpLoggingOptions
        {
            LogRequests = false,
            LogResponses = false
        };

        // Act
        var handler = new HttpLoggingHandler(options);

        // Assert
        Assert.Same(options, handler.Options);
        Assert.False(handler.Options.LogRequests);
        Assert.False(handler.Options.LogResponses);
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HttpLoggingHandler(null!));
    }

    [Fact]
    public async Task LogRequestAsync_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.LogRequestAsync(null!, request));
    }

    [Fact]
    public async Task LogRequestAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.LogRequestAsync(_mockLogger.Object, null!));
    }

    [Fact]
    public async Task LogRequestAsync_WithLoggingDisabled_DoesNotLog()
    {
        // Arrange
        var options = new HttpLoggingOptions { LogRequests = false };
        var handler = new HttpLoggingHandler(options);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // Act
        await handler.LogRequestAsync(_mockLogger.Object, request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task LogRequestAsync_WithBasicRequest_LogsRequestDetails()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/test");
        request.Headers.Add("User-Agent", "TestAgent/1.0");

        // Act
        await _handler.LogRequestAsync(_mockLogger.Object, request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HTTP Request: POST https://api.example.com/test")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogRequestAsync_WithRequestContent_LogsContentWhenEnabled()
    {
        // Arrange
        var options = new HttpLoggingOptions { LogRequestContent = true };
        var handler = new HttpLoggingHandler(options);
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/test");
        request.Content = new StringContent("test content", Encoding.UTF8, "application/json");

        // Act
        await handler.LogRequestAsync(_mockLogger.Object, request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task LogResponseAsync_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.LogResponseAsync(null!, request, response, TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task LogResponseAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.LogResponseAsync(_mockLogger.Object, null!, response, TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task LogResponseAsync_WithNullResponse_ThrowsArgumentNullException()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.LogResponseAsync(_mockLogger.Object, request, null!, TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task LogResponseAsync_WithLoggingDisabled_DoesNotLog()
    {
        // Arrange
        var options = new HttpLoggingOptions { LogResponses = false };
        var handler = new HttpLoggingHandler(options);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await handler.LogResponseAsync(_mockLogger.Object, request, response, TimeSpan.FromMilliseconds(100));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task LogResponseAsync_WithSuccessResponse_LogsAtInformationLevel()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            ReasonPhrase = "OK"
        };

        // Act
        await _handler.LogResponseAsync(_mockLogger.Object, request, response, TimeSpan.FromMilliseconds(150));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HTTP Response: GET https://api.example.com/test responded 200 OK in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogResponseAsync_WithClientErrorResponse_LogsAtWarningLevel()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            ReasonPhrase = "Bad Request"
        };

        // Act
        await _handler.LogResponseAsync(_mockLogger.Object, request, response, TimeSpan.FromMilliseconds(100));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HTTP Response: GET https://api.example.com/test responded 400 Bad Request in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogResponseAsync_WithServerErrorResponse_LogsAtErrorLevel()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");
        using var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "Internal Server Error"
        };

        // Act
        await _handler.LogResponseAsync(_mockLogger.Object, request, response, TimeSpan.FromMilliseconds(200));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HTTP Response: GET https://api.example.com/test responded 500 Internal Server Error in")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task LogRequestExceptionAsync_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var exception = new HttpRequestException("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.LogRequestExceptionAsync(null!, request, exception, TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task LogRequestExceptionAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var exception = new HttpRequestException("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.LogRequestExceptionAsync(_mockLogger.Object, null!, exception, TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task LogRequestExceptionAsync_WithNullException_ThrowsArgumentNullException()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _handler.LogRequestExceptionAsync(_mockLogger.Object, request, null!, TimeSpan.FromMilliseconds(100)));
    }

    [Fact]
    public async Task LogRequestExceptionAsync_WithException_LogsAtErrorLevel()
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");
        var exception = new HttpRequestException("Connection timeout");

        // Act
        await _handler.LogRequestExceptionAsync(_mockLogger.Object, request, exception, TimeSpan.FromMilliseconds(30000));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HTTP Request Exception: GET https://api.example.com/test failed after 30000ms")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, LogLevel.Information)]
    [InlineData(HttpStatusCode.Created, LogLevel.Information)]
    [InlineData(HttpStatusCode.NoContent, LogLevel.Information)]
    [InlineData(HttpStatusCode.BadRequest, LogLevel.Warning)]
    [InlineData(HttpStatusCode.Unauthorized, LogLevel.Warning)]
    [InlineData(HttpStatusCode.NotFound, LogLevel.Warning)]
    [InlineData(HttpStatusCode.InternalServerError, LogLevel.Error)]
    [InlineData(HttpStatusCode.BadGateway, LogLevel.Error)]
    [InlineData(HttpStatusCode.ServiceUnavailable, LogLevel.Error)]
    public async Task LogResponseAsync_UsesCorrectLogLevel_ForDifferentStatusCodes(HttpStatusCode statusCode, LogLevel expectedLogLevel)
    {
        // Arrange
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");
        using var response = new HttpResponseMessage(statusCode);

        // Act
        await _handler.LogResponseAsync(_mockLogger.Object, request, response, TimeSpan.FromMilliseconds(100));

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                expectedLogLevel,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}