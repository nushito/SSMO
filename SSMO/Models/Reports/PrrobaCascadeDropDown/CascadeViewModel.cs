using System.Collections.Generic;

namespace SSMO.Models.Reports.PrrobaCascadeDropDown
{
    public class CascadeViewModel
    {
        public int CustomerId { get; set; }
        public IEnumerable<CustomerListView> Customers { get; set; }
      //  public int SupplierId { get; set; }
        public IEnumerable<ProductListViewDetails> ProductList { get; set; }
    }
}
