using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents the metadata returned with every BrickLink API response.
/// Contains information about the status and details of the API call.
/// </summary>
public class Meta
{
    /// <summary>
    /// Gets or sets the HTTP status code of the response.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Gets or sets the status message describing the result of the API call.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional descriptive information about the response.
    /// This field may contain more detailed error information when the API call fails.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
