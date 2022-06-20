using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PROJECT.Models.Products
{
    public class AddProductsFormModel
    {
        public int Id { get; init; }
        public string ProductDescription { get; set; }
        public string Size { get; set; }
        [Required]
        public string Grade { get; set; }
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
        public string  SupplierName { get; set; }
        public ICollection<string> SupplierNames { get; set; }
    }
}
