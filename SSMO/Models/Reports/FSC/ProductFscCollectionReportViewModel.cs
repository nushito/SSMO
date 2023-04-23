using System;

namespace SSMO.Models.Reports.FSC
{
    public class ProductFscCollectionReportViewModel
    {
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string PurchaseInvoice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierFscClaim { get; set; }
        public string SupplierFscCertificate { get; set; }
        public decimal Quantity { get; set; }
    }
}
