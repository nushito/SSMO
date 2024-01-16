using SSMO.Models.Documents.Invoice;
using SSMO.Models.Image;
using SSMO.Models.Reports.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Documents.DebitNote
{
    public class EditDebitNoteViewModel
    {
        public string DocumentType { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public IEnumerable<int> OrderConfirmationNumbers { get; set; }
        public int? Vat { get; set; }
        public int InvoiceNumber { get; set; }
        public int DebitToInvoiceNumberId { get; set; }
        public ICollection<InvoiceNumbersForEditedDebitNoteViewModel> DebitNoteInvoicenumbers { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<string> Currencies { get; set; }
      //  public decimal GrossWeight { get; set; }
     //   public decimal NetWeight { get; set; }
     //   public string TruckNumber { get; set; }
       // public decimal DeliveryCost { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public string Incoterms { get; set; }
        public string Comment { get; set; }
        public List<EditProductForDebitNoteViewModel> Products { get; set; }
        public string PaymentTerms { get; set; }
        public string HeaderUrl { get; set; }
        public string FooterUrl { get; set; }        
    }
}
