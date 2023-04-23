using SSMO.Models.Customers;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports;
using SSMO.Services.Documents.Invoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Models.Documents.CreditNote
{
    public class ChooseInvoiceForCreditNoteViewModel
    {
        public int MyCompanyId { get; set; }
        public ICollection<MyCompaniesForReportViewModel> MyCompanies { get; set; }
        public DateTime Date { get; set; }
        public int InvoiceId { get; set; }
        public ICollection<int> InvoiceNumbers { get; set; }
        public string CreditNoteDeliveryAddress { get; set; }
        public bool QuantityBack { get; set; }
        public ICollection<AddProductsToCreditAndDebitNoteFormModel> Products { get; set; }
    }
}
