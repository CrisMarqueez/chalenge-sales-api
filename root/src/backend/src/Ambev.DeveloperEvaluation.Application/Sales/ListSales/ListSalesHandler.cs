using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentPrincipal _currentPrincipal;

    public ListSalesHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ICurrentPrincipal currentPrincipal)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _currentPrincipal = currentPrincipal;
    }

    public async Task<ListSalesResult> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListSalesCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        Guid? ownerFilter = null;
        if (!_currentPrincipal.CanViewAllSales)
            ownerFilter = _currentPrincipal.UserId ?? Guid.Empty;

        var (items, total) = await _saleRepository.ListPagedAsync(
            request.Page,
            request.PageSize,
            ownerFilter,
            cancellationToken);

        return new ListSalesResult
        {
            Items = _mapper.Map<IReadOnlyList<SaleSummaryResult>>(items),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
