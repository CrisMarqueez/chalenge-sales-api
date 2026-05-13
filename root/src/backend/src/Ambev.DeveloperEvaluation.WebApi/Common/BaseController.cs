using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new NullReferenceException());

    protected string GetCurrentUserEmail() =>
        User.FindFirst(ClaimTypes.Email)?.Value ?? throw new NullReferenceException();

    protected IActionResult Ok<T>(T data) =>
            base.Ok(new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult Created<T>(string routeName, object routeValues, T data) =>
        base.CreatedAtRoute(routeName, routeValues, new ApiResponseWithData<T> { Data = data, Success = true });

    protected IActionResult BadRequest(string message) =>
        base.BadRequest(new ApiResponse { Message = message, Success = false });

    protected IActionResult NotFound(string message = "Resource not found") =>
        base.NotFound(new ApiResponse { Message = message, Success = false });

    /// <summary>Lista paginada no formato <c>.doc/products-api.md</c> (sem envelope <c>success</c>/<c>data</c> extra).</summary>
    protected IActionResult OkPaginated<T>(PaginatedList<T> pagedList) =>
        base.Ok(new DocumentedPagedListResponse<T>
        {
            Data = pagedList.ToList(),
            CurrentPage = pagedList.CurrentPage,
            TotalPages = pagedList.TotalPages,
            TotalItems = pagedList.TotalCount,
        });
}
