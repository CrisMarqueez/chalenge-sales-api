using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesResult
{
    public IReadOnlyList<SaleSummaryResult> Items { get; set; } = Array.Empty<SaleSummaryResult>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
