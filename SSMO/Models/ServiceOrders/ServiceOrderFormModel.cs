using SSMO.Models.TransportCompany;
using SSMO.Services.Curruncies;
using SSMO.Services.Suppliers;
using System;
using System.Collections.Generic;

namespace SSMO.Models.ServiceOrders
{
    public class ServiceOrderFormModel
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int TransportCompanyId { get; set; }
        public ICollection<TransportCompanyListViewModel> TransportCompany { get; set; }       
        public int CurrencyId { get; set; }
        public ICollection<GetCurrencyModel> Currencies { get; set; }
        public decimal Cost { get; set; }
        public int Vat { get; set; }
        public bool Paid { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string TruckNumber { get; set; }
        public decimal AmountAfterVat { get; set; }
        public int MyCompanyId { get; set; }
        public ICollection<MyCompaniesForTrasnportOrderViewModel> MyCompanies { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public ICollection<AllSuppliers> Suppliers { get; set; }
        public int InvoiceDocumentNumberId { get; set; }
        public int InvoiceNumber { get; set; }
        public int PurchaseInvoiceId { get; set; }
        public string PurchaseNumber { get; set; }
        public IEnumerable<int> InvoiceDocumentNumbers { get; set; }
        //public IEnumerable<string> PurchaseNumbers { get; set; }
    
    }
}
