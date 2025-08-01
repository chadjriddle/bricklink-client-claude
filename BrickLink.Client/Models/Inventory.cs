using System.Text.Json.Serialization;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents an inventory lot listed for sale in a user's store.
/// </summary>
public class Inventory
{
    /// <summary>
    /// The unique identifier for this inventory lot.
    /// </summary>
    [JsonPropertyName("inventory_id")]
    public long InventoryId { get; set; }

    /// <summary>
    /// An object describing the catalog item.
    /// </summary>
    [JsonPropertyName("item")]
    public InventoryItem Item { get; set; } = new();

    /// <summary>
    /// The color ID of the item.
    /// </summary>
    [JsonPropertyName("color_id")]
    public int ColorId { get; set; }

    /// <summary>
    /// The number of items available in this lot.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// The condition of the items ('N' for New, 'U' for Used).
    /// </summary>
    [JsonPropertyName("new_or_used")]
    public NewOrUsed NewOrUsed { get; set; }

    /// <summary>
    /// The price per item in the lot.
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Indicates how the lot will be sold ('N' for Normal, 'F' for Fixed).
    /// </summary>
    [JsonPropertyName("bind_id")]
    public int BindId { get; set; }

    /// <summary>
    /// A public description of the lot displayed to buyers.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Private remarks for the seller's reference.
    /// </summary>
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }

    /// <summary>
    /// The multiple in which this item must be purchased (e.g., a bulk value of 100 means buyers must purchase 100, 200, etc.).
    /// </summary>
    [JsonPropertyName("bulk")]
    public int Bulk { get; set; }

    /// <summary>
    /// If true, the lot remains in inventory (with quantity 0) after being sold out.
    /// </summary>
    [JsonPropertyName("is_retain")]
    public bool IsRetain { get; set; }

    /// <summary>
    /// If true, the lot is in a private stockroom and not visible to buyers.
    /// </summary>
    [JsonPropertyName("is_stock_room")]
    public bool IsStockRoom { get; set; }

    /// <summary>
    /// The ID of the stockroom ('A', 'B', or 'C') if IsStockRoom is true.
    /// </summary>
    [JsonPropertyName("stock_room_id")]
    public string? StockRoomId { get; set; }

    /// <summary>
    /// The timestamp when this lot was created.
    /// </summary>
    [JsonPropertyName("date_created")]
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// The tier price for this inventory lot (if applicable).
    /// </summary>
    [JsonPropertyName("tier_quantity1")]
    public int? TierQuantity1 { get; set; }

    /// <summary>
    /// The first tier price for this inventory lot (if applicable).
    /// </summary>
    [JsonPropertyName("tier_price1")]
    public decimal? TierPrice1 { get; set; }

    /// <summary>
    /// The second tier quantity for this inventory lot (if applicable).
    /// </summary>
    [JsonPropertyName("tier_quantity2")]
    public int? TierQuantity2 { get; set; }

    /// <summary>
    /// The second tier price for this inventory lot (if applicable).
    /// </summary>
    [JsonPropertyName("tier_price2")]
    public decimal? TierPrice2 { get; set; }

    /// <summary>
    /// The third tier quantity for this inventory lot (if applicable).
    /// </summary>
    [JsonPropertyName("tier_quantity3")]
    public int? TierQuantity3 { get; set; }

    /// <summary>
    /// The third tier price for this inventory lot (if applicable).
    /// </summary>
    [JsonPropertyName("tier_price3")]
    public decimal? TierPrice3 { get; set; }

    /// <summary>
    /// My cost per item in this lot.
    /// </summary>
    [JsonPropertyName("my_cost")]
    public decimal? MyCost { get; set; }

    /// <summary>
    /// The sale percentage for this lot (used in sales/discounts).
    /// </summary>
    [JsonPropertyName("sale_rate")]
    public int? SaleRate { get; set; }

    /// <summary>
    /// The weight of a single item in grams.
    /// </summary>
    [JsonPropertyName("my_weight")]
    public decimal? MyWeight { get; set; }
}
