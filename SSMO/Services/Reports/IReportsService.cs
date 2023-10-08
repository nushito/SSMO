using SSMO.Models.Products;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Models.Reports.Purchase;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Reports
{
    public interface IReportsService
    {
        CustomerOrdersQueryModel AllCustomerOrders(string name, DateTime startDate, DateTime endDate,
            int currentpage, int customerOrdersPerPage);
        SupplierOrdersQueryModel AllSupplierOrders(string name,DateTime startDate, DateTime endDate,
            int currentpage, int supplierOrdersPerPage);
        SupplierOrderDetailsModel SupplierOrderDetail(int id);
        CustomerOrderDetailsModel CustomerOrderDetails(int id);
        bool EditCustomerOrder(int id, string number,
                System.DateTime date,
                int myCompanyId,
                string deliveryTerms,
                string loadingPlace,
                string deliveryAddress,
               int currencyId, int status,
               string fscClaim, string fscCertificate,
               decimal paidAdvance, bool paidStatus,
               IList<ProductCustomerFormModel> products, List<int> banks);
        public IEnumerable<CustomerOrderListViewBySupplier> GetCustomerOrdersBySupplier(int customerId, string supplierId);
        public CustomerInvoicesPaymentCollectionViewModel CustomersInvoicesPaymentDetails(string customerName, int currentpage, int customerInvoicePerPage);
        public SupplierInvoiceCollectionViewModel SuppliersInvoicesPaymentDetails(string supplierName, int currentpage, int supplierInvoicePerPage);

        public CustomerOrderForEdit CustomerOrderDetailsForEdit(int id);

        public CustomerOrderPaymentCollectionViewModel CustomerOrdersPaymentDetails(string customerName, int currentpage, int customerOrdersPerPage);
        public SupplierOrderForEditModel SupplierOrderForEditDetails(int supplierOrderId);

        bool EditSupplierOrder(int supplierOrderId, string supplierOrderNumber,
               DateTime date,
               int myCompanyId,
               string deliveryTerms,
               string loadingPlace, string deliveryAddress,
               decimal grossWeight, decimal netWeight,
              int currencyId, int status, int customerOrderNumber,
              string fscClaim, string fscCertificate,
              decimal paidAdvance, bool paidStatus, int? vat,
              List<ProductsForEditSupplierOrder> products, List<NewProductsForSupplierOrderModel> newProducts);

        public InvoiceReportModel InvoiceCollection(string myCompanyName, DateTime startDate, DateTime endDate, int currentpage, int invoicesPerPage);
        public InvoiceDetailsViewModel InvoiceDetails(int id);

        public PurchaseCollectionQueryModel PurchaseInvoices
            (string supplierName, DateTime startDate, DateTime endDate, int currentpage, int invoiceperPage);

    }
}

