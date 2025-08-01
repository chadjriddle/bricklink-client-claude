using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a catalog item from the BrickLink catalog, including parts, sets, minifigures, and other items.
/// </summary>
public class CatalogItem
{
    /// <summary>
    /// The item's catalog identification number.
    /// </summary>
    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    /// <summary>
    /// The item's official name.
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

    /// <summary>
    /// An alternate identification number for the item.
    /// </summary>
    [JsonPropertyName("alternate_no")]
    public string? AlternateNo { get; set; }

    /// <summary>
    /// A URL to the primary image of the item.
    /// </summary>
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    /// <summary>
    /// A URL to the thumbnail image of the item.
    /// </summary>
    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// The item's weight in grams.
    /// </summary>
    [JsonPropertyName("weight")]
    public decimal Weight { get; set; }

    /// <summary>
    /// The item's dimension on the x-axis.
    /// </summary>
    [JsonPropertyName("dim_x")]
    public decimal DimX { get; set; }

    /// <summary>
    /// The item's dimension on the y-axis.
    /// </summary>
    [JsonPropertyName("dim_y")]
    public decimal DimY { get; set; }

    /// <summary>
    /// The item's dimension on the z-axis.
    /// </summary>
    [JsonPropertyName("dim_z")]
    public decimal DimZ { get; set; }

    /// <summary>
    /// The year the item was first released.
    /// </summary>
    [JsonPropertyName("year_released")]
    public int YearReleased { get; set; }

    /// <summary>
    /// A short description of the item from the catalog.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the item is considered obsolete.
    /// </summary>
    [JsonPropertyName("is_obsolete")]
    public bool IsObsolete { get; set; }

    /// <summary>
    /// The language code for book or instruction items.
    /// </summary>
    [JsonPropertyName("language_code")]
    public string? LanguageCode { get; set; }
}