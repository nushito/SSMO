using System.Collections.Generic;

namespace SSMO.Models.ServiceOrders
{
    public class ServiceOrderFormModel
    {
        public string TransportCompany { get; set; }
        public decimal Cost { get; set; }
        public int Vat { get; set; }
        public bool Paid { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string TruckNumber { get; set; }
        public decimal AmountAfterVat { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public IEnumerable<string> SupplierNames { get; set; }
        public int InvoiceDocumentNumberId { get; set; }
        public int InvoiceDocumentNumber { get; set; }
        public string PurchaseNumber { get; set; }
        public IEnumerable<int> InvoiceDocumentNumbers { get; set; }
        public IEnumerable<string> PurchaseNumbers { get; set; }
    
    }
}
