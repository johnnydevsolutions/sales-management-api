using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, bool>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="logger">The logger</param>
    public CancelSaleHandler(ISaleRepository saleRepository, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request
    /// </summary>
    /// <param name="command">The CancelSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was cancelled, false if not found</returns>
    public async Task<bool> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            return false;

        // Cancel the sale
        sale.Cancel();

        // Save changes
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        // Log domain events
        foreach (var domainEvent in sale.DomainEvents)
        {
            _logger.LogInformation("Domain event occurred: {EventType} - {EventData}",
                domainEvent.GetType().Name,
                System.Text.Json.JsonSerializer.Serialize(domainEvent));
        }

        // Clear events after logging
        sale.ClearDomainEvents();

        return true;
    }
}
