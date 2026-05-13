using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Security;

/// <summary>
/// Usuário autenticado da requisição HTTP (JWT).
/// </summary>
public interface ICurrentPrincipal
{
    Guid? UserId { get; }

    UserRole Role { get; }

    /// <summary>Administrador: listagens e detalhes sem filtro de proprietário.</summary>
    bool CanViewAllSales { get; }

    /// <summary>
    /// Indica se o usuário pode acessar uma venda com o <paramref name="ownerUserId"/> gravado.
    /// Vendas sem proprietário (legado) só são visíveis para administrador.
    /// </summary>
    bool CanAccessSale(Guid? ownerUserId);
}
