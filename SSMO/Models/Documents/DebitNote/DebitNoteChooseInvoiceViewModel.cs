using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Reports;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Documents.DebitNote
{
    public class DebitNoteChooseInvoiceViewModel
    {
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public ICollection<MyCompaniesForReportViewModel> MyCompanies { get; set; }
        public DateTime Date { get; set; }
        public int InvoiceId { get; set; }
        public ICollection<int> InvoiceNumbers { get; set; }
        public bool MoreQuantity { get; set; }
        public ICollection<AddProductsToCreditAndDebitNoteFormModel> Products { get; set; }
    }
}
