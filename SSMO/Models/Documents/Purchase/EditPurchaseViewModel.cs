using SSMO.Models.Reports.Purchase;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Purchase
{
    public class EditPurchaseViewModel
    {
        public string PurchaseNumber { get; set; }
        public DateTime Date { get; set; }
        public int SupplierOrderId { get; set; }
        public ICollection<SupplierOrdersListForPurchaseEditModel> SupplierOrders { get; set; }
        public int Vat { get; set; }
        public string Incoterms { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public decimal PurchaseTransportCost { get; set; }      
        public decimal BankExpenses { get; set; }
        public decimal Duty { get; set; }
        public decimal CustomsExpenses { get; set; }
        public decimal Factoring { get; set; }
        public decimal FiscalAgentExpenses { get; set; }
        public decimal ProcentComission { get; set; }
        public decimal OtherExpenses { get; set; }
        public List<PurchaseProductsForEditFormModel> PurchaseProducts { get; set; }
    }
}
