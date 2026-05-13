namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class SaleDetailResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public IReadOnlyList<SaleLineItemResult> Items { get; set; } = Array.Empty<SaleLineItemResult>();
}
