using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class EditInvoicePaymentModel
    {
        public int Id { get; set; }
        public int DocumentNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public DateTime? DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public int CustomerOrderId { get; set; }
        public ICollection<CustomerOrdersNumbersCollection> CustomerOrders { get; set; }
    }
}
