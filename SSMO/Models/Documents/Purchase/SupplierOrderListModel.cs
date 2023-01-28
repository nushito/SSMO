using SSMO.Services.Documents.Purchase;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Purchase
{
    public class SupplierOrderListModel
    {
        public const int SupplierOrdersPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int TotalSupplierOrders { get; set; }
        public string SupplierName { get; set; }
        public int SupplierOrderId { get; set; }
        public ICollection<string> SupplierNames { get; set; }
        public IEnumerable<PurchaseModelAsPerSpec> SupplierOrderNumbers { get; set; }
        
    }
}
