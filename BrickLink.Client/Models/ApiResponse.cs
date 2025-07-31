using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents the standard response wrapper returned by the BrickLink API.
/// All API responses follow this structure with metadata and the actual data payload.
/// </summary>
/// <typeparam name="T">The type of data contained in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Gets or sets the response metadata containing information about the API call.
    /// </summary>
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = null!;

    /// <summary>
    /// Gets or sets the actual data payload returned by the API.
    /// This will be null if the API call was unsuccessful.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}