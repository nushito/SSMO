using SSMO.Models.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SSMO.Models.Documents
{
    public interface IDocument
    {
        public int Id { get; set; }
        public int Number { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int SellerId { get; set; }
        public SupplierModel Seller { get; set; }

        public int ClientId { get; set; }
        public AddCustomerFormModel Client { get; set; }
        public ICollection<string> Customers { get; set; }
       // public ICollection<AddProductsFormModel> Products { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int VatPercent { get; set; }
    }
}
