
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; init; }
        public string Description { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        [Required]
        public string Grade { get; set; }
        public IEnumerable<string> Grades { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int CustomerOrderId { get; set; }
     
        
        [Range(0.0, 9999999999999.99999)]
        public int Pieces { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Cubic { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal CostPrice { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Amount { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
    }
}
