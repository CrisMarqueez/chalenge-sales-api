namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Formato de erro descrito em <c>.doc/general-api.md</c>.
/// </summary>
public sealed class DocumentedErrorResponse
{
    public string Type { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
}
