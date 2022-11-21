using AutoMapper;
using SSMO.Data.Models;
using SSMO.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Documents.Packing_List;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Models.Suppliers;
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
            this.CreateMap<Customer, EditCustomerFormModel>();

            this.CreateMap<MyCompany, MyCompanyFormModel>();
            this.CreateMap<MyCompany, MyCompaniesForReportViewModel>();

            this.CreateMap<CustomerOrder, CustomerOrderViewModel>();
            this.CreateMap<CustomerOrder, CustomerOrderDetailsModel>();
            this.CreateMap<CustomerOrder, CustomerOrderDetailsPaymentModel>();
            this.CreateMap<CustomerOrder, EditCustomerOrderPaymentModel>();

            this.CreateMap<Product, ProductCustomerFormModel>()
                .ForMember(a=>a.OrderedQuantity, b=>b.MapFrom(a=>a.OrderedQuantity))
                .ForMember(a=>a.Price, b=>b.MapFrom(a=>a.Price));
            this.CreateMap<Product, ProductsForPackingListPrint>();
            this.CreateMap<ProductCustomerFormModel, Product>()
                .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.OrderedQuantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.Price));
            this.CreateMap<Product, ProductSupplierDetails>();
            this.CreateMap<Product, ProductsForInvoiceModel>();
            this.CreateProjection<ProductCustomerFormModel, Product>()
                  .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.OrderedQuantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.Price));
            this.CreateMap<Product, ProductsForEditSupplierOrder>();

          

            this.CreateMap<Status, StatusModel>();

            this.CreateMap<SupplierOrder, PurchaseModelAsPerSpec>()
                .ForMember(a=>a.SupplierOrderNumber , b=>b.MapFrom(a=>a.Number));
            this.CreateMap<SupplierOrder, SupplierOrdersPaymentDetailsModel>()
                .ForMember(a => a.SupplierOrderNumber, b => b.MapFrom(a => a.Number));
            this.CreateMap<SupplierOrder, SupplierOrderDetailsModel>()
                .ForMember(a => a.SupplierOrderNumber, b => b.MapFrom(a => a.Number));


            this.CreateMap<Document, InvoicePrintViewModel>();
            this.CreateMap<Document, EditInvoicePaymentModel>();
            this.CreateMap<Document, SupplierInvoicePaymentDetailsModel>();
            this.CreateMap<Document, CustomerInvoicePaymentDetailsModel>();
           
            this.CreateMap<Address, CustomerForEditAddressFormModel>();
            this.CreateMap<Address, EditSupplierAddressFormModel>();

            this.CreateMap<Supplier, EditSupplierFormModel>();
           
           
        }
    }
}
