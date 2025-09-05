using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its sale number
    /// </summary>
    /// <param name="saleNumber">The sale number to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales with pagination support
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="size">The page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales</returns>
    Task<IEnumerable<Sale>> GetAllAsync(int page = 1, int size = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves sales by customer ID
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales for the customer</returns>
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves sales by branch ID
    /// </summary>
    /// <param name="branchId">The branch ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales for the branch</returns>
    Task<IEnumerable<Sale>> GetByBranchIdAsync(string branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="sale">The sale with updated information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of sales
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total count of sales</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
