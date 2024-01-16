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
        public decimal? CurrencyExchangeRate { get; set; }
        public string ActionCalc { get; set; }
        public bool IsChecked { get; set; } 
        public bool OnlyCalculate { get; set; }

    }
}
