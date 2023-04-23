namespace SSMO.Models.Reports.Purchase
{
    public class PurchaseProductsForEditFormModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public int TotalSheets { get; set; }
        public string VehicleNumber { get; set; }
    }
}
