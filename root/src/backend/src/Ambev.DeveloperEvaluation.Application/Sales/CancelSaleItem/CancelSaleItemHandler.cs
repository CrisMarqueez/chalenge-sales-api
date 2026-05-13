using Ambev.DeveloperEvaluation.Application.Integration;
using Ambev.DeveloperEvaluation.Application.Integration.Events;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, SaleDetailResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IIntegrationEventPublisher _integrationEvents;
    private readonly ICurrentPrincipal _currentPrincipal;

    public CancelSaleItemHandler(
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

    public async Task<SaleDetailResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existing = await _saleRepository.GetByIdAsync(request.SaleId, tracking: false, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException(
                $"Sale with ID {request.SaleId} not found or line item {request.LineItemId} not found");

        SaleAccess.EnsureCanAccess(_currentPrincipal, existing);

        var sale = await _saleRepository.SetLineItemCancelledAsync(
            request.SaleId,
            request.LineItemId,
            cancellationToken);

        if (sale == null)
            throw new KeyNotFoundException(
                $"Sale with ID {request.SaleId} not found or line item {request.LineItemId} not found");

        await _integrationEvents.PublishAsync(
            new ItemCancelledIntegrationEvent(sale.Id, request.LineItemId),
            cancellationToken);

        var reloaded = await _saleRepository.GetByIdAsync(sale.Id, tracking: false, cancellationToken);
        return _mapper.Map<SaleDetailResult>(reloaded!);
    }
}
