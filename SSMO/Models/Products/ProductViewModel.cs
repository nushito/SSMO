
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
     
        [Required]
        public int Pieces { get; set; }
        public decimal Cubic { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TransportCost { get; set; }
        public decimal TerminalCharges { get; set; }
        public decimal Duty { get; set; }
        public decimal CustomsExpenses { get; set; }
        public decimal BankExpenses { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SoldPrice { get; set; }
        public decimal Income { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal Amount { get; set; }
    }
}
