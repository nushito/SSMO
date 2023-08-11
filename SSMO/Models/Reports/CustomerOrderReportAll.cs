using Microsoft.AspNetCore.Mvc;
using SSMO.Services.Reports;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports
{
    public class CustomerOrderReportAll
    {
        public const int CustomerOrdersPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalCustomerOrders { get; set; }
        public string CustomerName { get; init; }
        public IEnumerable<string> CustomerNames { get; set; }
        public IEnumerable<CustomerOrderDetailsModel> CustomerOrderCollection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    }
