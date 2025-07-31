using System.Net;
using BrickLink.Client.Logging;
using Microsoft.Extensions.Logging;
using Moq;

namespace BrickLink.Client.Tests.Logging;

public class LoggingDelegatingHandlerTests
{
    private readonly Mock<ILogger<LoggingDelegatingHandler>> _mockLogger;
    private readonly Mock<IHttpLoggingHandler> _mockLoggingHandler;

    public LoggingDelegatingHandlerTests()
    {
        _mockLogger = new Mock<ILogger<LoggingDelegatingHandler>>();
        _mockLoggingHandler = new Mock<IHttpLoggingHandler>();
    }

    [Fact]
    public void Constructor_WithLoggerAndLoggingHandler_SetsProperties()
    {
        // Act
        var handler = new LoggingDelegatingHandler(_mockLogger.Object, _mockLoggingHandler.Object);

        // Assert
        Assert.Same(_mockLoggingHandler.Object, handler.LoggingHandler);
    }

    [Fact]
    public void Constructor_WithLoggerOnly_CreatesDefaultLoggingHandler()
    {
        // Act
        var handler = new LoggingDelegatingHandler(_mockLogger.Object);

        // Assert
        Assert.NotNull(handler.LoggingHandler);
        Assert.IsType<HttpLoggingHandler>(handler.LoggingHandler);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LoggingDelegatingHandler(null!, _mockLoggingHandler.Object));
    }

    [Fact]
    public void Constructor_WithNullLoggingHandler_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new LoggingDelegatingHandler(_mockLogger.Object, null!));
    }

    [Fact]
    public void Dispose_WithDisposableLoggingHandler_DisposesLoggingHandler()
    {
        // Arrange
        var mockDisposableLoggingHandler = new Mock<IHttpLoggingHandler>();
        mockDisposableLoggingHandler.As<IDisposable>();

        var handler = new LoggingDelegatingHandler(_mockLogger.Object, mockDisposableLoggingHandler.Object);

        // Act
        handler.Dispose();

        // Assert
        mockDisposableLoggingHandler.As<IDisposable>().Verify(x => x.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_WithNonDisposableLoggingHandler_DoesNotThrow()
    {
        // Arrange
        var handler = new LoggingDelegatingHandler(_mockLogger.Object, _mockLoggingHandler.Object);

        // Act & Assert - Should not throw
        handler.Dispose();
    }

    [Fact]
    public async Task SendAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var testHandler = new TestHttpMessageHandler();
        var handler = new LoggingDelegatingHandler(_mockLogger.Object, _mockLoggingHandler.Object)
        {
            InnerHandler = testHandler
        };

        var httpClient = new HttpClient(handler);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => httpClient.SendAsync(null!));

        httpClient.Dispose();
    }

    [Fact]
    public async Task SendAsync_CallsLoggingHandler_ForRequestAndResponse()
    {
        // Arrange
        var testHandler = new TestHttpMessageHandler(() => new HttpResponseMessage(HttpStatusCode.OK));
        var handler = new LoggingDelegatingHandler(_mockLogger.Object, _mockLoggingHandler.Object)
        {
            InnerHandler = testHandler
        };

        var httpClient = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");

        // Act
        var response = await httpClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        _mockLoggingHandler.Verify(
            x => x.LogRequestAsync(_mockLogger.Object, request, It.IsAny<CancellationToken>()),
            Times.Once);

        _mockLoggingHandler.Verify(
            x => x.LogResponseAsync(_mockLogger.Object, request, It.IsAny<HttpResponseMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockLoggingHandler.Verify(
            x => x.LogRequestExceptionAsync(It.IsAny<ILogger>(), It.IsAny<HttpRequestMessage>(), It.IsAny<Exception>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Never);

        httpClient.Dispose();
        response.Dispose();
    }

    [Fact]
    public async Task SendAsync_CallsLoggingHandler_ForException()
    {
        // Arrange
        var exception = new HttpRequestException("Test exception");
        var testHandler = new TestHttpMessageHandler(() => throw exception);
        var handler = new LoggingDelegatingHandler(_mockLogger.Object, _mockLoggingHandler.Object)
        {
            InnerHandler = testHandler
        };

        var httpClient = new HttpClient(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/test");

        // Act & Assert
        var thrownException = await Assert.ThrowsAsync<HttpRequestException>(() => httpClient.SendAsync(request));
        Assert.Same(exception, thrownException);

        _mockLoggingHandler.Verify(
            x => x.LogRequestAsync(_mockLogger.Object, request, It.IsAny<CancellationToken>()),
            Times.Once);

        _mockLoggingHandler.Verify(
            x => x.LogRequestExceptionAsync(_mockLogger.Object, request, exception, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockLoggingHandler.Verify(
            x => x.LogResponseAsync(It.IsAny<ILogger>(), It.IsAny<HttpRequestMessage>(), It.IsAny<HttpResponseMessage>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Never);

        httpClient.Dispose();
    }

    /// <summary>
    /// Test implementation of HttpMessageHandler for testing purposes.
    /// </summary>
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpResponseMessage> _responseFactory;

        public TestHttpMessageHandler(Func<HttpResponseMessage>? responseFactory = null)
        {
            _responseFactory = responseFactory ?? (() => new HttpResponseMessage(HttpStatusCode.OK));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory());
        }
    }
}