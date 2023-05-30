using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Models.Descriptions;
using SSMO.Models.Grades;
using SSMO.Models.Sizes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Invoice
{
    public class ServiceProductForInvoiceFormModel
    {
        public Unit Unit { get; set; }
        public decimal Amount { get; set; }
        public decimal BgPrice { get; set; }
        public decimal BgAmount { get; set; }
        public decimal Profit { get; set; }
        public decimal SellPrice { get; set; }
        public string VehicleNumber { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public List<DescriptionForInvoiceViewModel> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public List<SizeForInvoiceViewModel> Sizes { get; set; }
        public int GradeId { get; set; }
        [Required]
        public string Grade { get; set; }
        public List<GradeForInvoiceViewModel> Grades { get; set; }
    }
}
