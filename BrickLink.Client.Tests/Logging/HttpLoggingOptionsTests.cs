using BrickLink.Client.Logging;
using Microsoft.Extensions.Logging;

namespace BrickLink.Client.Tests.Logging;

public class HttpLoggingOptionsTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        // Act
        var options = new HttpLoggingOptions();

        // Assert
        Assert.True(options.LogRequests);
        Assert.True(options.LogResponses);
        Assert.True(options.LogRequestHeaders);
        Assert.True(options.LogResponseHeaders);
        Assert.False(options.LogRequestContent);
        Assert.False(options.LogResponseContent);
        Assert.Equal(1024, options.MaxContentLogSize);
        Assert.Equal(LogLevel.Information, options.SuccessLogLevel);
        Assert.Equal(LogLevel.Warning, options.ClientErrorLogLevel);
        Assert.Equal(LogLevel.Error, options.ServerErrorLogLevel);
        Assert.Equal(LogLevel.Error, options.ExceptionLogLevel);
        Assert.Equal("[REDACTED]", options.RedactedValue);
    }

    [Fact]
    public void RedactedHeaders_ContainsExpectedDefaults()
    {
        // Act
        var options = new HttpLoggingOptions();

        // Assert
        Assert.Contains("Authorization", options.RedactedHeaders);
        Assert.Contains("X-API-Key", options.RedactedHeaders);
        Assert.Contains("X-Auth-Token", options.RedactedHeaders);
        Assert.Contains("Cookie", options.RedactedHeaders);
        Assert.Contains("Set-Cookie", options.RedactedHeaders);
        Assert.Contains("Proxy-Authorization", options.RedactedHeaders);
        Assert.Contains("WWW-Authenticate", options.RedactedHeaders);
    }

    [Fact]
    public void RedactedHeaders_IsCaseInsensitive()
    {
        // Arrange
        var options = new HttpLoggingOptions();

        // Act & Assert
        Assert.Contains("authorization", options.RedactedHeaders); // lowercase
        Assert.Contains("AUTHORIZATION", options.RedactedHeaders); // uppercase
        Assert.Contains("Authorization", options.RedactedHeaders); // mixed case
    }

    [Fact]
    public void Properties_CanBeModified()
    {
        // Arrange
        var options = new HttpLoggingOptions();

        // Act
        options.LogRequests = false;
        options.LogResponses = false;
        options.LogRequestHeaders = false;
        options.LogResponseHeaders = false;
        options.LogRequestContent = true;
        options.LogResponseContent = true;
        options.MaxContentLogSize = 2048;
        options.SuccessLogLevel = LogLevel.Debug;
        options.ClientErrorLogLevel = LogLevel.Error;
        options.ServerErrorLogLevel = LogLevel.Critical;
        options.ExceptionLogLevel = LogLevel.Critical;
        options.RedactedValue = "[HIDDEN]";

        // Assert
        Assert.False(options.LogRequests);
        Assert.False(options.LogResponses);
        Assert.False(options.LogRequestHeaders);
        Assert.False(options.LogResponseHeaders);
        Assert.True(options.LogRequestContent);
        Assert.True(options.LogResponseContent);
        Assert.Equal(2048, options.MaxContentLogSize);
        Assert.Equal(LogLevel.Debug, options.SuccessLogLevel);
        Assert.Equal(LogLevel.Error, options.ClientErrorLogLevel);
        Assert.Equal(LogLevel.Critical, options.ServerErrorLogLevel);
        Assert.Equal(LogLevel.Critical, options.ExceptionLogLevel);
        Assert.Equal("[HIDDEN]", options.RedactedValue);
    }

    [Fact]
    public void RedactedHeaders_CanBeModified()
    {
        // Arrange
        var options = new HttpLoggingOptions();

        // Act
        options.RedactedHeaders.Add("Custom-Secret-Header");
        options.RedactedHeaders.Remove("Authorization");

        // Assert
        Assert.Contains("Custom-Secret-Header", options.RedactedHeaders);
        Assert.DoesNotContain("Authorization", options.RedactedHeaders);
    }
}