using System.Net;
using BrickLink.Client.Exceptions;
using BrickLink.Client.Models;

namespace BrickLink.Client.Tests.Exceptions;

/// <summary>
/// Unit tests for the BrickLinkApiException class.
/// </summary>
public class BrickLinkApiExceptionTests
{
    [Fact]
    public void Constructor_Default_CreatesExceptionWithDefaultValues()
    {
        // Act
        var exception = new BrickLinkApiException();

        // Assert
        Assert.NotNull(exception);
        Assert.Equal((HttpStatusCode)0, exception.StatusCode); // Default value
        Assert.Equal(0, exception.Code); // Default value
        Assert.Null(exception.Description);
        Assert.Contains("BrickLink.Client.Exceptions.BrickLinkApiException", exception.Message); // Default exception message
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange
        const string message = "Test error message";

        // Act
        var exception = new BrickLinkApiException(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal((HttpStatusCode)0, exception.StatusCode);
        Assert.Equal(0, exception.Code);
        Assert.Null(exception.Description);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBothProperties()
    {
        // Arrange
        const string message = "Test error message";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new BrickLinkApiException(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void Constructor_WithApiDetails_SetsAllProperties()
    {
        // Arrange
        const string message = "API error";
        const HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        const int code = 400;
        const string description = "Bad request description";

        // Act
        var exception = new BrickLinkApiException(message, statusCode, code, description);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(statusCode, exception.StatusCode);
        Assert.Equal(code, exception.Code);
        Assert.Equal(description, exception.Description);
    }

    [Fact]
    public void Constructor_WithAllParameters_SetsAllProperties()
    {
        // Arrange
        const string message = "API error";
        const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        const int code = 500;
        const string description = "Server error description";
        var innerException = new HttpRequestException("HTTP error");

        // Act
        var exception = new BrickLinkApiException(message, statusCode, code, description, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(statusCode, exception.StatusCode);
        Assert.Equal(code, exception.Code);
        Assert.Equal(description, exception.Description);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void FromApiResponse_WithMeta_CreatesExceptionWithMetaData()
    {
        // Arrange
        const HttpStatusCode statusCode = HttpStatusCode.NotFound;
        var meta = new Meta
        {
            Code = 404,
            Message = "Resource not found",
            Description = "The requested item does not exist"
        };

        // Act
        var exception = BrickLinkApiException.FromApiResponse(statusCode, meta);

        // Assert
        Assert.Equal(meta.Message, exception.Message);
        Assert.Equal(statusCode, exception.StatusCode);
        Assert.Equal(meta.Code, exception.Code);
        Assert.Equal(meta.Description, exception.Description);
    }

    [Fact]
    public void CreateAuthenticationError_WithMessage_CreatesAuthenticationException()
    {
        // Arrange
        const string message = "Invalid credentials";

        // Act
        var exception = BrickLinkApiException.CreateAuthenticationError(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal(401, exception.Code);
        Assert.Equal("Authentication failed. Please verify your API credentials and signature.", exception.Description);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void CreateAuthenticationError_WithInnerException_IncludesInnerException()
    {
        // Arrange
        const string message = "Invalid signature";
        var innerException = new ArgumentException("Invalid argument");

        // Act
        var exception = BrickLinkApiException.CreateAuthenticationError(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal(401, exception.Code);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Fact]
    public void CreateNotFoundError_WithResourceInfo_CreatesNotFoundException()
    {
        // Arrange
        const string resourceType = "item";
        const string resourceId = "3001";

        // Act
        var exception = BrickLinkApiException.CreateNotFoundError(resourceType, resourceId);

        // Assert
        Assert.Equal($"The requested {resourceType} '{resourceId}' was not found.", exception.Message);
        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal(404, exception.Code);
        Assert.Equal($"The {resourceType} with identifier '{resourceId}' does not exist in the BrickLink catalog.", exception.Description);
    }

    [Fact]
    public void CreateRateLimitError_WithoutRetryAfter_CreatesRateLimitException()
    {
        // Act
        var exception = BrickLinkApiException.CreateRateLimitError();

        // Assert
        Assert.Equal("API rate limit exceeded. Please wait before making additional requests.", exception.Message);
        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal(429, exception.Code);
        Assert.Equal("Rate limit exceeded. Please implement exponential backoff and retry logic.", exception.Description);
    }

    [Fact]
    public void CreateRateLimitError_WithRetryAfter_IncludesRetryTime()
    {
        // Arrange
        const int retryAfter = 60;

        // Act
        var exception = BrickLinkApiException.CreateRateLimitError(retryAfter);

        // Assert
        Assert.Equal("API rate limit exceeded. Please wait before making additional requests.", exception.Message);
        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal(429, exception.Code);
        Assert.Equal($"Rate limit exceeded. Retry after {retryAfter} seconds.", exception.Description);
    }

    [Fact]
    public void CreateValidationError_WithParameterDetails_CreatesValidationException()
    {
        // Arrange
        const string parameterName = "itemType";
        const string parameterValue = "INVALID";
        const string validationMessage = "Must be one of: PART, SET, MINIFIG";

        // Act
        var exception = BrickLinkApiException.CreateValidationError(parameterName, parameterValue, validationMessage);

        // Assert
        Assert.Equal($"Invalid parameter '{parameterName}': {validationMessage}", exception.Message);
        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(400, exception.Code);
        Assert.Equal($"The parameter '{parameterName}' with value '{parameterValue}' is invalid. {validationMessage}", exception.Description);
    }

    [Fact]
    public void CreateServerError_WithMessage_CreatesServerException()
    {
        // Arrange
        const string message = "Database connection failed";

        // Act
        var exception = BrickLinkApiException.CreateServerError(message);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Equal(500, exception.Code);
        Assert.Equal("An internal server error occurred. Please try again later.", exception.Description);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void CreateServerError_WithInnerException_IncludesInnerException()
    {
        // Arrange
        const string message = "Service unavailable";
        var innerException = new TimeoutException("Request timeout");

        // Act
        var exception = BrickLinkApiException.CreateServerError(message, innerException);

        // Assert
        Assert.Equal(message, exception.Message);
        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Equal(500, exception.Code);
        Assert.Equal("An internal server error occurred. Please try again later.", exception.Description);
        Assert.Equal(innerException, exception.InnerException);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, 400)]
    [InlineData(HttpStatusCode.Unauthorized, 401)]
    [InlineData(HttpStatusCode.Forbidden, 403)]
    [InlineData(HttpStatusCode.NotFound, 404)]
    [InlineData(HttpStatusCode.TooManyRequests, 429)]
    [InlineData(HttpStatusCode.InternalServerError, 500)]
    public void FromApiResponse_WithVariousStatusCodes_MapsCorrectly(HttpStatusCode statusCode, int expectedCode)
    {
        // Arrange
        var meta = new Meta { Code = expectedCode, Message = "Test message", Description = "Test description" };

        // Act
        var exception = BrickLinkApiException.FromApiResponse(statusCode, meta);

        // Assert
        Assert.Equal(statusCode, exception.StatusCode);
        Assert.Equal(expectedCode, exception.Code);
    }
}
