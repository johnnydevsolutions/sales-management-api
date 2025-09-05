using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The logger</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Check if sale number already exists
        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        // Create the sale
        var sale = Sale.Create(
            command.SaleNumber,
            command.CustomerId,
            command.CustomerName,
            command.BranchId,
            command.BranchName
        );

        // Add items to the sale
        foreach (var itemCommand in command.Items)
        {
            var item = SaleItem.Create(
                itemCommand.ProductId,
                itemCommand.ProductName,
                itemCommand.Quantity,
                itemCommand.UnitPrice,
                sale.Id
            );

            sale.AddItem(item);
        }

        // Save the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Log domain events (since we're not publishing to a message broker)
        foreach (var domainEvent in createdSale.DomainEvents)
        {
            _logger.LogInformation("Domain event occurred: {EventType} - {EventData}",
                domainEvent.GetType().Name,
                System.Text.Json.JsonSerializer.Serialize(domainEvent));
        }

        // Clear events after logging
        createdSale.ClearDomainEvents();

        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
