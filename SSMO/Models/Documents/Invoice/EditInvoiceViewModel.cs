﻿
using SSMO.Models.Reports.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Invoice
{
    public class EditInvoiceViewModel
    {
        public string DocumentType { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public IEnumerable<int> OrderConfirmationNumbers { get; set; }
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
        public List<EditProductForCompanyInvoicesViewModel> Products { get; set; }
        public int? CustomerId { get; set; }      
        public ICollection<CustomerCollectionForChoosingNewOrderForInvoiceEditViewModel> Customers { get; set; }
        public List<int> SelectedCustomerOrders { get; set; }      
       
    }
}
