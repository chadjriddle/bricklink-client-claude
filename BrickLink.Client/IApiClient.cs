using BrickLink.Client.Services;

namespace BrickLink.Client;

/// <summary>
/// Defines the main contract for the BrickLink API client.
/// </summary>
public interface IApiClient : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the catalog service for retrieving item information.
    /// </summary>
    ICatalogService Catalog { get; }

    /// <summary>
    /// Gets the color service for retrieving color information.
    /// </summary>
    IColorService Colors { get; }

    /// <summary>
    /// Gets the category service for retrieving category information.
    /// </summary>
    ICategoryService Categories { get; }

    /// <summary>
    /// Gets the item mapping service for converting between BrickLink and LEGO Element IDs.
    /// </summary>
    IItemMappingService ItemMapping { get; }
}