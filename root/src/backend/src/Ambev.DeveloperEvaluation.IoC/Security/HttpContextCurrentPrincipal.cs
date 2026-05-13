using System.Security.Claims;
using Ambev.DeveloperEvaluation.Application.Security;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Ambev.DeveloperEvaluation.IoC.Security;

public sealed class HttpContextCurrentPrincipal : ICurrentPrincipal
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentPrincipal(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            if (Principal is null) return null;
            foreach (var type in UserIdClaimTypes)
            {
                var v = Principal.FindFirstValue(type);
                if (Guid.TryParse(v, out var id)) return id;
            }
            return null;
        }
    }

    public UserRole Role
    {
        get
        {
            var r = FindRoleClaimValue(Principal);
            if (string.IsNullOrWhiteSpace(r))
                return UserRole.None;
            return Enum.TryParse<UserRole>(r, ignoreCase: true, out var role)
                ? role
                : UserRole.None;
        }
    }

    /// <summary>Administrador: claim de perfil numérica 3 (ou nome legado), resolvida em <see cref="UserRole"/>.</summary>
    public bool CanViewAllSales => Role == UserRole.Admin;

    public bool CanAccessSale(Guid? ownerUserId)
    {
        if (CanViewAllSales)
            return true;
        if (UserId is not Guid uid)
            return false;
        if (!ownerUserId.HasValue)
            return false;
        return ownerUserId.Value == uid;
    }

    /// <summary>Nomes curtos (JWT) e URI (.NET) após possível mapeamento de claims.</summary>
    private static readonly string[] UserIdClaimTypes =
    [
        ClaimTypes.NameIdentifier,
        "sub",
        "nameid",
    ];

    private static string? FindRoleClaimValue(ClaimsPrincipal? principal)
    {
        if (principal is null) return null;
        if (principal.Identity is ClaimsIdentity ci && !string.IsNullOrEmpty(ci.RoleClaimType))
        {
            var v = principal.FindFirstValue(ci.RoleClaimType);
            if (!string.IsNullOrWhiteSpace(v)) return v;
        }

        return principal.FindFirstValue(ClaimTypes.Role)
            ?? principal.FindFirstValue("role");
    }
}
