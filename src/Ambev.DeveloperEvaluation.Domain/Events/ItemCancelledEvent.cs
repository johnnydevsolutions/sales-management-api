namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when an item in a sale is cancelled.
/// </summary>
public class ItemCancelledEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the ID of the sale containing the cancelled item.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the ID of the cancelled item.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the product ID of the cancelled item.
    /// </summary>
    public string ProductId { get; }

    /// <summary>
    /// Initializes a new instance of the ItemCancelledEvent class.
    /// </summary>
    /// <param name="saleId">The ID of the sale containing the cancelled item.</param>
    /// <param name="itemId">The ID of the cancelled item.</param>
    /// <param name="productId">The product ID of the cancelled item.</param>
    public ItemCancelledEvent(Guid saleId, Guid itemId, string productId)
    {
        SaleId = saleId;
        ItemId = itemId;
        ProductId = productId;
    }
}
