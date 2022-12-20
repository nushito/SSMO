using SSMO.Data.Enums;
using SSMO.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Data.Models
{
    public class Document
    {
        public int Id { get; init; }      
        [Required]
        public DocumentTypes DocumentType { get; set; }
        [DisplayName("Invoice Number")]
        public int DocumentNumber { get; set; }
        [DisplayName("Purchase Invoice Number")]
        public string Number { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int CustomerOrderId { get; set; }
        [Required]
        public CustomerOrder CustomerOrder { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public int MyCompanyId { get; set; }
        public MyCompany MyCompany { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Incoterms { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public decimal PurchaseTransportCost { get; set; }
        public decimal DeliveryTrasnportCost { get; set; }
        public decimal BankExpenses { get; set; }
        public decimal Duty { get; set; }
        public decimal CustomsExpenses { get; set; }
        public decimal Factoring { get; set; }
        public decimal FiscalAgentExpenses { get; set; }
        public decimal ProcentComission { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
        public ServiceOrder ServiceOrder { get; set; }
        public int? Vat { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public  ICollection<Product> Products { get; set; }
    }
}
