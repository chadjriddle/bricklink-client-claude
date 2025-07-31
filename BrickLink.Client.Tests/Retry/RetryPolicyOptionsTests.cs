using System.Net;
using System.Net.Sockets;
using BrickLink.Client.Retry;

namespace BrickLink.Client.Tests.Retry;

public sealed class RetryPolicyOptionsTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Act
        var options = new RetryPolicyOptions();

        // Assert
        Assert.Equal(RetryPolicyOptions.DefaultMaxRetryAttempts, options.MaxRetryAttempts);
        Assert.Equal(RetryPolicyOptions.DefaultBaseDelay, options.BaseDelay);
        Assert.Equal(RetryPolicyOptions.DefaultMaxDelay, options.MaxDelay);
        Assert.Equal(0.1, options.JitterFactor);
        Assert.NotNull(options.RetriableStatusCodes);
        Assert.NotNull(options.RetriableExceptionTypes);
    }

    [Fact]
    public void DefaultRetriableStatusCodes_ContainsExpectedValues()
    {
        // Act
        var options = new RetryPolicyOptions();

        // Assert
        var expectedStatusCodes = new HashSet<HttpStatusCode>
        {
            HttpStatusCode.InternalServerError,      // 500
            HttpStatusCode.BadGateway,               // 502
            HttpStatusCode.ServiceUnavailable,       // 503
            HttpStatusCode.GatewayTimeout,           // 504
            HttpStatusCode.RequestTimeout,           // 408
            HttpStatusCode.TooManyRequests           // 429
        };

        Assert.Equal(expectedStatusCodes, options.RetriableStatusCodes);
    }

    [Fact]
    public void DefaultRetriableExceptionTypes_ContainsExpectedValues()
    {
        // Act
        var options = new RetryPolicyOptions();

        // Assert
        var expectedExceptionTypes = new HashSet<Type>
        {
            typeof(HttpRequestException),
            typeof(TaskCanceledException),
            typeof(TimeoutException),
            typeof(SocketException)
        };

        Assert.Equal(expectedExceptionTypes, options.RetriableExceptionTypes);
    }

    [Fact]
    public void MaxRetryAttempts_CanBeSet()
    {
        // Arrange
        var options = new RetryPolicyOptions();
        const int expectedAttempts = 5;

        // Act
        options.MaxRetryAttempts = expectedAttempts;

        // Assert
        Assert.Equal(expectedAttempts, options.MaxRetryAttempts);
    }

    [Fact]
    public void BaseDelay_CanBeSet()
    {
        // Arrange
        var options = new RetryPolicyOptions();
        var expectedDelay = TimeSpan.FromSeconds(2);

        // Act
        options.BaseDelay = expectedDelay;

        // Assert
        Assert.Equal(expectedDelay, options.BaseDelay);
    }

    [Fact]
    public void MaxDelay_CanBeSet()
    {
        // Arrange
        var options = new RetryPolicyOptions();
        var expectedDelay = TimeSpan.FromMinutes(1);

        // Act
        options.MaxDelay = expectedDelay;

        // Assert
        Assert.Equal(expectedDelay, options.MaxDelay);
    }

    [Fact]
    public void JitterFactor_CanBeSet()
    {
        // Arrange
        var options = new RetryPolicyOptions();
        const double expectedJitter = 0.2;

        // Act
        options.JitterFactor = expectedJitter;

        // Assert
        Assert.Equal(expectedJitter, options.JitterFactor);
    }

    [Fact]
    public void RetriableStatusCodes_CanBeModified()
    {
        // Arrange
        var options = new RetryPolicyOptions();
        var customStatusCode = HttpStatusCode.Conflict;

        // Act
        options.RetriableStatusCodes.Add(customStatusCode);

        // Assert
        Assert.Contains(customStatusCode, options.RetriableStatusCodes);
    }

    [Fact]
    public void RetriableExceptionTypes_CanBeModified()
    {
        // Arrange
        var options = new RetryPolicyOptions();
        var customExceptionType = typeof(ArgumentException);

        // Act
        options.RetriableExceptionTypes.Add(customExceptionType);

        // Assert
        Assert.Contains(customExceptionType, options.RetriableExceptionTypes);
    }

    [Fact]
    public void Clone_CreatesIndependentCopy()
    {
        // Arrange
        var originalOptions = new RetryPolicyOptions
        {
            MaxRetryAttempts = 5,
            BaseDelay = TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromMinutes(2),
            JitterFactor = 0.2
        };
        originalOptions.RetriableStatusCodes.Add(HttpStatusCode.Conflict);
        originalOptions.RetriableExceptionTypes.Add(typeof(ArgumentException));

        // Act
        var clonedOptions = originalOptions.Clone();

        // Assert
        Assert.Equal(originalOptions.MaxRetryAttempts, clonedOptions.MaxRetryAttempts);
        Assert.Equal(originalOptions.BaseDelay, clonedOptions.BaseDelay);
        Assert.Equal(originalOptions.MaxDelay, clonedOptions.MaxDelay);
        Assert.Equal(originalOptions.JitterFactor, clonedOptions.JitterFactor);
        Assert.Equal(originalOptions.RetriableStatusCodes, clonedOptions.RetriableStatusCodes);
        Assert.Equal(originalOptions.RetriableExceptionTypes, clonedOptions.RetriableExceptionTypes);

        // Verify independence
        clonedOptions.MaxRetryAttempts = 10;
        clonedOptions.RetriableStatusCodes.Add(HttpStatusCode.PaymentRequired);

        Assert.NotEqual(originalOptions.MaxRetryAttempts, clonedOptions.MaxRetryAttempts);
        Assert.DoesNotContain(HttpStatusCode.PaymentRequired, originalOptions.RetriableStatusCodes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void MaxRetryAttempts_AcceptsValidValues(int attempts)
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act & Assert (should not throw)
        options.MaxRetryAttempts = attempts;
        Assert.Equal(attempts, options.MaxRetryAttempts);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void JitterFactor_AcceptsValidValues(double jitter)
    {
        // Arrange
        var options = new RetryPolicyOptions();

        // Act & Assert (should not throw)
        options.JitterFactor = jitter;
        Assert.Equal(jitter, options.JitterFactor);
    }
}
