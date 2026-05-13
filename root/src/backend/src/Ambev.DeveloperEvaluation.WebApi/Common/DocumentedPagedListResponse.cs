namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Formato de listagem paginada descrito em <c>.doc/general-api.md</c> e nos recursos products/carts/users.
/// </summary>
public sealed class DocumentedPagedListResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = [];
    public int TotalItems { get; init; }
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
}
