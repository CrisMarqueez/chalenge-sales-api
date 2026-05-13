namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class UpdateSaleRequest
{
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public IList<SaleLineItemRequest> Items { get; set; } = new List<SaleLineItemRequest>();
}
