using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a category from the BrickLink catalog hierarchy.
/// Categories are used to organize items in a hierarchical structure.
/// </summary>
public class Category
{
    /// <summary>
    /// The unique identifier for the category.
    /// </summary>
    [JsonPropertyName("category_id")]
    public int CategoryId { get; set; }

    /// <summary>
    /// The name of the category (e.g., "Bricks", "Minifigures").
    /// </summary>
    [JsonPropertyName("category_name")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the parent category. A value of 0 indicates a root category.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }
}