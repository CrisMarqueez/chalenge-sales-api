using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

[ApiController]
[Route("products")]
public sealed class DocumentedProductsController : ControllerBase
{
    private readonly DocumentedCatalogStore _store;

    public DocumentedProductsController(DocumentedCatalogStore store)
    {
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DocumentedPagedListResponse<ProductDto>), StatusCodes.Status200OK)]
    public ActionResult<DocumentedPagedListResponse<ProductDto>> List(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        [FromQuery] string? title = null,
        [FromQuery] string? category = null,
        [FromQuery] decimal? price = null,
        [FromQuery(Name = "_minPrice")] decimal? minPrice = null,
        [FromQuery(Name = "_maxPrice")] decimal? maxPrice = null)
    {
        return Ok(_store.ListProducts(page, size, order, title, category, price, minPrice, maxPrice));
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public ActionResult<List<string>> Categories()
    {
        return Ok(_store.GetProductCategories().ToList());
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(DocumentedPagedListResponse<ProductDto>), StatusCodes.Status200OK)]
    public ActionResult<DocumentedPagedListResponse<ProductDto>> ByCategory(
        [FromRoute] string category,
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null)
    {
        return Ok(_store.ListProductsByCategory(category, page, size, order));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> Get([FromRoute] int id)
    {
        var p = _store.GetProduct(id);
        if (p == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Product not found",
                Detail = $"The product with ID {id} does not exist in our database",
            });
        return Ok(p);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public ActionResult<ProductDto> Create([FromBody] ProductDto body)
    {
        body.Id = 0;
        return Ok(_store.CreateProduct(body));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<ProductDto> Update([FromRoute] int id, [FromBody] ProductDto body)
    {
        var p = _store.UpdateProduct(id, body);
        if (p == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Product not found",
                Detail = $"The product with ID {id} does not exist in our database",
            });
        return Ok(p);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<DeleteMessageResponse> Delete([FromRoute] int id)
    {
        if (!_store.DeleteProduct(id))
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Product not found",
                Detail = $"The product with ID {id} does not exist in our database",
            });
        return Ok(new DeleteMessageResponse { Message = "Product deleted successfully" });
    }
}
