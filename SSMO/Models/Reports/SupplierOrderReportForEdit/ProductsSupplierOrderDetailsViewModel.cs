namespace SSMO.Models.Reports.SupplierOrderReportForEdit
{
    public class ProductsSupplierOrderDetailsViewModel
    {
        public int DescriptionId { get; set; }
        public string DescriptionName { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string PurchaseFscClaim { get; set; }
        public string PurchaseFscCertificate { get; set; }
        public string Unit { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal LoadedQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal PurchaseAmount { get; set; }
        public string HsCode { get; set; }
    }
}
