using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

/// <summary>
/// Quantity tiers: 1–3 sem desconto; 4–9: 10%; 10–20: 20%; acima de 20 é inválido (validado à parte).
/// </summary>
public static class SaleDiscountPolicy
{
    public const int MaxQuantityPerProduct = 20;

    public static decimal GetDiscountRate(int quantity)
    {
        if (quantity is < 1 or > MaxQuantityPerProduct)
            return 0;

        if (quantity >= 10)
            return 0.20m;

        if (quantity >= 4)
            return 0.10m;

        return 0;
    }

    public static void Apply(SaleLineItem line)
    {
        if (line.Quantity > MaxQuantityPerProduct)
            throw new InvalidOperationException($"Cannot sell more than {MaxQuantityPerProduct} identical items of a product per line.");

        var subtotal = line.Quantity * line.UnitPrice;
        var rate = GetDiscountRate(line.Quantity);
        line.DiscountAmount = Math.Round(subtotal * rate, 2, MidpointRounding.AwayFromZero);
        line.LineTotal = Math.Round(subtotal - line.DiscountAmount, 2, MidpointRounding.AwayFromZero);
    }
}
