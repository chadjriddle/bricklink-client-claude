using System.Net;
using BrickLink.Client.Models;

namespace BrickLink.Client.Exceptions;

/// <summary>
/// Represents errors that occur during BrickLink API operations.
/// This is the base exception class for all BrickLink API-related errors.
/// </summary>
public class BrickLinkApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code returned by the BrickLink API.
    /// </summary>
    public HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// Gets the error code returned by the BrickLink API.
    /// This corresponds to the "code" field in the API response metadata.
    /// </summary>
    public int Code { get; init; }

    /// <summary>
    /// Gets additional descriptive information about the error.
    /// This corresponds to the "description" field in the API response metadata.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkApiException"/> class.
    /// </summary>
    public BrickLinkApiException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkApiException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BrickLinkApiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkApiException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BrickLinkApiException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkApiException"/> class
    /// with detailed API error information.
    /// </summary>
    /// <param name="message">The error message from the API response.</param>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="code">The error code from the API response metadata.</param>
    /// <param name="description">Additional descriptive information about the error.</param>
    public BrickLinkApiException(string message, HttpStatusCode statusCode, int code, string? description = null)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrickLinkApiException"/> class
    /// with detailed API error information and an inner exception.
    /// </summary>
    /// <param name="message">The error message from the API response.</param>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="code">The error code from the API response metadata.</param>
    /// <param name="description">Additional descriptive information about the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BrickLinkApiException(string message, HttpStatusCode statusCode, int code, string? description, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Code = code;
        Description = description;
    }

    /// <summary>
    /// Creates a <see cref="BrickLinkApiException"/> from an API response with error metadata.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="meta">The metadata from the API response containing error information.</param>
    /// <returns>A new instance of <see cref="BrickLinkApiException"/> with the provided error details.</returns>
    public static BrickLinkApiException FromApiResponse(HttpStatusCode statusCode, Meta meta)
    {
        return new BrickLinkApiException(meta.Message, statusCode, meta.Code, meta.Description);
    }

    /// <summary>
    /// Creates a <see cref="BrickLinkApiException"/> for authentication-related errors.
    /// </summary>
    /// <param name="message">The error message describing the authentication failure.</param>
    /// <param name="innerException">The underlying exception that caused the authentication failure, if any.</param>
    /// <returns>A new instance of <see cref="BrickLinkApiException"/> configured for authentication errors.</returns>
    public static BrickLinkApiException CreateAuthenticationError(string message, Exception? innerException = null)
    {
        return innerException != null
            ? new BrickLinkApiException(message, HttpStatusCode.Unauthorized, 401, "Authentication failed. Please verify your API credentials and signature.", innerException)
            : new BrickLinkApiException(message, HttpStatusCode.Unauthorized, 401, "Authentication failed. Please verify your API credentials and signature.");
    }

    /// <summary>
    /// Creates a <see cref="BrickLinkApiException"/> for resource not found errors.
    /// </summary>
    /// <param name="resourceType">The type of resource that was not found (e.g., "item", "color", "category").</param>
    /// <param name="resourceId">The identifier of the resource that was not found.</param>
    /// <returns>A new instance of <see cref="BrickLinkApiException"/> configured for not found errors.</returns>
    public static BrickLinkApiException CreateNotFoundError(string resourceType, string resourceId)
    {
        return new BrickLinkApiException($"The requested {resourceType} '{resourceId}' was not found.", HttpStatusCode.NotFound, 404,
            $"The {resourceType} with identifier '{resourceId}' does not exist in the BrickLink catalog.");
    }

    /// <summary>
    /// Creates a <see cref="BrickLinkApiException"/> for rate limiting errors.
    /// </summary>
    /// <param name="retryAfter">The number of seconds to wait before retrying the request, if provided by the API.</param>
    /// <returns>A new instance of <see cref="BrickLinkApiException"/> configured for rate limiting errors.</returns>
    public static BrickLinkApiException CreateRateLimitError(int? retryAfter = null)
    {
        var message = "API rate limit exceeded. Please wait before making additional requests.";
        var description = retryAfter.HasValue
            ? $"Rate limit exceeded. Retry after {retryAfter.Value} seconds."
            : "Rate limit exceeded. Please implement exponential backoff and retry logic.";

        return new BrickLinkApiException(message, HttpStatusCode.TooManyRequests, 429, description);
    }

    /// <summary>
    /// Creates a <see cref="BrickLinkApiException"/> for validation errors with invalid request parameters.
    /// </summary>
    /// <param name="parameterName">The name of the invalid parameter.</param>
    /// <param name="parameterValue">The invalid parameter value.</param>
    /// <param name="validationMessage">Additional details about why the parameter is invalid.</param>
    /// <returns>A new instance of <see cref="BrickLinkApiException"/> configured for validation errors.</returns>
    public static BrickLinkApiException CreateValidationError(string parameterName, string parameterValue, string validationMessage)
    {
        return new BrickLinkApiException($"Invalid parameter '{parameterName}': {validationMessage}", HttpStatusCode.BadRequest, 400,
            $"The parameter '{parameterName}' with value '{parameterValue}' is invalid. {validationMessage}");
    }

    /// <summary>
    /// Creates a <see cref="BrickLinkApiException"/> for server errors.
    /// </summary>
    /// <param name="message">The error message describing the server error.</param>
    /// <param name="innerException">The underlying exception that caused the server error, if any.</param>
    /// <returns>A new instance of <see cref="BrickLinkApiException"/> configured for server errors.</returns>
    public static BrickLinkApiException CreateServerError(string message, Exception? innerException = null)
    {
        return innerException != null
            ? new BrickLinkApiException(message, HttpStatusCode.InternalServerError, 500, "An internal server error occurred. Please try again later.", innerException)
            : new BrickLinkApiException(message, HttpStatusCode.InternalServerError, 500, "An internal server error occurred. Please try again later.");
    }
}
