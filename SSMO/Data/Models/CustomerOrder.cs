
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
    public class CustomerOrder
    {
        
        public int Id { get; init; }
        [Required]
        public string Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString ="{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        public string DeliveryAddress { get; set; }
        [Required]
        public string DeliveryTerms { get; set; }
        public int CustomerId { get; set; }
        public int MyCompanyId { get; set; }
        public MyCompany MyCompany { get; set; }
        public Customer Customer { get; set; }
        [Required]
        public string FSCClaim { get; set; }
        [Required]
        public string FSCSertificate { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Vat { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public bool PaidAmountStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }

        public string Origin { get; set; }
        public IEnumerable<SupplierOrder> SupplierOrder { get; set; }
    }
}
