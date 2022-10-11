using SSMO.Services.Supplier;
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
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal Duty { get; set; }
        public decimal CustomsExpenses { get; set; }
        public decimal Factoring { get; set; }
        public decimal FiscalAgentExpenses { get; set; }
        public string TruckNumber { get; set; }

        public decimal ProcentComission { get; set; }
        public decimal OtherExpenses { get; set; }

        public decimal PurchaseTransportCost { get; set; }
        public decimal BankExpenses { get; set; }
       

    }
}
