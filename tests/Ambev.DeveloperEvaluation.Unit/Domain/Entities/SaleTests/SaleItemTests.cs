using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleTests;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover business rules, discount calculations, and validation scenarios.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that SaleItem can be created with valid data.
    /// </summary>
    [Fact(DisplayName = "Should create sale item with valid data")]
    public void Given_ValidData_When_CreateSaleItem_Then_ItemShouldBeCreated()
    {
        // Arrange
        var productId = "PROD-001";
        var productName = "Test Product";
        var quantity = 5;
        var unitPrice = 10.00m;
        var saleId = Guid.NewGuid();

        // Act
        var item = SaleItem.Create(productId, productName, quantity, unitPrice, saleId);

        // Assert
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal(productId, item.ProductId);
        Assert.Equal(productName, item.ProductName);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.Equal(saleId, item.SaleId);
        Assert.False(item.IsCancelled);
        Assert.Equal(10.00m, item.DiscountPercentage); // 5 items = 10% discount
        Assert.Equal(5.00m, item.DiscountAmount); // 50.00 * 0.10
        Assert.Equal(45.00m, item.TotalAmount); // 50.00 - 5.00
    }

    /// <summary>
    /// Tests discount calculation for quantities between 4-9 items (10% discount).
    /// </summary>
    [Theory(DisplayName = "Should apply 10% discount for 4-9 items")]
    [InlineData(4, 10.00, 10.00, 4.00, 36.00)]
    [InlineData(5, 20.00, 10.00, 10.00, 90.00)]
    [InlineData(9, 15.00, 10.00, 13.50, 121.50)]
    public void Given_QuantityBetween4And9_When_CreateItem_Then_ShouldApply10PercentDiscount(
        int quantity, decimal unitPrice, decimal expectedDiscountPercentage,
        decimal expectedDiscountAmount, decimal expectedTotal)
    {
        // Arrange & Act
        var item = SaleItem.Create("PROD-001", "Test Product", quantity, unitPrice, Guid.NewGuid());

        // Assert
        Assert.Equal(expectedDiscountPercentage, item.DiscountPercentage);
        Assert.Equal(expectedDiscountAmount, item.DiscountAmount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    /// <summary>
    /// Tests discount calculation for quantities between 10-20 items (20% discount).
    /// </summary>
    [Theory(DisplayName = "Should apply 20% discount for 10-20 items")]
    [InlineData(10, 10.00, 20.00, 20.00, 80.00)]
    [InlineData(15, 20.00, 20.00, 60.00, 240.00)]
    [InlineData(20, 25.00, 20.00, 100.00, 400.00)]
    public void Given_QuantityBetween10And20_When_CreateItem_Then_ShouldApply20PercentDiscount(
        int quantity, decimal unitPrice, decimal expectedDiscountPercentage,
        decimal expectedDiscountAmount, decimal expectedTotal)
    {
        // Arrange & Act
        var item = SaleItem.Create("PROD-001", "Test Product", quantity, unitPrice, Guid.NewGuid());

        // Assert
        Assert.Equal(expectedDiscountPercentage, item.DiscountPercentage);
        Assert.Equal(expectedDiscountAmount, item.DiscountAmount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    /// <summary>
    /// Tests that no discount is applied for quantities less than 4.
    /// </summary>
    [Theory(DisplayName = "Should not apply discount for less than 4 items")]
    [InlineData(1, 10.00, 0.00, 0.00, 10.00)]
    [InlineData(2, 15.00, 0.00, 0.00, 30.00)]
    [InlineData(3, 20.00, 0.00, 0.00, 60.00)]
    public void Given_QuantityLessThan4_When_CreateItem_Then_ShouldNotApplyDiscount(
        int quantity, decimal unitPrice, decimal expectedDiscountPercentage,
        decimal expectedDiscountAmount, decimal expectedTotal)
    {
        // Arrange & Act
        var item = SaleItem.Create("PROD-001", "Test Product", quantity, unitPrice, Guid.NewGuid());

        // Assert
        Assert.Equal(expectedDiscountPercentage, item.DiscountPercentage);
        Assert.Equal(expectedDiscountAmount, item.DiscountAmount);
        Assert.Equal(expectedTotal, item.TotalAmount);
    }

    /// <summary>
    /// Tests that creating item with quantity greater than 20 throws exception.
    /// </summary>
    [Fact(DisplayName = "Should throw exception for quantity greater than 20")]
    public void Given_QuantityGreaterThan20_When_CreateItem_Then_ShouldThrowException()
    {
        // Act & Assert
        var exception = Assert.Throws<SaleItemValidationException>(() =>
            SaleItem.Create("PROD-001", "Test Product", 21, 10.00m, Guid.NewGuid()));
        
        Assert.Equal("It's not possible to sell above 20 identical items.", exception.Message);
    }

    /// <summary>
    /// Tests that creating item with zero or negative quantity throws exception.
    /// </summary>
    [Theory(DisplayName = "Should throw exception for invalid quantity")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Given_InvalidQuantity_When_CreateItem_Then_ShouldThrowException(int quantity)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            SaleItem.Create("PROD-001", "Test Product", quantity, 10.00m, Guid.NewGuid()));
        
        Assert.Equal("Quantity must be greater than zero. (Parameter 'quantity')", exception.Message);
    }

    /// <summary>
    /// Tests that creating item with zero or negative unit price throws exception.
    /// </summary>
    [Theory(DisplayName = "Should throw exception for invalid unit price")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Given_InvalidUnitPrice_When_CreateItem_Then_ShouldThrowException(decimal unitPrice)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            SaleItem.Create("PROD-001", "Test Product", 5, unitPrice, Guid.NewGuid()));
        
        Assert.Equal("Unit price must be greater than zero. (Parameter 'unitPrice')", exception.Message);
    }

    /// <summary>
    /// Tests that quantity can be updated and totals recalculated.
    /// </summary>
    [Fact(DisplayName = "Should update quantity and recalculate totals")]
    public void Given_ExistingItem_When_UpdateQuantity_Then_TotalsShouldBeRecalculated()
    {
        // Arrange
        var item = SaleItem.Create("PROD-001", "Test Product", 3, 10.00m, Guid.NewGuid());
        Assert.Equal(0, item.DiscountPercentage); // Initially no discount

        // Act
        item.UpdateQuantity(5);

        // Assert
        Assert.Equal(5, item.Quantity);
        Assert.Equal(10.00m, item.DiscountPercentage); // Now has 10% discount
        Assert.Equal(5.00m, item.DiscountAmount);
        Assert.Equal(45.00m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelled items cannot be updated.
    /// </summary>
    [Fact(DisplayName = "Should not allow updating cancelled items")]
    public void Given_CancelledItem_When_UpdateQuantity_Then_ShouldThrowException()
    {
        // Arrange
        var item = SaleItem.Create("PROD-001", "Test Product", 5, 10.00m, Guid.NewGuid());
        item.Cancel();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => item.UpdateQuantity(10));
        Assert.Equal("Cannot update quantity of a cancelled item.", exception.Message);
    }

    /// <summary>
    /// Tests that item can be cancelled successfully.
    /// </summary>
    [Fact(DisplayName = "Should cancel item successfully")]
    public void Given_ActiveItem_When_Cancel_Then_ItemShouldBeCancelled()
    {
        // Arrange
        var item = SaleItem.Create("PROD-001", "Test Product", 5, 10.00m, Guid.NewGuid());

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
    }
}
