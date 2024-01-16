using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class EditSupplierOrderPaymentModel
    {
        public string Currency { get; set; }
        public string ConvertToThatCurrency { get; set; }
        public string Number { get; set; } 
        public List<PurchaseNewpaymentsPerOrderFormModel> PurchasePaymentsCollection { get; set; }
        public decimal? NewPaidAmount { get; set; }
        public DateTime? NewDateOfPayment { get; set; }

        public decimal? CurrencyExchangeRate { get; set; }
        public string ActionCalc { get; set; }
       
    }
}
