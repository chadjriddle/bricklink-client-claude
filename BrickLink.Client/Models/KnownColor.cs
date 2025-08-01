using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a known color for a specific item, indicating the colors in which an item is available.
/// Note: This model schema is inferred from API documentation and may need refinement based on actual API responses.
/// </summary>
public class KnownColor
{
    /// <summary>
    /// The color ID of the known color.
    /// </summary>
    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    /// <summary>
    /// The number of sets in which this item appears in this color.
    /// Note: This property is inferred and may not match actual API response structure.
    /// </summary>
    [JsonPropertyName("quantity_in_sets")]
    public int QuantityInSets { get; set; }
}
