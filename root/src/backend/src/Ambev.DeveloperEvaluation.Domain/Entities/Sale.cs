using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Usuário que criou/possui a venda (para escopo em listagem: admin vê todas; demais, só as próprias).
    /// </summary>
    public Guid? OwnerUserId { get; set; }

    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<SaleLineItem> Items { get; set; } = new List<SaleLineItem>();

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void RecalculateTotals()
    {
        if (IsCancelled)
        {
            TotalAmount = 0;
            return;
        }

        decimal total = 0;
        foreach (var item in Items)
        {
            item.RecalculateLine();
            if (!item.IsCancelled)
                total += item.LineTotal;
        }

        TotalAmount = total;
    }

    public void AssignSaleNumberFromId()
    {
        var prefix = SaleDate == default ? DateTime.UtcNow : SaleDate.ToUniversalTime();
        SaleNumber = $"S-{prefix:yyyyMMdd}-{Id.ToString("N")[..8].ToUpperInvariant()}";
    }
}
