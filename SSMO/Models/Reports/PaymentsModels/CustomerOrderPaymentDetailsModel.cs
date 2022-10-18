using System;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrderPaymentDetailsModel
    {
        public int Id { get; init; }
        public int DocumentNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAdvance { get; set; }
        public decimal Balance { get; set; }
        public DateTime DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public string CustomerName { get; set; }
    }
}
