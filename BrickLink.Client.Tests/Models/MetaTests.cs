using System.Text.Json;
using BrickLink.Client.Models;

namespace BrickLink.Client.Tests.Models;

/// <summary>
/// Unit tests for the Meta class.
/// </summary>
public class MetaTests
{
    [Fact]
    public void Meta_Properties_CanBeSetAndRetrieved()
    {
        // Arrange & Act
        var meta = new Meta
        {
            Code = 200,
            Message = "OK",
            Description = "Request successful"
        };

        // Assert
        Assert.Equal(200, meta.Code);
        Assert.Equal("OK", meta.Message);
        Assert.Equal("Request successful", meta.Description);
    }

    [Fact]
    public void Meta_CanDeserializeFromJson()
    {
        // Arrange
        var json = """
        {
            "code": 404,
            "message": "RESOURCE_NOT_FOUND",
            "description": "The requested resource was not found"
        }
        """;

        // Act
        var meta = JsonSerializer.Deserialize<Meta>(json);

        // Assert
        Assert.NotNull(meta);
        Assert.Equal(404, meta.Code);
        Assert.Equal("RESOURCE_NOT_FOUND", meta.Message);
        Assert.Equal("The requested resource was not found", meta.Description);
    }

    [Fact]
    public void Meta_CanSerializeToJson()
    {
        // Arrange
        var meta = new Meta
        {
            Code = 400,
            Message = "BAD_REQUEST",
            Description = "Invalid parameters provided"
        };

        // Act
        var json = JsonSerializer.Serialize(meta);

        // Assert
        Assert.Contains("\"code\":400", json);
        Assert.Contains("\"message\":\"BAD_REQUEST\"", json);
        Assert.Contains("\"description\":\"Invalid parameters provided\"", json);
    }

    [Fact]
    public void Meta_WithNullDescription_SerializesCorrectly()
    {
        // Arrange
        var meta = new Meta
        {
            Code = 200,
            Message = "OK",
            Description = null
        };

        // Act
        var json = JsonSerializer.Serialize(meta);

        // Assert
        Assert.Contains("\"description\":null", json);
    }

    [Theory]
    [InlineData(200, "OK")]
    [InlineData(201, "CREATED")]
    [InlineData(400, "BAD_REQUEST")]
    [InlineData(401, "UNAUTHORIZED")]
    [InlineData(404, "NOT_FOUND")]
    [InlineData(500, "INTERNAL_SERVER_ERROR")]
    public void Meta_HandlesVariousStatusCodes(int code, string message)
    {
        // Arrange & Act
        var meta = new Meta { Code = code, Message = message };

        // Assert
        Assert.Equal(code, meta.Code);
        Assert.Equal(message, meta.Message);
    }
}