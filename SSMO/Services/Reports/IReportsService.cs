using SSMO.Models.Products;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Models.Reports.Purchase;
using SSMO.Models.Reports.ServiceOrders;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSMO.Services.Reports
{
    public interface IReportsService
    {
        CustomerOrdersQueryModel AllCustomerOrders(string name, int myCompanyId, DateTime startDate, DateTime endDate,
            int currentpage, int customerOrdersPerPage);
        SupplierOrdersQueryModel AllSupplierOrders(string name, int myCompanyId, DateTime startDate, DateTime endDate,
            int currentpage, int supplierOrdersPerPage);
        SupplierOrderDetailsModel SupplierOrderDetail(int id);
        //CustomerOrderDetailsModel CustomerOrderDetails(int id);
        Task<bool> EditCustomerOrder(int id, string number,
                System.DateTime date,
                int myCompanyId,
                string deliveryTerms,
                string loadingPlace,
                string deliveryAddress,
               int currencyId, int status,
               string fscClaim, string fscCertificate,              
               IList<ProductCustomerFormModel> products, List<int> banks, 
               int? fiscalAgentId, int? fscText, string paymentTerms, string eta, string etd);
        public IEnumerable<CustomerOrderListViewBySupplier> GetCustomerOrdersBySupplier(int customerId, string supplierId);
        public CustomerInvoicesPaymentCollectionViewModel CustomersInvoicesPaymentDetails(string customerName, int currentpage, int myCompanyId, int customerInvoicePerPage);
        public SupplierInvoiceCollectionViewModel SuppliersInvoicesPaymentDetails(string supplierName, int currentpage, int supplierInvoicePerPage);

        public CustomerOrderForEdit CustomerOrderDetailsForEdit(int id);

        public CustomerOrderPaymentCollectionViewModel CustomerOrdersPaymentDetails(string customerName, int currentpage,int myCompanyId, int customerOrdersPerPage);
        public SupplierOrderForEditModel SupplierOrderForEditDetails(int supplierOrderId);

        Task<bool> EditSupplierOrder
            (int supplierOrderId, string supplierOrderNumber, DateTime date,int myCompanyId,
               string deliveryTerms,string loadingPlace, string deliveryAddress,decimal grossWeight, 
               decimal netWeight, int currencyId, int status, int customerOrderNumber, int? vat,
              List<ProductsForEditSupplierOrder> products, List<NewProductsForSupplierOrderModel> newProducts);

        public InvoiceReportModel InvoiceCollection(int myCompanyId, DateTime startDate, DateTime endDate, int currentpage, int invoicesPerPage);
        public InvoiceDetailsViewModel InvoiceDetails(int id,int header, int footer);

        public PurchaseCollectionQueryModel PurchaseInvoices
            (string supplierName, int myCompanyId, DateTime startDate, DateTime endDate, int currentpage, int invoiceperPage);

        public ServiceOrdersQueryModel ServiceOrdersCollection
            (int companyId, DateTime startDate, DateTime endDate, int currentPage, int ordersPerPage);

    }
}

