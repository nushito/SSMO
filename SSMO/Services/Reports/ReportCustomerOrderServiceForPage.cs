using System.Collections.Generic;

namespace SSMO.Services.Reports
{
    public class ReportCustomerOrderServiceForPage
    {
        public int CurrentPage { get; set; }
        public int CustomerOrdersPerPage { get; set; }
        public int TotalCustomerOrders { get; set; }
        public IEnumerable<CustomerOrderDetailsModel> CustomerOrderCollection { get; set; }
    }
}
