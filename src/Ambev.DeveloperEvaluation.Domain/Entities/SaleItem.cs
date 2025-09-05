using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale transaction.
/// Implements business rules for quantity-based discounting.
/// </summary>
public class SaleItem : AuditableEntity
{
    /// <summary>
    /// Gets or sets the product external identifier.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product name (denormalized).
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied (0-100).
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item (after discount).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether this item is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the sale ID this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Navigation property to the parent sale.
    /// </summary>
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// Creates a new sale item with automatic discount calculation.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="quantity">The quantity.</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <param name="saleId">The parent sale ID.</param>
    /// <returns>A new SaleItem instance.</returns>
    public static SaleItem Create(string productId, string productName, int quantity, decimal unitPrice, Guid saleId)
    {
        // Validate business rules
        ValidateQuantity(quantity);
        ValidateUnitPrice(unitPrice);

        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            SaleId = saleId,
            UpdatedAt = DateTime.UtcNow
        };

        item.CalculateDiscountAndTotal();
        return item;
    }

    /// <summary>
    /// Updates the item quantity and recalculates totals.
    /// </summary>
    /// <param name="newQuantity">The new quantity.</param>
    public void UpdateQuantity(int newQuantity)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update quantity of a cancelled item.");

        ValidateQuantity(newQuantity);
        Quantity = newQuantity;
        CalculateDiscountAndTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the unit price and recalculates totals.
    /// </summary>
    /// <param name="newUnitPrice">The new unit price.</param>
    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update price of a cancelled item.");

        ValidateUnitPrice(newUnitPrice);
        UnitPrice = newUnitPrice;
        CalculateDiscountAndTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels this item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates discount and total amount based on business rules.
    /// </summary>
    private void CalculateDiscountAndTotal()
    {
        var subtotal = Quantity * UnitPrice;
        
        // Apply business rules for discount
        DiscountPercentage = CalculateDiscountPercentage(Quantity);
        DiscountAmount = subtotal * (DiscountPercentage / 100);
        TotalAmount = subtotal - DiscountAmount;
    }

    /// <summary>
    /// Calculates the discount percentage based on quantity and business rules.
    /// </summary>
    /// <param name="quantity">The quantity of items.</param>
    /// <returns>The discount percentage to apply.</returns>
    private static decimal CalculateDiscountPercentage(int quantity)
    {
        return quantity switch
        {
            >= 10 and <= 20 => 20m, // 20% discount for 10-20 items
            >= 4 => 10m,             // 10% discount for 4-9 items
            _ => 0m                  // No discount for less than 4 items
        };
    }

    /// <summary>
    /// Validates that the quantity follows business rules.
    /// </summary>
    /// <param name="quantity">The quantity to validate.</param>
    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (quantity > 20)
            throw new SaleItemValidationException("It's not possible to sell above 20 identical items.");
    }

    /// <summary>
    /// Validates that the unit price is valid.
    /// </summary>
    /// <param name="unitPrice">The unit price to validate.</param>
    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));
    }
}
