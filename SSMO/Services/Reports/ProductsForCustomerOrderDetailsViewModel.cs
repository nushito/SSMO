namespace SSMO.Services.Reports
{
    public class ProductsForCustomerOrderDetailsViewModel
    {
        public int DescriptionId { get; set; }
        public string DescriptionName { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public string Unit { get; set; }
        public decimal OrderedQuantity { get; set; }
        public decimal Price { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public decimal Amount { get; set; }
    }
}
