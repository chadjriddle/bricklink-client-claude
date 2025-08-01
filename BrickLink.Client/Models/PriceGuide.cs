using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents pricing statistics for a specified item.
/// </summary>
public class PriceGuide
{
    /// <summary>
    /// The item for which the price guide is being provided.
    /// </summary>
    [JsonPropertyName("item")]
    public InventoryItem Item { get; set; } = new();

    /// <summary>
    /// The condition of the items in this price guide.
    /// </summary>
    [JsonPropertyName("new_or_used")]
    public NewOrUsed NewOrUsed { get; set; }

    /// <summary>
    /// The ISO 4217 currency code for the prices.
    /// </summary>
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// The minimum price found.
    /// </summary>
    [JsonPropertyName("min_price")]
    public decimal MinPrice { get; set; }

    /// <summary>
    /// The maximum price found.
    /// </summary>
    [JsonPropertyName("max_price")]
    public decimal MaxPrice { get; set; }

    /// <summary>
    /// The average price of all listings.
    /// </summary>
    [JsonPropertyName("avg_price")]
    public decimal AvgPrice { get; set; }

    /// <summary>
    /// The quantity-weighted average price.
    /// </summary>
    [JsonPropertyName("qty_avg_price")]
    public decimal QtyAvgPrice { get; set; }

    /// <summary>
    /// The number of lots (for "stock") or number of times sold (for "sold").
    /// </summary>
    [JsonPropertyName("unit_quantity")]
    public int UnitQuantity { get; set; }

    /// <summary>
    /// The total number of items available (for "stock") or sold (for "sold").
    /// </summary>
    [JsonPropertyName("total_quantity")]
    public int TotalQuantity { get; set; }

    /// <summary>
    /// A list of individual price points.
    /// </summary>
    [JsonPropertyName("price_detail")]
    public List<PriceDetail> PriceDetail { get; set; } = new();
}
