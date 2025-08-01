using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a mapping between BrickLink catalog numbers and LEGO Element IDs.
/// </summary>
public class ItemMapping
{
    /// <summary>
    /// The BrickLink item information.
    /// </summary>
    [JsonPropertyName("item")]
    public InventoryItem Item { get; set; } = new();

    /// <summary>
    /// The BrickLink color ID.
    /// </summary>
    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    /// <summary>
    /// The corresponding LEGO Element ID.
    /// </summary>
    [JsonPropertyName("element_id")]
    public string ElementId { get; set; } = string.Empty;
}
