using System.Net;
using System.Net.Sockets;
using BrickLink.Client.Exceptions;
using BrickLink.Client.Retry;

namespace BrickLink.Client.Tests.Retry;

public sealed class ExponentialBackoffRetryPolicyTests
{
    [Fact]
    public void Constructor_WithNullOptions_UsesDefaults()
    {
        // Act
        var policy = new ExponentialBackoffRetryPolicy(null);

        // Assert
        Assert.Equal(RetryPolicyOptions.DefaultMaxRetryAttempts, policy.MaxRetryAttempts);
    }

    [Fact]
    public void Constructor_WithCustomOptions_UsesProvidedValues()
    {
        // Arrange
        var options = new RetryPolicyOptions { MaxRetryAttempts = 5 };

        // Act
        var policy = new ExponentialBackoffRetryPolicy(options);

        // Assert
        Assert.Equal(5, policy.MaxRetryAttempts);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Constructor_WithNegativeMaxRetryAttempts_ThrowsArgumentException(int maxRetryAttempts)
    {
        // Arrange
        var options = new RetryPolicyOptions { MaxRetryAttempts = maxRetryAttempts };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ExponentialBackoffRetryPolicy(options));
        Assert.Contains("MaxRetryAttempts cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativeBaseDelay_ThrowsArgumentException()
    {
        // Arrange
        var options = new RetryPolicyOptions { BaseDelay = TimeSpan.FromSeconds(-1) };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ExponentialBackoffRetryPolicy(options));
        Assert.Contains("BaseDelay cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativeMaxDelay_ThrowsArgumentException()
    {
        // Arrange
        var options = new RetryPolicyOptions { MaxDelay = TimeSpan.FromSeconds(-1) };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ExponentialBackoffRetryPolicy(options));
        Assert.Contains("MaxDelay cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_WithBaseDelayGreaterThanMaxDelay_ThrowsArgumentException()
    {
        // Arrange
        var options = new RetryPolicyOptions
        {
            BaseDelay = TimeSpan.FromSeconds(10),
            MaxDelay = TimeSpan.FromSeconds(5)
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ExponentialBackoffRetryPolicy(options));
        Assert.Contains("BaseDelay cannot be greater than MaxDelay", exception.Message);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void Constructor_WithInvalidJitterFactor_ThrowsArgumentException(double jitterFactor)
    {
        // Arrange
        var options = new RetryPolicyOptions { JitterFactor = jitterFactor };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ExponentialBackoffRetryPolicy(options));
        Assert.Contains("JitterFactor must be between 0.0 and 1.0", exception.Message);
    }

    [Fact]
    public void ShouldRetry_WithNullException_ReturnsFalse()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();

        // Act
        var shouldRetry = policy.ShouldRetry((Exception)null!, 1);

        // Assert
        Assert.False(shouldRetry);
    }

    [Fact]
    public void ShouldRetry_WithAttemptCountExceedingMax_ReturnsFalse()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("Test exception");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, policy.MaxRetryAttempts + 1);

        // Assert
        Assert.False(shouldRetry);
    }

    [Fact]
    public void ShouldRetry_WithBrickLinkApiException_ChecksStatusCode()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var apiException = new BrickLinkApiException("Test", HttpStatusCode.InternalServerError, 500);

        // Act
        var shouldRetry = policy.ShouldRetry(apiException, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError, true)]
    [InlineData(HttpStatusCode.BadGateway, true)]
    [InlineData(HttpStatusCode.ServiceUnavailable, true)]
    [InlineData(HttpStatusCode.GatewayTimeout, true)]
    [InlineData(HttpStatusCode.RequestTimeout, true)]
    [InlineData(HttpStatusCode.TooManyRequests, true)]
    [InlineData(HttpStatusCode.BadRequest, false)]
    [InlineData(HttpStatusCode.Unauthorized, false)]
    [InlineData(HttpStatusCode.NotFound, false)]
    public void ShouldRetry_WithHttpStatusCode_ReturnsExpectedResult(HttpStatusCode statusCode, bool expectedResult)
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();

        // Act
        var shouldRetry = policy.ShouldRetry(statusCode, 1);

