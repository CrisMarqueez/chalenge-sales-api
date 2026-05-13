using Ambev.DeveloperEvaluation.Application.Integration;
using Ambev.DeveloperEvaluation.Application.Integration.Events;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, SaleDetailResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IIntegrationEventPublisher _integrationEvents;
    private readonly ICurrentPrincipal _currentPrincipal;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IIntegrationEventPublisher integrationEvents,
        ICurrentPrincipal currentPrincipal)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _integrationEvents = integrationEvents;
        _currentPrincipal = currentPrincipal;
    }

    public async Task<SaleDetailResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (!_currentPrincipal.UserId.HasValue)
            throw new UnauthorizedAccessException("Authenticated user id is required to create a sale.");

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleDate = command.SaleDate.ToUniversalTime(),
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            IsCancelled = false,
            OwnerUserId = _currentPrincipal.UserId.Value
        };

        foreach (var item in command.Items)
        {
            sale.Items.Add(new SaleLineItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                IsCancelled = item.IsCancelled
            });
        }

        sale.AssignSaleNumberFromId();
        sale.RecalculateTotals();

        var created = await _saleRepository.CreateAsync(sale, cancellationToken);
        await _integrationEvents.PublishAsync(new SaleCreatedIntegrationEvent(created.Id), cancellationToken);

        return _mapper.Map<SaleDetailResult>(created);
    }
}
