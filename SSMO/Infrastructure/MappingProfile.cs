using AutoMapper;
using SSMO.Data.Models;
using SSMO.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Documents.Packing_List;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.FscTexts;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.Purchase;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Models.ServiceOrders;
using SSMO.Models.Suppliers;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Documents.Purchase;
using SSMO.Services.Products;
using SSMO.Services.Reports;
using System.Threading;

namespace SSMO.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Customer, AddCustomerFormModel>();
            this.CreateMap<Customer, EditCustomerFormModel>();
            this.CreateMap<Customer, InvoiceCustomerDetailsModel>();

            this.CreateMap<MyCompany, MyCompanyFormModel>();
            this.CreateMap<MyCompany, MyCompaniesForReportViewModel>();
            this.CreateMap<MyCompany, MyCompanyInvoiceDetailsModel>();
            this.CreateMap<MyCompany, MyCompanyEditFormModel>();
            this.CreateMap<MyCompany, MyCompaniesForTrasnportOrderViewModel>();

            this.CreateMap<CustomerOrder, CustomerOrderViewModel>();
            this.CreateMap<CustomerOrder, CustomerOrderDetailsModel>();
            this.CreateMap<CustomerOrder, CustomerOrderDetailsPaymentModel>();
            this.CreateMap<CustomerOrder, EditCustomerOrderPaymentModel>();
            this.CreateMap<CustomerOrder, CustomerOrderForInvoiceViewModel>();
            this.CreateMap<CustomerOrder, CustomerOrdersPaymentDetailsPerInvoice>();

            this.CreateMap<Product, ProductCustomerFormModel>()
                .ForMember(a=>a.Quantity, b=>b.MapFrom(a=>a.OrderedQuantity))
                .ForMember(a=>a.SellPrice, b=>b.MapFrom(a=>a.Price));
            this.CreateMap<Product, Models.Documents.Packing_List.ProductsForPackingListModel>();
            this.CreateMap<ProductCustomerFormModel, Product>()
                .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.Quantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.SellPrice));
            this.CreateMap<Product, ProductPerSupplierOrderDetails>();
            this.CreateMap<Product, Services.Documents.Invoice.ProductsForInvoiceModel>();
            this.CreateProjection<ProductCustomerFormModel, Product>()
                .ForMember(a => a.OrderedQuantity, b => b.MapFrom(a => a.Quantity))
                .ForMember(a => a.Price, b => b.MapFrom(a => a.SellPrice));
            this.CreateMap<Product, ProductsForEditSupplierOrder>();
            this.CreateMap<InvoiceProductDetails, BGProductsForBGInvoiceViewModel>();
            this.CreateMap<InvoiceProductDetails, InvoiceProductsDetailsViewModel>();          
            this.CreateMap<InvoiceProductDetails, EditProductForCompanyInvoicesViewModel>();
           
            this.CreateMap<Product, ProductsSupplierOrderDetailsViewModel>();
            this.CreateMap<Product, PurchaseProductAsSupplierOrderViewModel>();
            this.CreateMap<ProductsForInvoiceViewModel, Services.Documents.Invoice.ProductsForInvoiceModel>();
            this.CreateMap<InvoiceProductDetails, ProductsForPackingListModel>();
            this.CreateMap<CustomerOrderProductDetails, ProductCustomerFormModel>();
            this.CreateMap<CustomerOrderProductDetails, ProductsForCustomerOrderDetailsViewModel>();
            this.CreateMap<PurchaseProductDetails, PurchaseProductsForEditFormModel>();

            this.CreateMap<Status, StatusModel>();

            this.CreateMap<SupplierOrder, PurchaseModelAsPerSpec>()
                .ForMember(a=>a.SupplierOrderNumber , b=>b.MapFrom(a=>a.Number));
            this.CreateMap<SupplierOrder, SupplierOrdersPaymentDetailsModel>()
                .ForMember(a => a.SupplierOrderNumber, b => b.MapFrom(a => a.Number));
            this.CreateMap<SupplierOrder, SupplierOrderDetailsModel>()
                .ForMember(a => a.SupplierOrderNumber, b => b.MapFrom(a => a.Number));
            this.CreateMap<SupplierOrder, SupplierOrdersListForPurchaseEditModel>();
            this.CreateMap<SupplierOrder, EditSupplierOrderPaymentModel>();

            this.CreateMap<Document, InvoicePrintViewModel>();
            this.CreateMap<Document, EditInvoicePaymentModel>();
            this.CreateMap<Document, SupplierInvoicePaymentDetailsModel>();
            this.CreateMap<Document, CustomerInvoicePaymentDetailsModel>();
            this.CreateMap<Document, InvoiceCollectionViewModel>();
            this.CreateMap<Document, InvoiceDetailsViewModel>();
            this.CreateMap<Document, PurchaseInvoicesViewModel>();
            this.CreateMap<Document, EditPurchasePaymentDetails>();
           
            this.CreateMap<Address, CustomerForEditAddressFormModel>();
            this.CreateMap<Address, EditSupplierAddressFormModel>();

            this.CreateMap<Supplier, EditSupplierFormModel>();

            this.CreateMap<BankDetails, InvoiceBankDetailsModel>();

            this.CreateMap<Payment, SupplierPaymentDetailsViewModel>();
            this.CreateMap<Payment, PurchaseAllPaymentsViewModel>();
            this.CreateMap<Payment, PaymentViewModel>();

            this.CreateMap<FiscalAgent, FiscalAgentViewModel>();

            this.CreateMap<FscText, FscTextViewModel>();
        }
    }
}
