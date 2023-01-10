using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceDetailsViewModel
    {
        public int DocumentNumber { get; set; }

        public int MyCompanyId { get; set; }
        public MyCompanyInvoiceDetailsModel Seller { get; set; }
        public int CustomerOrderId { get; set; }
        public int OrderConfirmationNumber { get; set; }
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
        //  public ServiceOrder ServiceOrder { get; set; }
        public decimal Amount { get; set; }
        public int? VAT { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<InvoiceBankDetailsModel> CompanyBankDetails { get; set; }
        public IEnumerable<InvoiceProductsDetailsViewModel> Products { get; set; }
    }
}
