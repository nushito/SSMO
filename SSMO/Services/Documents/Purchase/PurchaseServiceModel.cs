using System.Collections.Generic;

namespace SSMO.Services.Documents.Purchase
{
    public class PurchaseServiceModel
    {
        public int CurrentPage { get; set; }
        public int CustomerOrdersPerPage { get; set; }
        public int TotalCustomerOrders { get; set; }
        public ICollection<PurchaseModelAsPerSpec> SupplierOrders { get; set; }
    }
}
