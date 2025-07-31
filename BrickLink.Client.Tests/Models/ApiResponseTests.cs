using System.Text.Json;
using System.Text.Json.Serialization;
using BrickLink.Client.Models;

namespace BrickLink.Client.Tests.Models;

/// <summary>
/// Unit tests for the ApiResponse{T} class.
/// </summary>
public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var meta = new Meta { Code = 200, Message = "OK" };
        var data = "test data";

        // Act
        var response = new ApiResponse<string>
        {
            Meta = meta,
            Data = data
        };

        // Assert
        Assert.Equal(meta, response.Meta);
        Assert.Equal(data, response.Data);
    }

    [Fact]
    public void ApiResponse_CanDeserializeFromJson()
    {
        // Arrange
        var json = """
        {
            "meta": {
                "code": 200,
                "message": "OK",
                "description": "Success"
            },
            "data": {
                "value": "test"
            }
        }
        """;

        // Act
        var response = JsonSerializer.Deserialize<ApiResponse<TestData>>(json);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Meta);
        Assert.Equal(200, response.Meta.Code);
        Assert.Equal("OK", response.Meta.Message);
        Assert.Equal("Success", response.Meta.Description);
        Assert.NotNull(response.Data);
        Assert.Equal("test", response.Data.Value);
    }

    [Fact]
    public void ApiResponse_CanSerializeToJson()
    {
        // Arrange
        var response = new ApiResponse<TestData>
        {
            Meta = new Meta { Code = 201, Message = "Created" },
            Data = new TestData { Value = "test" }
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.Contains("\"meta\":", json);
        Assert.Contains("\"data\":", json);
        Assert.Contains("\"code\":201", json);
        Assert.Contains("\"message\":\"Created\"", json);
        Assert.Contains("\"value\":\"test\"", json);
    }

    [Fact]
    public void ApiResponse_WithNullData_SerializesCorrectly()
    {
        // Arrange
        var response = new ApiResponse<string>
        {
            Meta = new Meta { Code = 404, Message = "Not Found" },
            Data = null
        };

        // Act
        var json = JsonSerializer.Serialize(response);

        // Assert
        Assert.Contains("\"data\":null", json);
    }

    private class TestData
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;
    }
}