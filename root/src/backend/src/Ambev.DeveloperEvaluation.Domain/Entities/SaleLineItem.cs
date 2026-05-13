using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleLineItem : BaseEntity
{
    public Guid SaleId { get; set; }
    public Sale? Sale { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LineTotal { get; set; }
    public bool IsCancelled { get; set; }

    public void RecalculateLine()
    {
        if (IsCancelled)
        {
            DiscountAmount = 0;
            LineTotal = 0;
            return;
        }

        SaleDiscountPolicy.Apply(this);
    }
}
