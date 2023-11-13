
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
    public class CustomerOrder
    {
        public int Id { get; init; }
        [Required]
        public int OrderConfirmationNumber { get; set; }
        public string CustomerPoNumber { get; set; }
        public string Type { get; set; }
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
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int? FscTextId { get; set; }
        public FscText FscText { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public string Comment { get; set; }
        public ICollection<CustomerOrderProductDetails> CustomerOrderProducts { get; set; }        
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalSheets { get; set; }
        public int TotalPallets { get; set; }
        public int? Vat { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public bool PaidAmountStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string Origin { get; set; }
        public int? FiscalAgentId { get; set; }
        public FiscalAgent Fiscalagent { get; set; }
        public string DealType { get; set; }
        public string DealDescription { get; set; }
        public ICollection<Document> Documents { get; set;}
        public IEnumerable<SupplierOrder> SupplierOrders { get; set; }
        public ICollection<InvoiceProductDetails> InvoiceProductDetails { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<BankDetails> BankDetails { get; set; }
        public ICollection<ServiceOrder> ServiceOrders { get; set; }
        public string DeliveryPeriod{ get; set; }
        public string PaymentTerms { get; set; }
        public string Etd { get; set; }
        public string Eta { get; set; }
    }
}
