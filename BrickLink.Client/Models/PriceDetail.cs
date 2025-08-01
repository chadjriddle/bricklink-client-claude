using System.Text.Json.Serialization;

namespace BrickLink.Client.Models;

/// <summary>
/// Represents an individual price point in a price guide.
/// </summary>
public class PriceDetail
{
    /// <summary>
    /// The number of items in the lot.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// The price per unit for this lot.
    /// </summary>
    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// For "stock" guide, indicates if the seller ships to your country.
    /// </summary>
    [JsonPropertyName("shipping_available")]
    public bool? ShippingAvailable { get; set; }

    /// <summary>
    /// For "sold" guide, the seller's country code.
    /// </summary>
    [JsonPropertyName("seller_country_code")]
    public string? SellerCountryCode { get; set; }

    /// <summary>
    /// For "sold" guide, the buyer's country code.
    /// </summary>
    [JsonPropertyName("buyer_country_code")]
    public string? BuyerCountryCode { get; set; }

    /// <summary>
    /// For "sold" guide, the date the order was placed.
    /// </summary>
    [JsonPropertyName("date_ordered")]
    public DateTimeOffset? DateOrdered { get; set; }
}
