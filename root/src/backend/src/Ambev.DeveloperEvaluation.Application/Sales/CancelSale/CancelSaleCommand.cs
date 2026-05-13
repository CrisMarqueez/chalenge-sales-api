using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public record CancelSaleCommand(Guid SaleId) : IRequest<SaleDetailResult>;
