﻿using SSMO.Services.Documents.Invoice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Invoice
{
    public class BGInvoicePrintViewModel
    {
        public string DocumentType { get; set; }
        public int DocumentNumber { get; set; }
        public string Number { get; set; }
        public BGMyCompanyInvoicePrintViewModel Seller { get; set; }
        public int CustomerOrderId { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public BGCustomerForInvoicePrint Customer { get; set; }
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
        public decimal Amount { get; set; }
        public int? VAT { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<BGProductsForBGInvoiceViewModel> Products { get; set; }
    }
}