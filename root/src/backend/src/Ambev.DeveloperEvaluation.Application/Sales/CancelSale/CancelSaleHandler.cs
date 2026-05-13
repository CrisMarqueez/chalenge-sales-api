using Ambev.DeveloperEvaluation.Application.Integration;
using Ambev.DeveloperEvaluation.Application.Integration.Events;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, SaleDetailResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IIntegrationEventPublisher _integrationEvents;
    private readonly ICurrentPrincipal _currentPrincipal;

    public CancelSaleHandler(
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

    public async Task<SaleDetailResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existing = await _saleRepository.GetByIdAsync(request.SaleId, tracking: false, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        SaleAccess.EnsureCanAccess(_currentPrincipal, existing);

        var sale = await _saleRepository.SetSaleCancelledAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        await _integrationEvents.PublishAsync(new SaleCancelledIntegrationEvent(sale.Id), cancellationToken);

        return _mapper.Map<SaleDetailResult>(sale);
    }
}
