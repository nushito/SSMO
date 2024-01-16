using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerInvoicePaymentDetailsModel
    {
        public int Id { get; init; }
        public int DocumentNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalAmount { get; set; }       
        public bool PaidStatus { get; set; }
        public string CustomerName { get; set; }
        public ICollection<PaymentViewModel> Payments { get; set; }
        public ICollection<CustomerOrdersPaymentDetailsPerInvoice> CustomerOrders { get; set; }
    }
}
