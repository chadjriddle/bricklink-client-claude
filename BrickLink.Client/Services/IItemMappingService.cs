using BrickLink.Client.Models;

namespace BrickLink.Client.Services;

/// <summary>
/// Defines the contract for item mapping operations between BrickLink and LEGO Element IDs.
/// </summary>
public interface IItemMappingService : IApiService
{
    /// <summary>
    /// Retrieves the LEGO Element ID(s) for a given BrickLink part and color.
    /// </summary>
    /// <param name="partNo">The BrickLink part number.</param>
    /// <param name="colorId">Optional color ID of the part. If omitted, returns mappings for all colors.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of item mappings.</returns>
    Task<IReadOnlyList<ItemMapping>> GetElementIdAsync(string partNo, int? colorId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the BrickLink item number and color ID for a given LEGO Element ID.
    /// </summary>
    /// <param name="elementId">The LEGO Element ID to look up.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of item mappings.</returns>
    Task<IReadOnlyList<ItemMapping>> GetItemNumberAsync(string elementId, CancellationToken cancellationToken = default);
}