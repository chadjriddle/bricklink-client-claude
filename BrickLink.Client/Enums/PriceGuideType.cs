namespace BrickLink.Client.Enums;

/// <summary>
/// Represents the type of price guide to retrieve.
/// </summary>
public enum PriceGuideType
{
    /// <summary>
    /// Current stock-based pricing (items currently for sale).
    /// </summary>
    Stock,

    /// <summary>
    /// Historical sold pricing (items sold in the last 6 months).
    /// </summary>
    Sold
}