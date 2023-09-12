using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Models.Descriptions;
using SSMO.Models.Grades;
using SSMO.Models.Sizes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Invoice
{
    public class ProductsForInvoiceViewModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Document Invoice { get; set; }
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int ProductId { get; set; }       
        public Unit Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        public decimal Amount { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal BgAmount { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal SellPrice { get; set; }
        public int PurchaseCostPriceId { get; set; }
        public List<PurchaseProductCostPriceViewModel> PurchaseCostPrice { get; set; }
        public string VehicleNumber { get; set; }
        public decimal InvoicedQuantity { get; set; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public List<DescriptionForInvoiceViewModel> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public List<SizeForInvoiceViewModel> Sizes { get; set; }
        public int GradeId { get; set; }   
        public string Grade { get; set; }
        public List<GradeForInvoiceViewModel> Grades { get; set; }

    }
}
