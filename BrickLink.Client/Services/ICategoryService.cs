using BrickLink.Client.Models;

namespace BrickLink.Client.Services;

/// <summary>
/// Defines the contract for category-related API operations.
/// </summary>
public interface ICategoryService : IApiService
{
    /// <summary>
    /// Retrieves a list of all categories defined in the BrickLink catalog.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of all categories.</returns>
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves information about a specific category.
    /// </summary>
    /// <param name="categoryId">The ID of the category to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the category information.</returns>
    Task<Category> GetCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
}