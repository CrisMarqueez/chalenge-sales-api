using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

public class SalesApiProfile : Profile
{
    public SalesApiProfile()
    {
        CreateMap<SaleLineItemRequest, SaleLineItemData>();
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>()
            .ForMember(d => d.Id, o => o.Ignore());
        CreateMap<SaleLineItemResult, SaleLineItemResponse>();
        CreateMap<SaleDetailResult, SaleDetailResponse>();
        CreateMap<SaleSummaryResult, SaleSummaryResponse>();
    }
}
