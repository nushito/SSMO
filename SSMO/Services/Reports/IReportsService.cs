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
        IEnumerable<CustomerOrderDetailsModel> AllCustomerOrders(string name, int currentpage, int customerOrdersPerPage);

        IEnumerable<SupplierOrderDetailsModel> AllSupplierOrders(string name, int currentpage, int supplierOrdersPerPage);
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
               List<ProductCustomerFormModel> products);
        public IEnumerable<CustomerOrderListViewBySupplier> GetCustomerOrdersBySupplier(int customerId, string supplierId);
        public IEnumerable<CustomerInvoicePaymentDetailsModel> CustomersInvoicesPaymentDetails(string customerName, int currentpage, int customerInvoicePerPage);
        public IEnumerable<SupplierInvoicePaymentDetailsModel> SuppliersInvoicesPaymentDetails(string supplierName, int currentpage, int supplierInvoicePerPage);

        public CustomerOrderForEdit CustomerOrderDetailsForEdit(int id);

        public IEnumerable<CustomerOrderDetailsPaymentModel> CustomerOrdersPaymentDetails(string customerName, int currentpage, int customerOrdersPerPage);
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
              List<ProductsForEditSupplierOrder> products);

        public IEnumerable<InvoiceCollectionViewModel> InvoiceCollection(string myCompanyName, int currentpage, int invoicesPerPage);
        public InvoiceDetailsViewModel InvoiceDetails(int id);

        public ICollection<PurchaseInvoicesViewModel> PurchaseInvoices
            (string supplierName, DateTime startDate, DateTime endDate, int invoiceperPage, int currentpage);

    }
}

