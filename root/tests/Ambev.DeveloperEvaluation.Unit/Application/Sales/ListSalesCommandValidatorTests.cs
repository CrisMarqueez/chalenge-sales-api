using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Paginação da listagem de vendas alinhada a <c>.doc/general-api.md</c> (_page ≥ 1, _size razoável).
/// </summary>
public sealed class ListSalesCommandValidatorTests
{
    private readonly ListSalesCommandValidator _validator = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(100, 100)]
    public void Validate_page_e_pageSize_validos_passam(int page, int pageSize)
    {
        var cmd = new ListSalesCommand { Page = page, PageSize = pageSize };
        var r = _validator.Validate(cmd);
        Assert.True(r.IsValid);
    }

    [Fact]
    public void Validate_page_zero_falha()
    {
        var cmd = new ListSalesCommand { Page = 0, PageSize = 10 };
        var r = _validator.Validate(cmd);
        Assert.False(r.IsValid);
    }

    [Fact]
    public void Validate_pageSize_zero_falha()
    {
        var cmd = new ListSalesCommand { Page = 1, PageSize = 0 };
        var r = _validator.Validate(cmd);
        Assert.False(r.IsValid);
    }

    [Fact]
    public void Validate_pageSize_acima_do_limite_falha()
    {
        var cmd = new ListSalesCommand { Page = 1, PageSize = 101 };
        var r = _validator.Validate(cmd);
        Assert.False(r.IsValid);
    }
}
