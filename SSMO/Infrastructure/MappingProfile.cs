using AutoMapper;
using SSMO.Data.Models;
using SSMO.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Services.Products;

namespace SSMO.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Customer, AddCustomerFormModel>();
            this.CreateMap<MyCompany, MyCompanyFormModel>();
                this.CreateMap<CustomerOrder, CustomerOrderViewModel>();
            this.CreateMap<Product, ProductViewModel>()
                .ForMember(a=>a.OrderedQuantity, b=>b.MapFrom(a=>a.OrderedQuantity))
                .ForMember(a=>a.Price, b=>b.MapFrom(a=>a.Price));

            this.CreateMap<ProductViewModel, Product>()
                .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.OrderedQuantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.Price));

            this.CreateMap<Product, ProductSupplierDetails>();

            this.CreateMap<CustomerOrder, CustomerOrderDetailsModel>();

            this.CreateMap<Status, StatusModel>();
        }
    }
}
