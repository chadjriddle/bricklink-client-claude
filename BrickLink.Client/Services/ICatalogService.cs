using BrickLink.Client.Models;
using BrickLink.Client.Enums;

namespace BrickLink.Client.Services;

/// <summary>
/// Defines the contract for catalog-related API operations.
/// </summary>
public interface ICatalogService : IApiService
{
    /// <summary>
    /// Retrieves detailed information about a specific catalog item.
    /// </summary>
    /// <param name="itemType">The type of the item (PART, SET, MINIFIG, etc.).</param>
    /// <param name="itemNo">The catalog number of the item.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the catalog item details.</returns>
    Task<CatalogItem> GetItemAsync(ItemType itemType, string itemNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the image URL for a specific item in a specific color.
    /// </summary>
    /// <param name="itemType">The type of the item.</param>
    /// <param name="itemNo">The catalog number of the item.</param>
    /// <param name="colorId">The color ID for the desired image.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the item with image URLs.</returns>
    Task<CatalogItem> GetItemImageAsync(ItemType itemType, string itemNo, int colorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all known colors for a specific item.
    /// </summary>
    /// <param name="itemType">The type of the item.</param>
    /// <param name="itemNo">The catalog number of the item.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of known colors.</returns>
    Task<IReadOnlyList<KnownColor>> GetItemColorsAsync(ItemType itemType, string itemNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves sets that contain the specified item.
    /// </summary>
    /// <param name="itemType">The type of the item.</param>
    /// <param name="itemNo">The catalog number of the item.</param>
    /// <param name="colorId">Optional color ID to filter results.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of supersets.</returns>
    Task<IReadOnlyList<SupersetEntry>> GetSupersetsAsync(ItemType itemType, string itemNo, int? colorId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves items contained within the specified set.
    /// </summary>
    /// <param name="itemType">The type of the item.</param>
    /// <param name="itemNo">The catalog number of the item.</param>
    /// <param name="includeBox">Whether to include the box in the subset list.</param>
    /// <param name="includeInstructions">Whether to include instructions in the subset list.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of subsets.</returns>
    Task<IReadOnlyList<SubsetEntry>> GetSubsetsAsync(ItemType itemType, string itemNo, bool? includeBox = null, bool? includeInstructions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves pricing statistics for a specified item.
    /// </summary>
    /// <param name="itemType">The type of the item.</param>
    /// <param name="itemNo">The catalog number of the item.</param>
    /// <param name="colorId">Optional color ID of the item.</param>
    /// <param name="guideType">The type of price guide (Stock or Sold).</param>
    /// <param name="condition">Optional condition filter (New or Used).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the price guide information.</returns>
    Task<PriceGuide> GetPriceGuideAsync(ItemType itemType, string itemNo, int? colorId = null, PriceGuideType guideType = PriceGuideType.Stock, NewOrUsed? condition = null, CancellationToken cancellationToken = default);
}