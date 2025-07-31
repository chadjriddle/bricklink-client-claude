using System.Net;
using BrickLink.Client.Exceptions;
using BrickLink.Client.Retry;
using Moq;

namespace BrickLink.Client.Tests.Retry;

public sealed class RetryHandlerTests
{
    [Fact]
    public void Constructor_WithNullRetryPolicy_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RetryHandler(null!));
    }

    [Fact]
    public void Constructor_WithDefaultParameterless_UsesDefaultPolicy()
    {
        // Act
        var handler = new RetryHandler();

        // Assert
        Assert.NotNull(handler.RetryPolicy);
        Assert.IsType<ExponentialBackoffRetryPolicy>(handler.RetryPolicy);
    }

    [Fact]
    public void Constructor_WithCustomRetryPolicy_UsesProvidedPolicy()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();

        // Act
        var handler = new RetryHandler(mockPolicy.Object);

        // Assert
        Assert.Same(mockPolicy.Object, handler.RetryPolicy);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullOperation_ThrowsArgumentNullException()
    {
        // Arrange
        var handler = new RetryHandler();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.ExecuteAsync((Func<Task<int>>)null!));
    }

    [Fact]
    public async Task ExecuteAsync_WithVoidNullOperation_ThrowsArgumentNullException()
    {
        // Arrange
        var handler = new RetryHandler();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.ExecuteAsync((Func<Task>)null!));
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulOperation_ReturnsResult()
    {
        // Arrange
        var handler = new RetryHandler();
        const int expectedResult = 42;
        var operation = new Func<Task<int>>(() => Task.FromResult(expectedResult));

        // Act
        var result = await handler.ExecuteAsync(operation);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulVoidOperation_Completes()
    {
        // Arrange
        var handler = new RetryHandler();
        var executed = false;
        var operation = new Func<Task>(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        // Act
        await handler.ExecuteAsync(operation);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task ExecuteAsync_WithRetriableException_RetriesAndSucceeds()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(3);
        mockPolicy.Setup(p => p.ShouldRetry(It.IsAny<Exception>(), It.IsAny<int>())).Returns(true);
        mockPolicy.Setup(p => p.GetRetryDelay(It.IsAny<int>())).Returns(TimeSpan.Zero);

        var handler = new RetryHandler(mockPolicy.Object);
        var callCount = 0;
        var operation = new Func<Task<int>>(() =>
        {
            callCount++;
            if (callCount < 3)
            {
                throw new HttpRequestException("Transient error");
            }
            return Task.FromResult(42);
        });

        // Act
        var result = await handler.ExecuteAsync(operation);

        // Assert
        Assert.Equal(42, result);
        Assert.Equal(3, callCount);
        mockPolicy.Verify(p => p.ShouldRetry(It.IsAny<HttpRequestException>(), It.IsAny<int>()), Times.Exactly(2));
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonRetriableException_ThrowsImmediately()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(3);
        mockPolicy.Setup(p => p.ShouldRetry(It.IsAny<Exception>(), It.IsAny<int>())).Returns(false);

        var handler = new RetryHandler(mockPolicy.Object);
        var expectedException = new ArgumentException("Non-retriable error");
        var operation = new Func<Task<int>>(() => throw expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<ArgumentException>(() => handler.ExecuteAsync(operation));
        Assert.Same(expectedException, actualException);

        mockPolicy.Verify(p => p.ShouldRetry(expectedException, 1), Times.Once);
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithExhaustedRetries_ThrowsLastException()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(2);
        mockPolicy.Setup(p => p.ShouldRetry(It.IsAny<Exception>(), It.IsAny<int>())).Returns(true);
        mockPolicy.Setup(p => p.GetRetryDelay(It.IsAny<int>())).Returns(TimeSpan.Zero);

        var handler = new RetryHandler(mockPolicy.Object);
        var expectedException = new HttpRequestException("Persistent error");
        var operation = new Func<Task<int>>(() => throw expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(() => handler.ExecuteAsync(operation));
        Assert.Same(expectedException, actualException);

        mockPolicy.Verify(p => p.ShouldRetry(expectedException, It.IsAny<int>()), Times.Exactly(2));
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_ThrowsOnCancellation()
    {
        // Arrange
        var handler = new RetryHandler();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var operation = new Func<Task<int>>(() => Task.FromResult(42));

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => handler.ExecuteAsync(operation, cts.Token));
    }

    [Fact]
    public async Task ExecuteHttpAsync_WithNullOperation_ThrowsArgumentNullException()
    {
        // Arrange
        var handler = new RetryHandler();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.ExecuteHttpAsync(null!));
    }

    [Fact]
    public async Task ExecuteHttpAsync_WithSuccessfulResponse_ReturnsResponse()
    {
        // Arrange
        var handler = new RetryHandler();
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var operation = new Func<Task<HttpResponseMessage>>(() => Task.FromResult(expectedResponse));

        // Act
        var result = await handler.ExecuteHttpAsync(operation);

        // Assert
        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task ExecuteHttpAsync_WithRetriableStatusCode_RetriesAndSucceeds()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(3);
        mockPolicy.Setup(p => p.ShouldRetry(HttpStatusCode.InternalServerError, It.IsAny<int>())).Returns(true);
        mockPolicy.Setup(p => p.ShouldRetry(HttpStatusCode.OK, It.IsAny<int>())).Returns(false);
        mockPolicy.Setup(p => p.GetRetryDelay(It.IsAny<int>())).Returns(TimeSpan.Zero);

        var handler = new RetryHandler(mockPolicy.Object);
        var callCount = 0;
        var operation = new Func<Task<HttpResponseMessage>>(() =>
        {
            callCount++;
            if (callCount < 3)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        // Act
        var result = await handler.ExecuteHttpAsync(operation);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(3, callCount);
        mockPolicy.Verify(p => p.ShouldRetry(HttpStatusCode.InternalServerError, It.IsAny<int>()), Times.Exactly(2));
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteHttpAsync_WithNonRetriableStatusCode_ReturnsResponseImmediately()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(3);
        mockPolicy.Setup(p => p.ShouldRetry(HttpStatusCode.BadRequest, It.IsAny<int>())).Returns(false);

        var handler = new RetryHandler(mockPolicy.Object);
        var expectedResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var operation = new Func<Task<HttpResponseMessage>>(() => Task.FromResult(expectedResponse));

        // Act
        var result = await handler.ExecuteHttpAsync(operation);

        // Assert
        Assert.Same(expectedResponse, result);
        mockPolicy.Verify(p => p.ShouldRetry(HttpStatusCode.BadRequest, 1), Times.Once);
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteHttpAsync_WithExhaustedRetries_ThrowsBrickLinkApiException()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(2);
        mockPolicy.Setup(p => p.ShouldRetry(HttpStatusCode.InternalServerError, It.IsAny<int>())).Returns(true);
        mockPolicy.Setup(p => p.GetRetryDelay(It.IsAny<int>())).Returns(TimeSpan.Zero);

        var handler = new RetryHandler(mockPolicy.Object);
        var operation = new Func<Task<HttpResponseMessage>>(() =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BrickLinkApiException>(() => handler.ExecuteHttpAsync(operation));
        Assert.Contains("HTTP request failed with status code InternalServerError", exception.Message);
        Assert.Contains("2 retry attempts", exception.Message);

        mockPolicy.Verify(p => p.ShouldRetry(HttpStatusCode.InternalServerError, It.IsAny<int>()), Times.Exactly(2));
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteHttpAsync_WithExceptionDuringRetry_ThrowsOriginalException()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(2);
        mockPolicy.Setup(p => p.ShouldRetry(It.IsAny<Exception>(), It.IsAny<int>())).Returns(true);
        mockPolicy.Setup(p => p.GetRetryDelay(It.IsAny<int>())).Returns(TimeSpan.Zero);

        var handler = new RetryHandler(mockPolicy.Object);
        var expectedException = new HttpRequestException("Network error");
        var operation = new Func<Task<HttpResponseMessage>>(() => throw expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<HttpRequestException>(() => handler.ExecuteHttpAsync(operation));
        Assert.Same(expectedException, actualException);

        mockPolicy.Verify(p => p.ShouldRetry(expectedException, It.IsAny<int>()), Times.Exactly(2));
        mockPolicy.Verify(p => p.GetRetryDelay(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteHttpAsync_DisposesFailedResponses()
    {
        // Arrange
        var mockPolicy = new Mock<IRetryPolicy>();
        mockPolicy.Setup(p => p.MaxRetryAttempts).Returns(2);
        mockPolicy.Setup(p => p.ShouldRetry(HttpStatusCode.InternalServerError, It.IsAny<int>())).Returns(true);
        mockPolicy.Setup(p => p.GetRetryDelay(It.IsAny<int>())).Returns(TimeSpan.Zero);

        var handler = new RetryHandler(mockPolicy.Object);
        var disposedResponses = new List<HttpResponseMessage>();
        var callCount = 0;

        var operation = new Func<Task<HttpResponseMessage>>(() =>
        {
            callCount++;
            if (callCount < 3)
            {
                var failedResponse = new TestHttpResponseMessage(HttpStatusCode.InternalServerError);
                failedResponse.OnDispose = () => disposedResponses.Add(failedResponse);
                return Task.FromResult((HttpResponseMessage)failedResponse);
            }
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        // Act
        var result = await handler.ExecuteHttpAsync(operation);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, disposedResponses.Count);
        Assert.All(disposedResponses, response => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode));
    }

    private class TestHttpResponseMessage : HttpResponseMessage
    {
        public Action? OnDispose { get; set; }

        public TestHttpResponseMessage(HttpStatusCode statusCode) : base(statusCode) { }

        protected override void Dispose(bool disposing)
        {
            OnDispose?.Invoke();
            base.Dispose(disposing);
        }
    }
}
