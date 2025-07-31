using System.Net;
using System.Net.Sockets;
using BrickLink.Client.Exceptions;
using BrickLink.Client.Retry;

namespace BrickLink.Client.Tests.Retry;

public sealed class EdgeCaseTests
{
    [Fact]
    public void ExponentialBackoffRetryPolicy_WithZeroJitterFactor_DoesNotApplyJitter()
    {
        // Arrange
        var options = new RetryPolicyOptions
        {
            BaseDelay = TimeSpan.FromSeconds(1),
            JitterFactor = 0.0
        };
        var policy = new ExponentialBackoffRetryPolicy(options);

        // Act
        var delay1 = policy.GetRetryDelay(1);
        var delay2 = policy.GetRetryDelay(1);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(1), delay1);
        Assert.Equal(delay1, delay2); // Should be identical with no jitter
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithInheritedSocketException_ReturnsTrue()
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
    public void ExponentialBackoffRetryPolicy_WithHttpRequestExceptionContainingTimeout_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("The operation timed out");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithHttpRequestExceptionContainingConnection_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("Connection failed");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithHttpRequestExceptionContainingNetwork_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("Network error occurred");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithHttpRequestExceptionContainingHost_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("Host not found");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithHttpRequestExceptionContainingDns_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("DNS resolution failed");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithTaskCanceledExceptionWithTimeoutMessage_ReturnsTrue()
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new TaskCanceledException("The operation timed out");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithNonTransientHttpRequestException_ReturnsTrue()
    {
        // Arrange - HttpRequestException is in the default retriable types
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new HttpRequestException("Bad request format");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert - Should return true because HttpRequestException is in the default retriable types
        Assert.True(shouldRetry);
    }

    [Fact]
    public void ExponentialBackoffRetryPolicy_WithTaskCanceledExceptionWithoutTimeoutMessage_ReturnsTrue()
    {
        // Arrange - TaskCanceledException is in the default retriable types
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new TaskCanceledException("User canceled operation");

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert - Should return true because TaskCanceledException is in the default retriable types
        Assert.True(shouldRetry);
    }

    [Theory]
    [InlineData(SocketError.ConnectionRefused)]
    [InlineData(SocketError.ConnectionReset)]
    [InlineData(SocketError.ConnectionAborted)]
    [InlineData(SocketError.NetworkDown)]
    [InlineData(SocketError.NetworkUnreachable)]
    [InlineData(SocketError.HostDown)]
    [InlineData(SocketError.HostUnreachable)]
    [InlineData(SocketError.TimedOut)]
    [InlineData(SocketError.TryAgain)]
    public void ExponentialBackoffRetryPolicy_WithTransientSocketErrors_ReturnsTrue(SocketError socketError)
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new SocketException((int)socketError);

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.True(shouldRetry);
    }

    [Theory]
    [InlineData(SocketError.AccessDenied)]
    [InlineData(SocketError.AddressAlreadyInUse)]
    [InlineData(SocketError.InvalidArgument)]
    public void ExponentialBackoffRetryPolicy_WithNonTransientSocketErrors_ReturnsFalse(SocketError socketError)
    {
        // Arrange
        var policy = new ExponentialBackoffRetryPolicy();
        var exception = new SocketException((int)socketError);

        // Act
        var shouldRetry = policy.ShouldRetry(exception, 1);

        // Assert
        Assert.False(shouldRetry);
    }
}
