using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrderPaymentReportViewModel
    {
        public const int CustomerOrderPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalCustomerOrders { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public string CustomerName { get; set; }
        public IEnumerable<string> CustomerNames { get; set; }
        public IEnumerable<CustomerOrderDetailsPaymentModel> CustomerOrdersPaymentCollection { get; set; }
    }
}
