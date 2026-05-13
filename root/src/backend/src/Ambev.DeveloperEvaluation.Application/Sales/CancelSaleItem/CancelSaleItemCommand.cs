using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public record CancelSaleItemCommand(Guid SaleId, Guid LineItemId) : IRequest<SaleDetailResult>;
