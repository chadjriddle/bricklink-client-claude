using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a superset entry containing a list of items that contain the base part in a specific color.
/// </summary>
public class SupersetEntry
{
    /// <summary>
    /// The color of the base part for which supersets were requested.
    /// </summary>
    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    /// <summary>
    /// A list of items that contain the base part.
    /// </summary>
    [JsonPropertyName("entries")]
    public List<SupersetItem> Entries { get; set; } = new();
}
