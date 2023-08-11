using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierPaymentDetailsViewModel
    {
        public decimal PaidAmount { get; set; }
        public DateTime Date { get; set; }
        public int SupplierOrderId { get; set; }        
    }
}
