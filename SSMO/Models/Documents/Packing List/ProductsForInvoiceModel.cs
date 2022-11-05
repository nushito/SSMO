using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Packing_List
{
    public class ProductsForPackingListPrint
    {
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public int GradeId { get; set; }
        public string Grade { get; set; }
        public IEnumerable<string> Grades { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int CustomerOrderId { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal OrderedQuantity { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
    }
}
