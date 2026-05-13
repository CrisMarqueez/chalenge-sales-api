using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

internal static class SaleAccess
{
    /// <summary>
    /// Falha como recurso inexistente para não expor vendas de outros usuários.
    /// </summary>
    public static void EnsureCanAccess(ICurrentPrincipal user, Sale sale)
    {
        if (user.CanAccessSale(sale.OwnerUserId))
            return;
        throw new KeyNotFoundException($"Sale with ID {sale.Id} not found");
    }
}
