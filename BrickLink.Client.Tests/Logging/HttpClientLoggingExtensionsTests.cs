using BrickLink.Client.Logging;
using Microsoft.Extensions.Logging;
using Moq;

namespace BrickLink.Client.Tests.Logging;

public class HttpClientLoggingExtensionsTests
{
    private readonly Mock<ILogger<LoggingDelegatingHandler>> _mockLogger;
    private readonly Mock<IHttpLoggingHandler> _mockLoggingHandler;

    public HttpClientLoggingExtensionsTests()
    {
        _mockLogger = new Mock<ILogger<LoggingDelegatingHandler>>();
        _mockLoggingHandler = new Mock<IHttpLoggingHandler>();
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerAndLoggingHandler_ReturnsConfiguredHandler()
    {
        // Act
        var handler = HttpClientLoggingExtensions.CreateLoggingHandler(_mockLogger.Object, _mockLoggingHandler.Object);

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<LoggingDelegatingHandler>(handler);
        Assert.Same(_mockLoggingHandler.Object, handler.LoggingHandler);
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerAndLoggingHandler_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingHandler(null!, _mockLoggingHandler.Object));
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerAndLoggingHandler_NullLoggingHandler_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingHandler(_mockLogger.Object, (IHttpLoggingHandler)null!));
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerOnly_ReturnsHandlerWithDefaultLogging()
    {
        // Act
        var handler = HttpClientLoggingExtensions.CreateLoggingHandler(_mockLogger.Object);

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<LoggingDelegatingHandler>(handler);
        Assert.NotNull(handler.LoggingHandler);
        Assert.IsType<HttpLoggingHandler>(handler.LoggingHandler);
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerOnly_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingHandler(null!));
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerAndOptions_ReturnsConfiguredHandler()
    {
        // Arrange
        var options = new HttpLoggingOptions
        {
            LogRequests = false,
            LogResponses = true,
            MaxContentLogSize = 2048
        };

        // Act
        var handler = HttpClientLoggingExtensions.CreateLoggingHandler(_mockLogger.Object, options);

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<LoggingDelegatingHandler>(handler);
        Assert.NotNull(handler.LoggingHandler);
        Assert.IsType<HttpLoggingHandler>(handler.LoggingHandler);

        var httpLoggingHandler = (HttpLoggingHandler)handler.LoggingHandler;
        Assert.Same(options, httpLoggingHandler.Options);
        Assert.False(httpLoggingHandler.Options.LogRequests);
        Assert.True(httpLoggingHandler.Options.LogResponses);
        Assert.Equal(2048, httpLoggingHandler.Options.MaxContentLogSize);
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerAndOptions_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new HttpLoggingOptions();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingHandler(null!, options));
    }

    [Fact]
    public void CreateLoggingHandler_WithLoggerAndOptions_NullOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingHandler(_mockLogger.Object, (HttpLoggingOptions)null!));
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerAndLoggingHandler_ReturnsConfiguredPipeline()
    {
        // Act
        var pipeline = HttpClientLoggingExtensions.CreateLoggingPipeline(_mockLogger.Object, _mockLoggingHandler.Object);

        // Assert
        Assert.NotNull(pipeline);
        Assert.IsType<LoggingDelegatingHandler>(pipeline);

        var loggingHandler = (LoggingDelegatingHandler)pipeline;
        Assert.Same(_mockLoggingHandler.Object, loggingHandler.LoggingHandler);
        Assert.NotNull(loggingHandler.InnerHandler);
        Assert.IsType<HttpClientHandler>(loggingHandler.InnerHandler);
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerAndLoggingHandlerAndInnerHandler_ReturnsConfiguredPipeline()
    {
        // Arrange
        var innerHandler = new HttpClientHandler();

        // Act
        var pipeline = HttpClientLoggingExtensions.CreateLoggingPipeline(_mockLogger.Object, _mockLoggingHandler.Object, innerHandler);

        // Assert
        Assert.NotNull(pipeline);
        Assert.IsType<LoggingDelegatingHandler>(pipeline);

        var loggingHandler = (LoggingDelegatingHandler)pipeline;
        Assert.Same(_mockLoggingHandler.Object, loggingHandler.LoggingHandler);
        Assert.Same(innerHandler, loggingHandler.InnerHandler);

        pipeline.Dispose();
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerAndLoggingHandler_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingPipeline(null!, _mockLoggingHandler.Object));
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerAndLoggingHandler_NullLoggingHandler_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingPipeline(_mockLogger.Object, (IHttpLoggingHandler)null!));
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerOnly_ReturnsConfiguredPipeline()
    {
        // Act
        var pipeline = HttpClientLoggingExtensions.CreateLoggingPipeline(_mockLogger.Object);

        // Assert
        Assert.NotNull(pipeline);
        Assert.IsType<LoggingDelegatingHandler>(pipeline);

        var loggingHandler = (LoggingDelegatingHandler)pipeline;
        Assert.NotNull(loggingHandler.LoggingHandler);
        Assert.IsType<HttpLoggingHandler>(loggingHandler.LoggingHandler);
        Assert.NotNull(loggingHandler.InnerHandler);
        Assert.IsType<HttpClientHandler>(loggingHandler.InnerHandler);

        pipeline.Dispose();
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerOnlyAndInnerHandler_ReturnsConfiguredPipeline()
    {
        // Arrange
        var innerHandler = new HttpClientHandler();

        // Act
        var pipeline = HttpClientLoggingExtensions.CreateLoggingPipeline(_mockLogger.Object, innerHandler);

        // Assert
        Assert.NotNull(pipeline);
        Assert.IsType<LoggingDelegatingHandler>(pipeline);

        var loggingHandler = (LoggingDelegatingHandler)pipeline;
        Assert.NotNull(loggingHandler.LoggingHandler);
        Assert.IsType<HttpLoggingHandler>(loggingHandler.LoggingHandler);
        Assert.Same(innerHandler, loggingHandler.InnerHandler);

        pipeline.Dispose();
    }

    [Fact]
    public void CreateLoggingPipeline_WithLoggerOnly_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            HttpClientLoggingExtensions.CreateLoggingPipeline(null!, (HttpMessageHandler?)null));
    }
}
