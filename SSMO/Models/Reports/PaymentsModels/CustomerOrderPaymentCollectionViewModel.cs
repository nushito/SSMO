using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class CustomerOrderPaymentCollectionViewModel
    {
        public int TotalCustomerOrders { get; set; }
        public IEnumerable<CustomerOrderDetailsPaymentModel> CustomerPaymentCollection { get; set; }
    }
}
