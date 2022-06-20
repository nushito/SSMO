

using SSMO.Data.Enums;
using SSMO.Enums;
//using SSMO.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
    public class SupplierOrder
    {
        public int Id { get; init; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public Supplier Supplier { get; set; }
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public StatusEnum Status { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public string DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }

        public ICollection<Product> Products { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
