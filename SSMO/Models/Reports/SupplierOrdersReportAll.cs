using SSMO.Models.Reports.SupplierOrderReportForEdit;
using System.Collections.Generic;

namespace SSMO.Models.Reports
{
    public class SupplierOrdersReportAll
    {
        public const int SupplierOrdersPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalSupplierOrders { get; set; }
        public string SupplierName { get; set; }
        public IEnumerable<string> SupplierNames { get; set; }
        public IEnumerable<SupplierOrderDetailsModel> SupplierOrderCollection { get; set; }
    }
}
