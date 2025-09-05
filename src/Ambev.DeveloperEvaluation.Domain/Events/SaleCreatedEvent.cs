namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a new sale is created.
/// </summary>
public class SaleCreatedEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the ID of the created sale.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Gets the sale number.
    /// </summary>
    public string SaleNumber { get; }

    /// <summary>
    /// Gets the customer ID.
    /// </summary>
    public string CustomerId { get; }

    /// <summary>
    /// Initializes a new instance of the SaleCreatedEvent class.
    /// </summary>
    /// <param name="saleId">The ID of the created sale.</param>
    /// <param name="saleNumber">The sale number.</param>
    /// <param name="customerId">The customer ID.</param>
    public SaleCreatedEvent(Guid saleId, string saleNumber, string customerId)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        CustomerId = customerId;
    }
}
