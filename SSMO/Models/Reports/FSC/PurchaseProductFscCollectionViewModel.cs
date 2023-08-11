using System;

namespace SSMO.Models.Reports.FSC
{
    public class PurchaseProductFscCollectionViewModel
    {
        public int DescriptionId { get; set; }
        public string Description { get; set; }  
        public string SupplierName { get; set; }
        public string PurchaseInvoice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Transport { get; set; }
        public string FscClaim { get; set; }    
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
}
