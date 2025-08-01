using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents an item that is included in a base set (subset item).
/// </summary>
public class SubsetItem
{
    /// <summary>
    /// The subset item information.
    /// </summary>
    [JsonPropertyName("item")]
    public InventoryItem Item { get; set; } = new();

    /// <summary>
    /// The color ID of the subset item.
    /// </summary>
    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    /// <summary>
    /// The quantity of this item included in the set.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// The quantity of this item included as an "extra" part.
    /// </summary>
    [JsonPropertyName("extra_quantity")]
    public int ExtraQuantity { get; set; }

    /// <summary>
    /// Indicates if this item is an alternative to another in the same match_no group.
    /// </summary>
    [JsonPropertyName("is_alternate")]
    public bool IsAlternate { get; set; }
}
