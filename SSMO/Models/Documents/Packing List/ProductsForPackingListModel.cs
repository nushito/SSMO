using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Packing_List
{
    public class ProductsForPackingListModel
    {
        public int DescriptionId { get; set; }
        public string DescriptionName { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public string Unit { get; set; }
        public int CustomerOrderId { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal InvoicedQuantity { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int ProductId { get; set; }
        public string VehicleNumber { get; set; }
    }
}
