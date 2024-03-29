﻿using SSMO.Models.Customers;
using SSMO.Services.Documents.Invoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Models.Documents.Invoice
{
    public class InvoicePrintViewModel
    {
        public string DocumentType { get; set; }
        public int DocumentNumber { get; set; }
        public string Number { get; set; }
        public MyCompanyForInvoicePrint Seller { get; set; }
        public int CustomerOrderId { get; set; }
        public int OrderConfirmationNumber { get; set; }
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
        public string  Swb { get; set; }

        public bool PaidStatus { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
      //  public ServiceOrder ServiceOrder { get; set; }
        public decimal Amount { get; set; }
        public int? VAT { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<ProductsForInvoiceModel> Products { get; set; }
    }
}
