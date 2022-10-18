using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrdersPaymentsReportsViewModel
    {
        public const int CustomerOrdersPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalCustomerOrders { get; set; }
        public string CustomerName { get; set; }
        public IEnumerable<string> CustomerNames { get; set; }
        public IEnumerable<CustomerOrderPaymentDetailsModel> CustomerPaymentCollection { get; set; }
    }
}
