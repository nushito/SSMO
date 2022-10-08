using AutoMapper;
using SSMO.Data.Models;
using SSMO.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Documents.Purchase;
using SSMO.Services.Products;
using SSMO.Services.Reports;

namespace SSMO.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Customer, AddCustomerFormModel>();
            this.CreateMap<MyCompany, MyCompanyFormModel>();
                this.CreateMap<CustomerOrder, CustomerOrderViewModel>();

            this.CreateMap<Product, ProductCustomerFormModel>()
                .ForMember(a=>a.OrderedQuantity, b=>b.MapFrom(a=>a.OrderedQuantity))
                .ForMember(a=>a.Price, b=>b.MapFrom(a=>a.Price));

            this.CreateMap<ProductCustomerFormModel, Product>()
                .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.OrderedQuantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.Price));

            this.CreateMap<Product, ProductSupplierDetails>();

            this.CreateMap<CustomerOrder, CustomerOrderDetailsModel>();

            this.CreateMap<Status, StatusModel>();

            this.CreateMap<SupplierOrder, PurchaseModelAsPerSpec>()
                .ForMember(a=>a.SupplierOrderNumber , b=>b.MapFrom(a=>a.Number));

            this.CreateMap<Product, ProductsForInvoiceModel>();
            this.CreateProjection<ProductCustomerFormModel, Product>()
                  .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.OrderedQuantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.Price)); ;
            this.CreateMap<Document, InvoicePrintViewModel>();
        }
    }
}
