using SSMO.Models.Reports.PrrobaCascadeDropDown;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Invoice
{
    public class CustomerOrdersForInvoice
    {
        public int MyCompanyId { get; set; }
        public ICollection<MyCompanyViewModel> MyCompanies { get; set; }
        public int CustomerId { get; set; }
        public IEnumerable<CustomerListView> Customers { get; set; }
        public List<int> SelectedCustomerOrders { get; set; }

    }
}
