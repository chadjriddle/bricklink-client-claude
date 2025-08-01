using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents an item that contains a base part (superset item).
/// </summary>
public class SupersetItem
{
    /// <summary>
    /// The superset item information.
    /// </summary>
    [JsonPropertyName("item")]
    public InventoryItem Item { get; set; } = new();

    /// <summary>
    /// How many of the base part are included in this superset.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// How the part appears in the superset.
    /// </summary>
    [JsonPropertyName("appear_as")]
    public AppearAs AppearAs { get; set; }
}
