using Ambev.DeveloperEvaluation.Domain.Services;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class SaleLineItemDataValidator : AbstractValidator<SaleLineItemData>
{
    public SaleLineItemDataValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantity).InclusiveBetween(1, SaleDiscountPolicy.MaxQuantityPerProduct);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
    }
}
