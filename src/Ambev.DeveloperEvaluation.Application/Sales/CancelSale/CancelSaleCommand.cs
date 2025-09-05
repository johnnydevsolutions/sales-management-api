using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command for cancelling a sale
/// </summary>
public class CancelSaleCommand : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to cancel
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}
