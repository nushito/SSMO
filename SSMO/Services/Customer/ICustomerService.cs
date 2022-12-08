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
        public EditCustomerFormModel GetCustomerForEdit
            (string customerName);
        public bool EditCustomer(string customerName,string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string phoneNumber, 
            string bgName, string bgStreet, string bgCity, string bgCountry, string bgRepresentativePerson);
        public bool CreateCustomer(string customerName, string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string phoneNumber, string bgName,
            string bgStreet, string bgCity, string bgCountry, string bgRepresentativePerson);
    }
}
