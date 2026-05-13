using System.Net;
using System.Text.Json;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

/// <summary>
/// Garante que <c>GET /products</c> segue o contrato de <c>.doc/general-api.md</c> e <c>.doc/products-api.md</c>.
/// </summary>
public class DocumentedProductsContractTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DocumentedProductsContractTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_products_usa_parametros_e_corpo_documentados()
    {
        using var client = _factory.WithWebHostBuilder(b => b.UseEnvironment("Testing")).CreateClient();

        using var res = await client.GetAsync("/products?_page=1&_size=2");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        await using var stream = await res.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.GetProperty("data").ValueKind);
        Assert.Equal(2, root.GetProperty("data").GetArrayLength());
        Assert.True(root.GetProperty("totalItems").GetInt32() >= 3);
        Assert.Equal(1, root.GetProperty("currentPage").GetInt32());
        Assert.True(root.GetProperty("totalPages").GetInt32() >= 1);
        Assert.False(root.TryGetProperty("success", out _));
    }
}
