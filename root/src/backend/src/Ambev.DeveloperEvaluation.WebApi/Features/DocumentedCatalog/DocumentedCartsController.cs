using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

[ApiController]
[Route("carts")]
public sealed class DocumentedCartsController : ControllerBase
{
    private readonly DocumentedCatalogStore _store;

    public DocumentedCartsController(DocumentedCatalogStore store)
    {
        _store = store;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DocumentedPagedListResponse<CartDto>), StatusCodes.Status200OK)]
    public ActionResult<DocumentedPagedListResponse<CartDto>> List(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        [FromQuery(Name = "_minDate")] DateTime? minDate = null,
        [FromQuery(Name = "_maxDate")] DateTime? maxDate = null)
    {
        return Ok(_store.ListCarts(page, size, order, minDate, maxDate));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<CartDto> Get([FromRoute] int id)
    {
        var c = _store.GetCart(id);
        if (c == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Cart not found",
                Detail = $"The cart with ID {id} does not exist in our database",
            });
        return Ok(c);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public ActionResult<CartDto> Create([FromBody] CartDto body)
    {
        body.Id = 0;
        return Ok(_store.CreateCart(body));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<CartDto> Update([FromRoute] int id, [FromBody] CartDto body)
    {
        var c = _store.UpdateCart(id, body);
        if (c == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Cart not found",
                Detail = $"The cart with ID {id} does not exist in our database",
            });
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<DeleteMessageResponse> Delete([FromRoute] int id)
    {
        if (!_store.DeleteCart(id))
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "Cart not found",
                Detail = $"The cart with ID {id} does not exist in our database",
            });
        return Ok(new DeleteMessageResponse { Message = "Cart deleted successfully" });
    }
}
