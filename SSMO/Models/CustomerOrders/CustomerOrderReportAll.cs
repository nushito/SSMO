using System.Collections.Generic;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderReportAll
    {
        public string Search { get; set; }
        public IEnumerable<string> Searches { get; set; }
        public ICollection<CustomerOrderReport> CustomerOrderCollection { get; set; }
    }
}
