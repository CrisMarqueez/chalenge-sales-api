using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

/// <summary>
/// Garante serialização JSON alinhada a <c>.doc/general-api.md</c> (paginação e erros).
/// </summary>
public sealed class DocumentedApiJsonContractTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact]
    public void DocumentedErrorResponse_serializa_type_error_detail_em_camelCase()
    {
        var body = new DocumentedErrorResponse
        {
            Type = "ValidationError",
            Error = "Invalid input data",
            Detail = "The 'price' field must be a positive number",
        };

        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("ValidationError", root.GetProperty("type").GetString());
        Assert.Equal("Invalid input data", root.GetProperty("error").GetString());
        Assert.Equal("The 'price' field must be a positive number", root.GetProperty("detail").GetString());
    }

    [Fact]
    public void DocumentedPagedListResponse_serializa_data_totalItems_currentPage_totalPages()
    {
        var body = new DocumentedPagedListResponse<string>
        {
            Data = ["a", "b"],
            TotalItems = 5,
            CurrentPage = 1,
            TotalPages = 3,
        };

        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.GetProperty("data").ValueKind);
        Assert.Equal(2, root.GetProperty("data").GetArrayLength());
        Assert.Equal(5, root.GetProperty("totalItems").GetInt32());
        Assert.Equal(1, root.GetProperty("currentPage").GetInt32());
        Assert.Equal(3, root.GetProperty("totalPages").GetInt32());
        Assert.False(root.TryGetProperty("success", out _));
    }
}
