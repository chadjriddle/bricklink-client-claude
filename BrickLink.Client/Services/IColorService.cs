using BrickLink.Client.Models;

namespace BrickLink.Client.Services;

/// <summary>
/// Defines the contract for color-related API operations.
/// </summary>
public interface IColorService : IApiService
{
    /// <summary>
    /// Retrieves a list of all colors defined in the BrickLink catalog.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of all colors.</returns>
    Task<IReadOnlyList<Color>> GetColorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves information about a specific color.
    /// </summary>
    /// <param name="colorId">The ID of the color to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the color information.</returns>
    Task<Color> GetColorAsync(int colorId, CancellationToken cancellationToken = default);
}
