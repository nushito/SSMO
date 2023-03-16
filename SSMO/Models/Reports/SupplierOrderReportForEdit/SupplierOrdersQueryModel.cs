using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.SupplierOrderReportForEdit
{
    public class SupplierOrdersQueryModel
    {
        public IEnumerable<SupplierOrderDetailsModel> SupplierOrders { get; set; }
        public int TotalSupplierOrders { get; set; }
    }
}
