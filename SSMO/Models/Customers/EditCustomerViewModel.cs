using System.Collections.Generic;

namespace SSMO.Models.Customers
{
    public class EditCustomerViewModel
    {
        public string CustomerName { get; set; }
        public IEnumerable<string> CustomerNames { get; set; }
        public EditCustomerFormModel CustomerForEdit { get; set; }
    }
}
