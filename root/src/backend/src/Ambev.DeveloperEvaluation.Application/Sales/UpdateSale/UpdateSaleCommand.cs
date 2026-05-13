using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<SaleDetailResult>
{
    public Guid Id { get; set; }
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public IList<SaleLineItemData> Items { get; set; } = new List<SaleLineItemData>();
}
