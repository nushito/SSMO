using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class EditSupplierOrderPaymentModel
    {
        public decimal PaidAvance { get; set; }
        public DateTime DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
    }
}
