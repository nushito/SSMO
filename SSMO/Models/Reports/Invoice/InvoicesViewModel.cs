using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoicesViewModel
    {
        public const int InvoicesPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int? TotalInvoices { get; set; }
        public int MyCompanyId { get; init; }
        public IEnumerable<MyCompaniesForReportViewModel> MyCompanyNames { get; set; }
        public IEnumerable<InvoiceCollectionViewModel> InvoiceCollection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
