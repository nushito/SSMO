using SSMO.Services.Reports;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.CustomerOrderReportForEdit
{
    public class CustomerOrdersQueryModel
    {
        public IEnumerable<CustomerOrderDetailsModel> CustomerOrders { get; set; }
        public int TotalCustomerOrders { get; set; }
    }
}
