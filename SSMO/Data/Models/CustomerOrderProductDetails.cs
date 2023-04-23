using SSMO.Data.Enums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SSMO.Data.Models
{
    public class CustomerOrderProductDetails
    {
        public int Id { get; init; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public decimal Quantity { get; set; }

        [DisplayName("Autstanding Quantity For Invoice")]
        public decimal AutstandingQuantity { get; set; }
        public Unit Unit { get; set; }
        public int Pallets { get; set; }
        public int SheetsPerPallet { get; set; }
        public int TotalSheets { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Amount { get; set; }
        public ICollection<InvoiceProductDetails> InvoiceProductDetails { get; set; }
        public ICollection<Document> CustomerInvoices { get; set; }
    }
}
