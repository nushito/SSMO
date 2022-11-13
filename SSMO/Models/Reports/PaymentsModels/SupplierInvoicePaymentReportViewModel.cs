using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierInvoicePaymentReportViewModel
    {
        public const int SupplierInvoicePerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalSupplierInvoices { get; set; }
        public string SupplierName { get; set; }
        public IEnumerable<string> SupplierNames { get; set; }
        public IEnumerable<SupplierInvoicePaymentDetailsModel> SupplierInvoicesPaymentCollection { get; set; }
    }
}
