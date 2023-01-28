﻿namespace SSMO.Models.Reports.Purchase
{
    public class PurchaseProductsDetailsViewModel
    {
        public string Description { get; set; }
        public string Grade { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public int Pallet { get; set; }
        public int SheetsPerPallet { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}