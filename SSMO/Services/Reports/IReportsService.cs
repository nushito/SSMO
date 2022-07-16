using SSMO.Models.CustomerOrders;
using System.Collections.Generic;

namespace SSMO.Services.Reports
{
    public interface IReportsService
    {
        IEnumerable<CustomerOrderViewModel> AllCustomerOrders(string name);
    }
}
