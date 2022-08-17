using Microsoft.AspNetCore.Mvc;
using SSMO.Services.Reports;
using System.Collections.Generic;

namespace SSMO.Models.Reports
{
    public class CustomerOrderReportAll
    {
        //   [BindProperty(Name = "customerName", SupportsGet = true)]
        public const int CustomerOrdersPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalCustomerOrders { get; set; }
        public string CustomerName { get; set; }

        public IEnumerable<string> CustomerNames { get; set; }
        public IEnumerable<CustomerOrderDetailsModel> CustomerOrderCollection { get; set; }
    }
}
