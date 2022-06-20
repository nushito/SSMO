using SSMO.Models.Customers;
using System.Collections.Generic;

namespace SSMO.Services.Customer
{
    public interface ICustomerService
    {
        public IEnumerable<string> GetCustomers();
        public ICollection<AddCustomerFormModel> AllCustomers();
      //  public IEnumerable<int> GetInvoices(string name);
    }
}
