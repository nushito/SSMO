using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class PurchaseNewpaymentsPerOrderFormModel
    {
        public int Id { get; set; }        
        public string PurchaseNumber { get; set; }       
        public decimal NewPaidAmount { get; set; }
        public DateTime? NewDatePaidAmount { get; set; }
    }
}
