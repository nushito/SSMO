using System.Collections.Generic;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceReportModel
    {
        public int CurrentPage { get; set; }
        public int InvoicesPerPage { get; set; }
        public IEnumerable<InvoiceCollectionViewModel> InvoiceCollection { get; set; }
        public int TotalInvoices { get; set; }
    }
}
