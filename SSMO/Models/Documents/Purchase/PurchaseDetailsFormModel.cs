using SSMO.Services.Curruncies;
using SSMO.Services.Suppliers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Purchase
{
    public class PurchaseDetailsFormModel
    {
        public string Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string SupplierOrderNumber { get; set; }
        public int SupplierOrderId { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public string SupplierFSCCertificate { get; set; }
        public string Origin { get; set; }
        public decimal Amount { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal Duty { get; set; }
        public decimal CustomsExpenses { get; set; }
        public decimal Factoring { get; set; }
        public decimal FiscalAgentExpenses { get; set; }
        public string TruckNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string LoadingAddress { get; set; }
        public string Swb { get; set; }
        public string ShippingLine { get; set; }
        public string Eta { get; set; }
        public int Vat { get; set; }
        public decimal ProcentComission { get; set; }
        public decimal OtherExpenses { get; set; }
        public decimal PurchaseTransportCost { get; set; }
        public decimal BankExpenses { get; set; }
        public string Incoterms { get; set; }
        public string Currency { get; set; }
        public ICollection<GetCurrencyModel> Currencies { get; set; }
        public int CostPriceCurrency { get; set; }
        public bool DelayCostCalculation { get; set; }
        public List<PurchaseProductAsSupplierOrderViewModel> ProductDetails { get; set; }

    }
}

