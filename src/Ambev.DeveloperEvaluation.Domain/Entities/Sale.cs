using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// Contains all information about a sale including customer, branch, items and total amounts.
/// </summary>
public class Sale : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer external identifier.
    /// Following External Identities pattern with denormalization.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name (denormalized).
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch external identifier where the sale was made.
    /// </summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch name (denormalized).
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total sale amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the collection of sale items.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Adds a new item to the sale.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(SaleItem item)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale.");

        Items.Add(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Removes an item from the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove.</param>
    public void RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            RecalculateTotal();
        }
    }

    /// <summary>
    /// Cancels the entire sale.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            return;

        IsCancelled = true;
        AddDomainEvent(new SaleCancelledEvent(Id, SaleNumber));
    }

    /// <summary>
    /// Cancels a specific item in the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to cancel.</param>
    public void CancelItem(Guid itemId)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot cancel items in a cancelled sale.");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null && !item.IsCancelled)
        {
            item.Cancel();
            RecalculateTotal();
            AddDomainEvent(new ItemCancelledEvent(Id, itemId, item.ProductId));
        }
    }

    /// <summary>
    /// Recalculates the total amount of the sale based on active items.
    /// </summary>
    private void RecalculateTotal()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }

    /// <summary>
    /// Creates a new sale with the provided details.
    /// </summary>
    public static Sale Create(string saleNumber, string customerId, string customerName, 
        string branchId, string branchName)
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = saleNumber,
            SaleDate = DateTime.UtcNow,
            CustomerId = customerId,
            CustomerName = customerName,
            BranchId = branchId,
            BranchName = branchName,
            UpdatedAt = DateTime.UtcNow
        };

        sale.AddDomainEvent(new SaleCreatedEvent(sale.Id, sale.SaleNumber, sale.CustomerId));
        return sale;
    }

    /// <summary>
    /// Updates the sale information.
    /// </summary>
    public void Update(string customerName, string branchName)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale.");

        CustomerName = customerName;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleModifiedEvent(Id, SaleNumber));
    }
}
