using SSMO.Models.Customers;
using System.Collections.Generic;

namespace SSMO.Services.Customer
{
    public interface ICustomerService
    {
        public IEnumerable<string> GetCustomerNames();

        public IEnumerable<AddCustomerFormModel> CustomersData();
        public AddCustomerFormModel GetCustomer(int id);   
        //  public IEnumerable<int> GetInvoices(string name);
    }
}
