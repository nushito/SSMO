
using SSMO.Data.Enums;
using SSMO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
    public class SupplierOrder
    {
        public SupplierOrder()
        {
            Products = new List<Product>();
        }
        public int Id { get; init; }
        public string Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [Required]
        public Supplier Supplier { get; set; }
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int MyCompanyId { get; set; }
        public MyCompany MyCompany { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public string DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public decimal TotalAmount { get; set; }

        public int? VAT { get; set; }

        public ICollection<Product> Products { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
