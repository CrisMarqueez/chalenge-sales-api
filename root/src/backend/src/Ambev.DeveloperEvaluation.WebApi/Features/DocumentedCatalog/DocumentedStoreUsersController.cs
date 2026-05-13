using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.DocumentedCatalog;

[ApiController]
[Route("users")]
public sealed class DocumentedStoreUsersController : ControllerBase
{
    private readonly DocumentedCatalogStore _store;
    private readonly IMediator _mediator;

    public DocumentedStoreUsersController(DocumentedCatalogStore store, IMediator mediator)
    {
        _store = store;
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DocumentedPagedListResponse<UserDto>), StatusCodes.Status200OK)]
    public ActionResult<DocumentedPagedListResponse<UserDto>> List(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null)
    {
        return Ok(_store.ListUsers(page, size, order));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<UserDto> Get([FromRoute] int id)
    {
        var u = _store.GetUser(id);
        if (u == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "User not found",
                Detail = $"The user with ID {id} does not exist in our database",
            });
        return Ok(u);
    }

    /// <summary>
    /// Corpo e resposta conforme <c>.doc/users-api.md</c> (utilizador completo, incluindo name e address).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] UserDto body, CancellationToken cancellationToken)
    {
        var validator = new DocumentedCreateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(
                new DocumentedErrorResponse
                {
                    Type = "ValidationError",
                    Error = "Invalid input data",
                    Detail = string.Join(" ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")),
                });
        }

        body.Id = 0;
        body.DomainId = null;

        try
        {
            return Ok(_store.CreateUser(body));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new DocumentedErrorResponse
            {
                Type = "ValidationError",
                Error = "Invalid input data",
                Detail = ex.Message,
            });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public ActionResult<UserDto> Update([FromRoute] int id, [FromBody] UserDto body)
    {
        var u = _store.UpdateUser(id, body);
        if (u == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "User not found",
                Detail = $"The user with ID {id} does not exist in our database",
            });
        return Ok(u);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var u = _store.GetUser(id);
        if (u == null)
            return NotFound(new DocumentedErrorResponse
            {
                Type = "ResourceNotFound",
                Error = "User not found",
                Detail = $"The user with ID {id} does not exist in our database",
            });

        if (u.DomainId is { } domainId)
            await _mediator.Send(new DeleteUserCommand(domainId), cancellationToken);

        _store.DeleteUser(id);
        return Ok(u);
    }
}
