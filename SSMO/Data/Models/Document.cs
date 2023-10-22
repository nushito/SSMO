using SSMO.Data.Enums;
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
        public Document()
        {
            CustomerOrders = new List<CustomerOrder>();
            PurchaseProducts = new List<PurchaseProductDetails>();
            CustomerOrderProducts = new List<CustomerOrderProductDetails> ();
            InvoiceProducts = new List<InvoiceProductDetails>();
            CreditAndDebitNoteProducts = new List<Product>();
            DebitNoteProducts = new List<InvoiceProductDetails>();
            CreditNoteProducts = new List<InvoiceProductDetails>();
        }

        public int Id { get; init; }      
        [Required]
        public DocumentTypes DocumentType { get; set; }
        [DisplayName("Invoice Number")]
        public int DocumentNumber { get; set; }
        [DisplayName("Purchase Invoice Number")]
        public string PurchaseNumber { get; set; }
        public int CreditToInvoiceId { get; set; }
        public DateTime CreditToInvoiceDate { get; set; }
        public int DebitToInvoiceId { get; set; }
        public DateTime DebitToInvoiceDate { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int? SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public int MyCompanyId { get; set; }
        public MyCompany MyCompany { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string Incoterms { get; set; }
        public string CreditNoteDeliveryAddress { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public string ShippingLine { get; set; }
        public string Eta { get; set; }
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
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int? CostPriceCurrencyId { get; set; }
        public Currency CostPriceCurrency { get; set; }
        public decimal Balance { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
        public ServiceOrder ServiceOrder { get; set; }
        public int? Vat { get; set; }
        public decimal Amount { get; set; }
        public decimal CreditNoteTotalAmount { get; set; }
        public decimal DebitNoteTotalAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalQuantity { get; set; }
        public string Comment { get; set; }
        public string DeliveryAddress { get; set; }
        public string DealTypeEng { get; set; }
        public string DealDescriptionEng { get; set; }
        public string DealTypeBg { get; set; }
        public string DealDescriptionBg { get; set; }
        public int? FiscalAgentId { get; set; }
        public FiscalAgent Fiscalagent { get; set; }
        public int? FscTextId { get; set; }
        public FscText FscText { get; set; }
        public ICollection<BankDetails> BankDetails { get; set; }        
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
        public ICollection<PurchaseProductDetails> PurchaseProducts { get; set; }
        public ICollection<CustomerOrderProductDetails> CustomerOrderProducts { get; set; }
        public ICollection<InvoiceProductDetails> InvoiceProducts { get; set; }
        public ICollection<Product> CreditAndDebitNoteProducts { get; set; }
        public ICollection<InvoiceProductDetails> DebitNoteProducts { get; set; }
        public ICollection<InvoiceProductDetails> CreditNoteProducts { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<ServiceOrder> ServiceOrders { get; set; }
    }
}
