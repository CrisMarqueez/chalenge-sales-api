using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    private static DocumentedErrorResponse ValidationErrorResponse(IEnumerable<ValidationFailure> failures) =>
        new()
        {
            Type = "ValidationError",
            Error = "Invalid input data",
            Detail = string.Join(" ", failures.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")),
        };

    [HttpPost]
    [ProducesResponseType(typeof(SaleDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(ValidationErrorResponse(validationResult.Errors));

        var command = _mapper.Map<CreateSaleCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(_mapper.Map<SaleDetailResponse>(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SaleDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSaleCommand(id), cancellationToken);
        return Ok(_mapper.Map<SaleDetailResponse>(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(DocumentedPagedListResponse<SaleSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListSales(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ListSalesCommand { Page = page, PageSize = pageSize }, cancellationToken);

        var mapped = _mapper.Map<List<SaleSummaryResponse>>(result.Items);
        var paged = new PaginatedList<SaleSummaryResponse>(mapped, result.TotalCount, result.Page, result.PageSize);

        return Ok(
            new DocumentedPagedListResponse<SaleSummaryResponse>
            {
                Data = paged.ToList(),
                CurrentPage = paged.CurrentPage,
                TotalPages = paged.TotalPages,
                TotalItems = paged.TotalCount,
            });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SaleDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateSale(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(ValidationErrorResponse(validationResult.Errors));

        var command = _mapper.Map<UpdateSaleCommand>(request);
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(_mapper.Map<SaleDetailResponse>(result));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DeleteMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSaleCommand(id), cancellationToken);

        return Ok(new DeleteMessageResponse { Message = "Sale deleted successfully" });
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(SaleDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSaleCommand(id), cancellationToken);
        return Ok(_mapper.Map<SaleDetailResponse>(result));
    }

    [HttpPost("{saleId:guid}/items/{itemId:guid}/cancel")]
    [ProducesResponseType(typeof(SaleDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DocumentedErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelSaleItem(
        [FromRoute] Guid saleId,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelSaleItemCommand(saleId, itemId), cancellationToken);
        return Ok(_mapper.Map<SaleDetailResponse>(result));
    }
}
