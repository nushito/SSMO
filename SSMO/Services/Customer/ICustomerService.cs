using SSMO.Models.Customers;
using SSMO.Models.Reports.CustomerOrderCascadeDropDown;
using SSMO.Models.ServiceOrders;
using System.Collections.Generic;

namespace SSMO.Services.Customer
{
    public interface ICustomerService
    {
        public IEnumerable<string> GetCustomerNames(string userId);

        public IEnumerable<CustomerListView> GetCustomerNamesAndId(string userId);

        public IEnumerable<AddCustomerFormModel> CustomersData();

        //darpa customers pri izdavane na transport order
        public IEnumerable<CustomersListForServiceOrderViewModel> CustomersListForService(string userId);
        public AddCustomerFormModel GetCustomer(int id);           
        public EditCustomerFormModel GetCustomerForEdit
            (string customerName);
        public bool EditCustomer(string customerName,string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string phoneNumber, 
            string bgName, string bgStreet, string bgCity, string bgCountry, string bgRepresentativePerson);
        public bool CreateCustomer
            (string customerName, string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string phoneNumber, string bgName,
            string bgStreet, string bgCity, string bgCountry, string bgRepresentativePerson,string userId);
    }
}
