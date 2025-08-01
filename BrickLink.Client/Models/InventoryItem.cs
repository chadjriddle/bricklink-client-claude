using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents basic item information used in inventory and order contexts.
/// This is a simplified representation of catalog items containing only essential properties.
/// </summary>
public class InventoryItem
{
    /// <summary>
    /// The item's catalog identification number (e.g., "3001").
    /// </summary>
    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    /// <summary>
    /// The item's official name (e.g., "Brick 2 x 4").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The type of the item (e.g., PART, SET, MINIFIG).
    /// </summary>
    [JsonPropertyName("type")]
    public ItemType Type { get; set; }

    /// <summary>
    /// The ID of the item's main category.
    /// </summary>
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }
}
