
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Products
{
    public class ProductViewModel
    {
        public int Id { get; init; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Descriptions { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public int GradeId { get; set; }
        [Required]
        public string Grade { get; set; }
        public IEnumerable<string> Grades { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int CustomerOrderId { get; set; }
     
        
        [Range(0.0, 9999999999999.99999)]
        public int Pieces { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal OrderedQuantity { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Price { get; set; }


        [Range(0.0, 9999999999999.99999)]
        public decimal PurchaseAmount { get; set; }
        public decimal PurchasePrice { get; set; }

        public decimal CostPrice { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal Amount { get; set; }

        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }

        public decimal? PurchaseTransportCost { get; set; }
        public decimal? DeliveryTrasnportCost { get; set; }
        public decimal? BankExpenses { get; set; }
        public decimal? Duty { get; set; }
        public decimal? CustomsExpenses { get; set; }
        public decimal? Factoring { get; set; }
        public decimal? FiscalAgentExpenses { get; set; }
        public decimal? ProcentComission { get; set; }
        public decimal? OtherExpenses { get; set; }




    }
}
