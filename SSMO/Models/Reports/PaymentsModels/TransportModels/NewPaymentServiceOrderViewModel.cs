using System;

namespace SSMO.Models.Reports.PaymentsModels.TransportModels
{
    public class NewPaymentServiceOrderViewModel
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }        
    }
}
