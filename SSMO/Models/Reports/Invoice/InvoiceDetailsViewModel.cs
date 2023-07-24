using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceDetailsViewModel
    {
        public int DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public int MyCompanyId { get; set; }
        public MyCompanyInvoiceDetailsModel Seller { get; set; }
        public ICollection<CustomerOrderForInvoiceViewModel> CustomerOrders { get; set; }
        public int CustomerOrderId { get; set; }
        public ICollection<int> OrderConfirmationNumber { get; set; }
        public ICollection<string> CustomerPoNumbers { get; set; }
        public int CustomerId { get; set; }
        public InvoiceCustomerDetailsModel Customer { get; set; }
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
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
        public string Comment { get; set; }
        //  public ServiceOrder ServiceOrder { get; set; }
        public decimal Amount { get; set; }
        public int? VAT { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int CreditToInvoiceId { get; set; }       
        public decimal CreditNoteTotalAmount { get; set; }
        public DateTime CreditToInvoiceDate { get; set; }
        public int DebitToInvoiceId { get; set; }       
        public decimal DebitNoteTotalAmount { get; set; }
        public DateTime DebitToInvoiceDate { get; set; }
        public ICollection<InvoiceBankDetailsModel> CompanyBankDetails { get; set; }
        public IEnumerable<InvoiceProductsDetailsViewModel> Products { get; set; }
    }
}
