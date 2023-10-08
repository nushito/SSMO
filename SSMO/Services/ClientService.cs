using ImageMagick;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.FSC;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.ProductsStock;
using SSMO.Services.Documents.Invoice;
using System.Collections.Generic;

namespace SSMO.Services
{
    public class ClientService
    {
        private static InvoiceDetailsViewModel _clientModel;
        private static IEnumerable<ProductAvailabilityDetailsViewModel> _productOnStock;
        private static ProductsFscCollectionViewModel _fscReport;
        private static SupplierOrdersPaymentReportViewModel _payments;
        private static BgInvoiceViewModel _bgInvoice;
        private static CustomerInvoicePaymentsReportsViewModel _customerInvoicePayments;
        private static CustomerOrderPrintViewModel _customerOrderPrint;
        public static void AddClient(InvoiceDetailsViewModel clientModel)
        {
            _clientModel = clientModel;
        }
        public static InvoiceDetailsViewModel GetClient()
        {
            return _clientModel;
        }

        public static void AddProductsOnStock(IEnumerable<ProductAvailabilityDetailsViewModel> productOnStockModel)
        {
            _productOnStock = productOnStockModel;
        }

        public static IEnumerable<ProductAvailabilityDetailsViewModel> GetProductOnStock()
        {
            return _productOnStock;
        }

        public static void AddFscReport( ProductsFscCollectionViewModel fscReport)
        {
            _fscReport = fscReport;
        }

        public static ProductsFscCollectionViewModel GetFscReport()
        {
            return _fscReport;  
        }

        public static void AddPurchasePayments(SupplierOrdersPaymentReportViewModel payments)
        {
            _payments= payments;
        }

        public static SupplierOrdersPaymentReportViewModel GetPurchasePayments()
        {
            return _payments;
        }

        public static void AddBgInvoice(BgInvoiceViewModel bgInvoice)
        {
            _bgInvoice = bgInvoice;
        }

        public static BgInvoiceViewModel GetBgInvoice()
        {
            return _bgInvoice;
        }

        public static void AddCustomerInvoicePayments(CustomerInvoicePaymentsReportsViewModel customerInvoice)
        {
            _customerInvoicePayments = customerInvoice;
        }

        public static CustomerInvoicePaymentsReportsViewModel GetCustomerInvoicePayments()
        {
            return _customerInvoicePayments;
        }

        public static void AddCustomerOrderPrint(CustomerOrderPrintViewModel customerOrderPrint)
        {
            _customerOrderPrint = customerOrderPrint;
        }
        public static CustomerOrderPrintViewModel GetCustomerOrderPrint()
        {
            return _customerOrderPrint;
        }

    }
}
