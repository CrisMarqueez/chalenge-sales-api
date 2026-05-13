using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class SaleProfile : Profile
{
    public SaleProfile()
    {
        CreateMap<SaleLineItem, SaleLineItemResult>();
        CreateMap<Sale, SaleDetailResult>()
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items.OrderBy(i => i.ProductName)));
        CreateMap<Sale, SaleSummaryResult>();
    }
}
