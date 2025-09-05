using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

/// <summary>
/// Handler for processing GetAllSalesQuery requests
/// </summary>
public class GetAllSalesHandler : IRequestHandler<GetAllSalesQuery, GetAllSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetAllSalesHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetAllSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAllSalesQuery request
    /// </summary>
    /// <param name="query">The GetAllSales query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The paginated sales result</returns>
    public async Task<GetAllSalesResult> Handle(GetAllSalesQuery query, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetAllAsync(query.Page, query.Size, cancellationToken);
        var totalCount = await _saleRepository.GetCountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)totalCount / query.Size);

        var salesData = _mapper.Map<List<GetSaleResult>>(sales);

        return new GetAllSalesResult
        {
            CurrentPage = query.Page,
            PageSize = query.Size,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Data = salesData
        };
    }
}
