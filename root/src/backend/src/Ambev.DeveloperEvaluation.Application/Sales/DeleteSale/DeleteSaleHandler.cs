using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICurrentPrincipal _currentPrincipal;

    public DeleteSaleHandler(ISaleRepository saleRepository, ICurrentPrincipal currentPrincipal)
    {
        _saleRepository = saleRepository;
        _currentPrincipal = currentPrincipal;
    }

    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, tracking: false, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        SaleAccess.EnsureCanAccess(_currentPrincipal, sale);

        var success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        return new DeleteSaleResponse { Success = true };
    }
}
