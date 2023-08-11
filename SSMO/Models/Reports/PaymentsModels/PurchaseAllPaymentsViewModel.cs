using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class PurchaseAllPaymentsViewModel
    {
        public decimal PaidAmount { get; set; }
        public DateTime Date { get; set; }
        public int DocumentId { get; set; }
        
    }
}
