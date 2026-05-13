using Ambev.DeveloperEvaluation.Application.Integration;
using Ambev.DeveloperEvaluation.Application.Integration.Events;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleDetailResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IIntegrationEventPublisher _integrationEvents;
    private readonly ICurrentPrincipal _currentPrincipal;

    public UpdateSaleHandler(
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

    public async Task<SaleDetailResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existing = await _saleRepository.GetByIdAsync(command.Id, tracking: false, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        SaleAccess.EnsureCanAccess(_currentPrincipal, existing);

        var sale = new Sale
        {
            Id = command.Id,
            SaleNumber = existing.SaleNumber,
            SaleDate = command.SaleDate.ToUniversalTime(),
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            IsCancelled = command.IsCancelled,
            CreatedAt = existing.CreatedAt,
            OwnerUserId = existing.OwnerUserId
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

        sale.RecalculateTotals();

        var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _integrationEvents.PublishAsync(new SaleModifiedIntegrationEvent(updated.Id), cancellationToken);

        return _mapper.Map<SaleDetailResult>(updated);
    }
}
