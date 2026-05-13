using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleCommandValidatorTests
{
    [Fact]
    public void Validate_EmptyItems_Fails()
    {
        var validator = new CreateSaleCommandValidator();
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            CustomerName = "C",
            BranchId = Guid.NewGuid(),
            BranchName = "B",
            Items = []
        };

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
    }
}
