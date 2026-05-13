namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class CreateSaleRequest
{
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public IList<SaleLineItemRequest> Items { get; set; } = new List<SaleLineItemRequest>();
}
