using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderReportAll
    {
        [BindProperty(Name = "customerName", SupportsGet = true)]
        [FromBody]
        public string CustomerName { get; set; }
        public IEnumerable<string> CustomerNames { get; set; }
        public IEnumerable<CustomerOrderViewModel> CustomerOrderCollection { get; set; }
    }
}
