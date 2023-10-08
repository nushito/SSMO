using System;

namespace SSMO.Models.Reports.FSC
{
    public class SoldProductsFscCollectionViewModel
    {
        public int DescriptionId { get; set; }
        public string Description { get; set; }     
        public int InvoiceNumber { get; set; }
        public DateTime Date { get; set; }
        public string Transport { get; set; }
        public string CustomerName { get; set; }     
        public string FscClaim { get; set; }        
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string Supplier { get; set; }
        public string PurchaseInvoice { get; set; }
    }
}
