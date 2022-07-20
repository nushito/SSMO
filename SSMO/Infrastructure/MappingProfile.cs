using AutoMapper;
using SSMO.Data.Models;
using SSMO.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;

namespace SSMO.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Customer, AddCustomerFormModel>();
            this.CreateMap<MyCompany, MyCompanyFormModel>();
            this.CreateMap<CustomerOrder, CustomerOrderReport>();
            this.CreateMap<CustomerOrder, CustomerOrderViewModel>();
            this.CreateMap<Product, ProductViewModel>()
                .ForMember(a=>a.Cubic,b=>b.MapFrom(a=>a.OrderedQuantity))
                .ForMember(a=>a.CostPrice, b=>b.MapFrom(a=>a.Price));

            this.CreateMap<ProductViewModel, Product>()
                .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.Cubic))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.CostPrice));

            this.CreateMap<Status, StatusModel>();
        }
    }
}
