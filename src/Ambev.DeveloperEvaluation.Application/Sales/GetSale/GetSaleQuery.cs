using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Query for retrieving a specific sale by its ID
/// </summary>
public class GetSaleQuery : IRequest<GetSaleResult?>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to retrieve
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of GetSaleQuery
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    public GetSaleQuery(Guid id)
    {
        Id = id;
    }
}
