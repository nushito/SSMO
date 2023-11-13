using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.ServiceOrders
{
    public class ServiceOrdersQueryModel
    {
        public int TotalServiceOrders { get; set; }
        public IEnumerable<ServiceOrderCollectionDetailViewModel> ServiceOrders { get; set; }
    }
}
