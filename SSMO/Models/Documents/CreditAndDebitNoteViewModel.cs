using DocumentFormat.OpenXml.Wordprocessing;
using SSMO.Models.Documents.Invoice;
using SSMO.Services.Documents.Invoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents
{
    public class CreditAndDebitNoteViewModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int ClientId { get; set; }
        [Display(Name = "Customer")]
        public CustomerForInvoicePrint Client { get; set; }
        public int SellerId { get; set; }
        public MyCompanyForInvoicePrint Seller { get; set; }
        public int InvoiceNumber { get; set; }
        public ICollection<int> InvoiceNumbers { get; set; }
        public DateTime InvoiceDate { get; set; }
        public ICollection<InvoiceBankDetailsViewModel> CompanyBankDetails { get; set; }
        public ICollection<AddProductsToCreditAndDebitNoteFormModel> Products { get; set; }
        public int CurrencyId { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Total { get; set; }
        public int VatPercent { get; set; }
        public decimal Amount { get; set; }
        public bool QuantityBack { get; set; }
        public string CreditNoteDeliveryAddress { get; set; }
        public ICollection<InvoiceBankDetailsViewModel> BankDetails { get; set; }
        public string PaymentTerms { get; set; }
    }
}
