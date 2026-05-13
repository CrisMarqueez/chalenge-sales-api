using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleCommand, SaleDetailResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentPrincipal _currentPrincipal;

    public GetSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ICurrentPrincipal currentPrincipal)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _currentPrincipal = currentPrincipal;
    }

    public async Task<SaleDetailResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, tracking: false, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        SaleAccess.EnsureCanAccess(_currentPrincipal, sale);

        return _mapper.Map<SaleDetailResult>(sale);
    }
}
