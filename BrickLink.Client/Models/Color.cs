using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents a color from the BrickLink catalog with detailed color information.
/// </summary>
public class Color
{
    /// <summary>
    /// The unique identifier for the color.
    /// </summary>
    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    /// <summary>
    /// The official name of the color (e.g., "Black", "Bright Green").
    /// </summary>
    [JsonPropertyName("color_name")]
    public string ColorName { get; set; } = string.Empty;

    /// <summary>
    /// The HTML hex code for the color (e.g., "05131D").
    /// </summary>
    [JsonPropertyName("color_code")]
    public string ColorCode { get; set; } = string.Empty;

    /// <summary>
    /// The group or type of the color (e.g., "Solid", "Transparent", "Chrome").
    /// </summary>
    [JsonPropertyName("color_type")]
    public string ColorType { get; set; } = string.Empty;
}