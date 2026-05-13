using Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

/// <summary>
/// Cobre regras de <c>.doc/general-api.md</c>, <c>products-api.md</c>, <c>carts-api.md</c> e <c>users-api.md</c>
/// sobre o protótipo em memória <see cref="DocumentedCatalogStore"/>.
/// </summary>
public sealed class DocumentedCatalogStoreTests
{
    private readonly DocumentedCatalogStore _store = new();

    [Fact]
    public void ListProducts_paginacao_usa_totalItems_currentPage_totalPages_como_na_doc()
    {
        // general-api.md + products-api.md: _page, _size → data[], totalItems, currentPage, totalPages
        var r = _store.ListProducts(page: 1, size: 2, order: null, title: null, category: null, price: null, minPrice: null, maxPrice: null);

        Assert.Equal(2, r.Data.Count);
        Assert.True(r.TotalItems >= 3);
        Assert.Equal(1, r.CurrentPage);
        Assert.True(r.TotalPages >= 2);
    }

    [Fact]
    public void ListProducts_segunda_pagina_respeita_tamanho()
    {
        var r = _store.ListProducts(page: 2, size: 2, order: null, title: null, category: null, price: null, minPrice: null, maxPrice: null);

        Assert.Single(r.Data);
        Assert.Equal(2, r.CurrentPage);
    }

    [Fact]
    public void ListProducts_filtro_categoria_igualdade_como_na_doc()
    {
        // GET /products?category=men's clothing
        var r = _store.ListProducts(1, 10, null, null, "men's clothing", null, null, null);

        Assert.Equal(2, r.TotalItems);
        Assert.All(r.Data, p => Assert.Equal("men's clothing", p.Category, ignoreCase: true));
    }

    [Fact]
    public void ListProducts_filtro_titulo_prefixo_asterisco_como_na_doc()
    {
        // GET /products?title=Fjallraven*
        var r = _store.ListProducts(1, 10, null, "Fjallraven*", null, null, null, null);

        Assert.Single(r.Data);
        Assert.Contains("Fjallraven", r.Data[0].Title, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ListProducts_filtro_categoria_sufixo_asterisco_como_na_doc()
    {
        // GET /products?category=*clothing
        var r = _store.ListProducts(1, 10, null, null, "*clothing", null, null, null);

        Assert.Equal(2, r.TotalItems);
    }

    [Fact]
    public void ListProducts_faixa_de_preco_min_max_como_na_doc()
    {
        // GET /products?_minPrice=50&_maxPrice=200
        var r = _store.ListProducts(1, 10, null, null, null, null, 50m, 200m);

        Assert.Single(r.Data);
        Assert.Equal(109.95m, r.Data[0].Price);
    }

    [Fact]
    public void ListProducts_ordenacao_price_desc_como_na_doc()
    {
        // GET /products?_order="price desc, title asc"
        var r = _store.ListProducts(1, 10, "price desc, title asc", null, null, null, null, null);

        Assert.True(r.Data.Count >= 3);
        Assert.Equal(695m, r.Data[0].Price);
        Assert.Equal(22.3m, r.Data[^1].Price);
    }

    [Fact]
    public void ListProducts_pagina_e_tamanho_invalidos_normalizam_como_esperado_em_listagens()
    {
        var r = _store.ListProducts(page: 0, size: 0, null, null, null, null, null, null);

        Assert.Equal(1, r.CurrentPage);
        Assert.Equal(3, r.Data.Count);
        Assert.Equal(3, r.TotalItems);
    }

    [Fact]
    public void ListProductsByCategory_filtra_apenas_categoria_informada()
    {
        var r = _store.ListProductsByCategory("jewelery", 1, 10, null);

        Assert.Single(r.Data);
        Assert.Equal("jewelery", r.Data[0].Category, ignoreCase: true);
    }

    [Fact]
    public void GetProductCategories_retorna_lista_sem_duplicatas()
    {
        var cats = _store.GetProductCategories();

        Assert.Contains("men's clothing", cats, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("jewelery", cats, StringComparer.OrdinalIgnoreCase);
        Assert.Equal(cats.Count, cats.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void ListCarts_filtro_minDate_como_na_doc()
    {
        // GET /carts?_minDate=2023-01-01 — carrinho seed é 2020-10-10
        var antes = _store.ListCarts(1, 10, null, new DateTime(2020, 1, 1), null);
        Assert.True(antes.TotalItems >= 1);

        var depois = _store.ListCarts(1, 10, null, new DateTime(2023, 1, 1), null);
        Assert.Equal(0, depois.TotalItems);
    }

    [Fact]
    public void ListCarts_filtro_maxDate_como_na_doc()
    {
        var antes = _store.ListCarts(1, 10, null, null, new DateTime(2021, 1, 1));
        Assert.True(antes.TotalItems >= 1);

        var exclui = _store.ListCarts(1, 10, null, null, new DateTime(2019, 12, 31));
        Assert.Equal(0, exclui.TotalItems);
    }

    [Fact]
    public void ListCarts_faixa_minDate_maxDate_inclui_seed()
    {
        var r = _store.ListCarts(1, 10, null, new DateTime(2020, 1, 1), new DateTime(2020, 12, 31));
        Assert.True(r.TotalItems >= 1);
        Assert.Equal("2020-10-10", r.Data[0].Date);
    }

    [Fact]
    public void ListProducts_ordenacao_rating_rate_asc_usa_nomes_json_como_na_doc()
    {
        var r = _store.ListProducts(1, 10, "rating.rate asc", null, null, null, null, null);

        Assert.True(r.Data.Count >= 3);
        for (var i = 1; i < r.Data.Count; i++)
            Assert.True(r.Data[i - 1].Rating.Rate <= r.Data[i].Rating.Rate);
    }

    [Fact]
    public void ListUsers_ordenacao_username_desc()
    {
        var r = _store.ListUsers(1, 10, "username desc");

        Assert.True(r.TotalItems >= 1);
        Assert.Equal("johnd", r.Data[0].Username, ignoreCase: true);
    }

    [Fact]
    public void GetUser_retorna_usuario_seed()
    {
        var u = _store.GetUser(1);

        Assert.NotNull(u);
        Assert.Equal("john@example.com", u.Email);
        Assert.Equal("johnd", u.Username, ignoreCase: true);
    }

    [Fact]
    public void CreateProduct_atribui_novo_id_e_preserva_campos()
    {
        var novo = new ProductDto
        {
            Title = "Unit Test Product",
            Price = 9.99m,
            Description = "D",
            Category = "electronics",
            Image = "https://example.com/x.png",
            Rating = new ProductRatingDto { Rate = 5m, Count = 1 },
        };

        var criado = _store.CreateProduct(novo);

        Assert.True(criado.Id > 0);
        Assert.Equal("Unit Test Product", criado.Title);
        var listado = _store.GetProduct(criado.Id);
        Assert.NotNull(listado);
        Assert.Equal(criado.Id, listado.Id);
    }

    [Fact]
    public void FindCatalogUserByUsername_encontra_johnd()
    {
        var u = _store.FindCatalogUserByUsername("JOHND");
        Assert.NotNull(u);
        Assert.Equal("john@example.com", u!.Email);
    }
}