        // Assert
        Assert.Equal(expectedResult, shouldRetry);
    }

    [Fact]
    public void ShouldRetry_WithHttpStatusCodeExceedingMaxAttempts_ReturnsFalse()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();

        // Act
        var shouldRetry = policy.ShouldRetry(HttpStatusCode.InternalServerError, policy.MaxRetryAttempts + 1);

        // Assert
        Assert.False(shouldRetry);
    }

    [Theory]
    [InlineData(typeof(HttpRequestException))]
    [InlineData(typeof(TaskCanceledException))]
    [InlineData(typeof(TimeoutException))]
    public void ShouldRetry_WithRetriableExceptionType_ReturnsTrue(Type exceptionType)
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception")!;

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ShouldRetry_WithRetriableSocketException_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new SocketException((int)SocketError.ConnectionRefused);

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ShouldRetry_WithNonRetriableException_ReturnsFalse()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new ArgumentException("Test exception");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.False(shouldRetry);
    }

    [Theory]
    [InlineData(SocketError.ConnectionRefused, true)]
    [InlineData(SocketError.ConnectionReset, true)]
    [InlineData(SocketError.ConnectionAborted, true)]
    [InlineData(SocketError.NetworkDown, true)]
    [InlineData(SocketError.NetworkUnreachable, true)]
    [InlineData(SocketError.HostDown, true)]
    [InlineData(SocketError.HostUnreachable, true)]
    [InlineData(SocketError.TimedOut, true)]
    [InlineData(SocketError.TryAgain, true)]
    [InlineData(SocketError.AccessDenied, false)]
    [InlineData(SocketError.AddressAlreadyInUse, false)]
    public void ShouldRetry_WithSocketException_ReturnsExpectedResult(SocketError socketError, bool expectedResult)
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new SocketException((int)socketError);

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.Equal(expectedResult, shouldRetry);
    }

    [Theory]
    [InlineData(1, 1.0)] // First attempt: baseDelay * 2^0 = 1 second
    [InlineData(2, 2.0)] // Second attempt: baseDelay * 2^1 = 2 seconds  
    [InlineData(3, 4.0)] // Third attempt: baseDelay * 2^2 = 4 seconds
    [InlineData(4, 8.0)] // Fourth attempt: baseDelay * 2^3 = 8 seconds
    public void GetRetryDelay_WithNoJitter_ReturnsExponentialBackoffDelay(int attemptCount, double expectedSeconds)
    {
        // Arrange
        var options = new RetryPolicyOptions
        {
            BaseDelay = TimeSpan.FromSeconds(1),
            JitterFactor = 0.0 // No jitter
        };
        var policy = new ExponentialBackoffRetryPolicy(options);

        // Act
        var delay = policy.GetRetryDelay(attemptCount);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), delay);
    }

    [Fact]
    public void GetRetryDelay_WithMaxDelayReached_ReturnsMaxDelay()
    {
        // Arrange
        var options = new RetryPolicyOptions
        {
            BaseDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(5),
            JitterFactor = 0.0 // No jitter
        };
        var policy = new ExponentialBackoffRetryPolicy(options);

        // Act
        var delay = policy.GetRetryDelay(10); // Should be capped at MaxDelay

        // Assert
        Assert.Equal(options.MaxDelay, delay);
    }

    [Fact]
    public void GetRetryDelay_WithNegativeAttemptCount_ReturnsZero()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();

        // Act
        var delay = policy.GetRetryDelay(-1);

        // Assert
        Assert.Equal(TimeSpan.Zero, delay);
    }

    [Fact]
    public void GetRetryDelay_WithZeroAttemptCount_ReturnsZero()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();

        // Act
        var delay = policy.GetRetryDelay(0);

        // Assert
        Assert.Equal(TimeSpan.Zero, delay);
    }

    [Fact]
    public void GetRetryDelay_WithJitter_ReturnsDelayWithinJitterRange()
    {
        // Arrange
        var options = new RetryPolicyOptions
        {
            BaseDelay = TimeSpan.FromSeconds(1),
            JitterFactor = 0.2 // 20% jitter
        };
        var policy = new ExponentialBackoffRetryPolicy(options);
        var expectedBaseDelay = TimeSpan.FromSeconds(1); // First attempt
        var minDelay = TimeSpan.FromMilliseconds(expectedBaseDelay.TotalMilliseconds * 0.8); // -20%
        var maxDelay = TimeSpan.FromMilliseconds(expectedBaseDelay.TotalMilliseconds * 1.2); // +20%

        // Act & Assert (run multiple times due to randomness)
        for (int i = 0; i < 100; i++)
        {
            var delay = policy.GetRetryDelay(1);
            Assert.True(delay >= minDelay && delay <= maxDelay,
                $"Delay {delay.TotalMilliseconds}ms should be between {minDelay.TotalMilliseconds}ms and {maxDelay.TotalMilliseconds}ms");
        }
    }

    [Fact]
    public void ShouldRetry_WithTransientHttpRequestException_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("The request timed out");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ShouldRetry_WithTimeoutTaskCanceledException_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var innerException = new TimeoutException("Operation timed out");
        var exception = new TaskCanceledException("Task was canceled", innerException);

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }
}
