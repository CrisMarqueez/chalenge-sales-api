using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AdminUsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DocumentedPagedListResponse<AdminUserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListUsersQuery(page, pageSize), cancellationToken);
        var mapped = _mapper.Map<List<AdminUserResponse>>(result.Items);
        var paged = new PaginatedList<AdminUserResponse>(
            mapped,
            result.TotalCount,
            result.Page,
            result.PageSize);

        return Ok(
            new DocumentedPagedListResponse<AdminUserResponse>
            {
                Data = paged.ToList(),
                CurrentPage = paged.CurrentPage,
                TotalPages = paged.TotalPages,
                TotalItems = paged.TotalCount,
            });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<AdminUserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserCommand(id), cancellationToken);
        return Ok(
            new ApiResponseWithData<AdminUserResponse>
            {
                Success = true,
                Data = _mapper.Map<AdminUserResponse>(user),
            });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<AdminUserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateAdminUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateUserCommand>(request);
        command.Id = id;
        var user = await _mediator.Send(command, cancellationToken);
        return Ok(
            new ApiResponseWithData<AdminUserResponse>
            {
                Success = true,
                Message = "User updated successfully",
                Data = _mapper.Map<AdminUserResponse>(user),
            });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return Ok(new ApiResponse { Success = true, Message = "User deleted successfully" });
    }
}
