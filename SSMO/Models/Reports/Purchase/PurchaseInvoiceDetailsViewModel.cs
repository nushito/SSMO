using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Purchase
{
    public class PurchaseInvoiceDetailsViewModel
    {
        public string PurchaseNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Incoterms { get; set; }
        public string SupplierOrderNumber { get; set; }      
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal Duty { get; set; }
        public decimal CustomsExpenses { get; set; }
        public decimal Factoring { get; set; }
        public decimal FiscalAgentExpenses { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public string DeliveryAddress { get; set; }
        public int Vat { get; set; }
        public decimal ProcentComission { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal PurchaseTransportCost { get; set; }
        public decimal BankExpenses { get; set; }
        public int SupplierOrderId { get; set; }
        public ICollection<PurchaseProductsDetailsViewModel> Products { get; set; }

    }
}
