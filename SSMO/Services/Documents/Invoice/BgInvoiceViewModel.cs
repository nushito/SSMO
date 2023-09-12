using SSMO.Models.Documents.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Invoice
{
    public class BgInvoiceViewModel
    {
        public int DocumentNumber { get; set; }
        public int CreditToInvoiceNumber { get; set; }
        public DateTime CreditToInvoiceDate { get; set; }
        public int DebitToInvoiceNumber { get; set; }
        public DateTime DebitToInvoiceDate { get; set; }        
        public DateTime Date { get; set; }
        public string DocumentType { get; set; }
        public decimal Amount { get; set; }
        public int? Vat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string DealTypeBg { get; set; }
        public string DealDescriptionBg { get; set; }
        public BGMyCompanyInvoicePrintViewModel BgMyCompany { get; set; }
        public BGCustomerForInvoicePrint BgCustomer { get; set; }
        public ICollection<BGProductsForBGInvoiceViewModel> BgProducts { get; set; }
        public ICollection<InvoiceBankDetailsViewModel> CompanyBankDetails { get; set; }
    }
}
