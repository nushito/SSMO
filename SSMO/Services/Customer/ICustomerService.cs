using SSMO.Models.Customers;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using System.Collections.Generic;

namespace SSMO.Services.Customer
{
    public interface ICustomerService
    {
        public IEnumerable<string> GetCustomerNames();

        public IEnumerable<CustomerListView> GetCustomerNamesAndId();

        public IEnumerable<AddCustomerFormModel> CustomersData();
        public AddCustomerFormModel GetCustomer(int id);   
        //  public IEnumerable<int> GetInvoices(string name);
    }
}
