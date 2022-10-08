using System.Collections.Generic;

namespace SSMO.Models.Reports.PrrobaCascadeDropDown
{
    public class CustomerBySupplierOrdersViewModel
    {
        public int CustomerId { get; set; }
        public IEnumerable<CustomerListView> Customers { get; set; }
        public IEnumerable<CustomerOrderListViewBySupplier> ProductList { get; set; }
    }
}
