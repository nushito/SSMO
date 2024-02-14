using SSMO.Models.Image;
using SSMO.Services.Documents.Invoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Invoice
{
    public class InvoicePrintViewModel
    {
        public string DocumentType { get; set; }
        public int DocumentNumber { get; set; }
        public string Number { get; set; }
        public MyCompanyForInvoicePrint Seller { get; set; }
        public int CustomerOrderId { get; set; }
        public List<int> OrderConfirmationNumbers { get; set; }
        public List<string> CustomerPoNumbers { get; set; }
        public CustomerForInvoicePrint Customer { get; set; }
        public int SupplierOrderId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Delivery Terms")]
        public string Incoterms { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public bool PaidStatus { get; set; }
        public string Currency { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
      //  public ServiceOrder ServiceOrder { get; set; }
        public decimal Amount { get; set; }
        public int? VAT { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Comment { get; set; }
        public string DealTypeEng { get; set; }
        public string DealDescriptionEng { get; set; }
        public string FiscalAgentName { get; set; }
        public string FiscalAgentDetail { get; set; }
        public string FscTextEng { get; set; }
        public string FscCertificate { get; set; }
        public string DeliveryAddress { get; set; }
        public string LoadingAddress { get; set; }
        public string PaymentTerms { get; set; }
        public string HeaderUrl { get; set; }
        public string FooterUrl { get; set; }        
        public ICollection<InvoiceBankDetailsViewModel> CompanyBankDetails { get; set; }
        public IEnumerable<ProductsForInvoiceModel> Products { get; set; }
        public IEnumerable<ServiceProductForInvoiceFormModel> ServiceProducts { get; set; }
        public string PlaceOfIssue { get;  set; }
    }
}
