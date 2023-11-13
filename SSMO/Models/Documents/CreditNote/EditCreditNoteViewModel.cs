using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.CreditNote;
using SSMO.Models.Reports.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Documents.CreditNote
{
    public class EditCreditNoteViewModel
    {
        public string DocumentType { get; set; }
        public int InvoiceNumberId { get; set; }
        public IEnumerable<InvoiceNumbersForEditedCreditNoteViewModel> InvoiceNumbers { get; set; }
        public int CreditToInvoiceNumber { get; set; }       
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<string> Currencies { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string TruckNumber { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public string Incoterms { get; set; }
        public string Comment { get; set; }
        public IList<EditProductForCreditNoteViewModel> Products { get; set; }
        public string PaymentTerms { get; set; }
    }
}
