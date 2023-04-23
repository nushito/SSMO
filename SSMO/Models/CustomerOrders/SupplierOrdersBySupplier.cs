using SSMO.Services.Suppliers;
using System.Collections.Generic;

namespace SSMO.Models.CustomerOrders
{
    public class SupplierOrdersBySupplier
    {
        public int SupplierId { get; set; }
        public int SupplierOrderId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierOrderNumber { get; set; }
        public string SupplierAndOrderSelect { get; set; }
    }
}
