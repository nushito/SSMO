using System.Collections.Generic;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoicesViewModel
    {
        public const int InvoicesPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalInvoices { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<string> MyCompanyNames { get; set; }
        public IEnumerable<InvoiceCollectionViewModel> InvoiceCollection { get; set; }
    }
}
