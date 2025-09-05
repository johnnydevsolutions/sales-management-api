using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleTests;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover sale creation, item management, and business logic scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that a sale can be created with valid data.
    /// </summary>
    [Fact(DisplayName = "Should create sale with valid data")]
    public void Given_ValidData_When_CreateSale_Then_SaleShouldBeCreated()
    {
        // Arrange
        var saleNumber = "SAL-001";
        var customerId = "CUST-001";
        var customerName = "John Doe";
        var branchId = "BR-001";
        var branchName = "Main Branch";

        // Act
        var sale = Sale.Create(saleNumber, customerId, customerName, branchId, branchName);

        // Assert
        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.Equal(saleNumber, sale.SaleNumber);
        Assert.Equal(customerId, sale.CustomerId);
        Assert.Equal(customerName, sale.CustomerName);
        Assert.Equal(branchId, sale.BranchId);
        Assert.Equal(branchName, sale.BranchName);
        Assert.False(sale.IsCancelled);
        Assert.Equal(0, sale.TotalAmount);
        Assert.Empty(sale.Items);
        Assert.Single(sale.DomainEvents);
        Assert.IsType<SaleCreatedEvent>(sale.DomainEvents.First());
    }

    /// <summary>
    /// Tests that items can be added to a sale successfully.
    /// </summary>
    [Fact(DisplayName = "Should add item to sale successfully")]
    public void Given_ValidSale_When_AddItem_Then_ItemShouldBeAdded()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        var item = SaleItem.Create("PROD-001", "Product 1", 5, 10.00m, sale.Id);

        // Act
        sale.AddItem(item);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(45.00m, sale.TotalAmount); // 5 * 10.00 with 10% discount = 45.00
    }

    /// <summary>
    /// Tests that items cannot be added to a cancelled sale.
    /// </summary>
    [Fact(DisplayName = "Should not allow adding items to cancelled sale")]
    public void Given_CancelledSale_When_AddItem_Then_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        sale.Cancel();
        var item = SaleItem.Create("PROD-001", "Product 1", 5, 10.00m, sale.Id);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.AddItem(item));
        Assert.Equal("Cannot add items to a cancelled sale.", exception.Message);
    }

    /// <summary>
    /// Tests that a sale can be cancelled successfully.
    /// </summary>
    [Fact(DisplayName = "Should cancel sale successfully")]
    public void Given_ActiveSale_When_Cancel_Then_SaleShouldBeCancelled()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        sale.ClearDomainEvents(); // Clear creation event

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
        Assert.Single(sale.DomainEvents);
        Assert.IsType<SaleCancelledEvent>(sale.DomainEvents.First());
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale doesn't generate duplicate events.
    /// </summary>
    [Fact(DisplayName = "Should not generate duplicate events when cancelling already cancelled sale")]
    public void Given_CancelledSale_When_Cancel_Then_ShouldNotGenerateDuplicateEvents()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        sale.Cancel();
        var eventCountAfterFirstCancel = sale.DomainEvents.Count;

        // Act
        sale.Cancel();

        // Assert
        Assert.Equal(eventCountAfterFirstCancel, sale.DomainEvents.Count);
    }

    /// <summary>
    /// Tests that sale information can be updated successfully.
    /// </summary>
    [Fact(DisplayName = "Should update sale information successfully")]
    public void Given_ActiveSale_When_Update_Then_InformationShouldBeUpdated()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        var newCustomerName = "Jane Doe";
        var newBranchName = "Secondary Branch";
        sale.ClearDomainEvents(); // Clear creation event

        // Act
        sale.Update(newCustomerName, newBranchName);

        // Assert
        Assert.Equal(newCustomerName, sale.CustomerName);
        Assert.Equal(newBranchName, sale.BranchName);
        Assert.Single(sale.DomainEvents);
        Assert.IsType<SaleModifiedEvent>(sale.DomainEvents.First());
    }

    /// <summary>
    /// Tests that cancelled sales cannot be updated.
    /// </summary>
    [Fact(DisplayName = "Should not allow updating cancelled sale")]
    public void Given_CancelledSale_When_Update_Then_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        sale.Cancel();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Update("New Name", "New Branch"));
        Assert.Equal("Cannot update a cancelled sale.", exception.Message);
    }

    /// <summary>
    /// Tests that total amount is recalculated when items are added.
    /// </summary>
    [Fact(DisplayName = "Should recalculate total amount when items are added")]
    public void Given_SaleWithItems_When_AddNewItem_Then_TotalShouldBeRecalculated()
    {
        // Arrange
        var sale = Sale.Create("SAL-001", "CUST-001", "John Doe", "BR-001", "Main Branch");
        var item1 = SaleItem.Create("PROD-001", "Product 1", 5, 10.00m, sale.Id); // 45.00 after discount
        var item2 = SaleItem.Create("PROD-002", "Product 2", 3, 20.00m, sale.Id); // 60.00 no discount

        // Act
        sale.AddItem(item1);
        sale.AddItem(item2);

        // Assert
        Assert.Equal(105.00m, sale.TotalAmount); // 45.00 + 60.00
    }
}
