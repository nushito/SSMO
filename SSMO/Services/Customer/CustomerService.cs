using SSMO.Data;
using SSMO.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Data.Models;

namespace SSMO.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public CustomerService(ApplicationDbContext dbContex, IMapper mapper)
        {
            this.dbContext = dbContex;
            this.mapper = mapper;
        }

        public bool CreateCustomer
            (string customerName, string vat, string eik, string representativePerson, 
            string country, string city, string street, string email, string phoneNumber, 
            string bgName, string bgStreet, string bgCity, string bgCountry, string bgRepresentativePerson)
        {
            if (customerName == null) return false;

            var customer = new Data.Models.Customer
            {
                ClientAddress = new Address
                {
                    Country = country,
                    City = city,
                    Street = street,
                    BgCity = bgCity,
                    BgStreet = bgStreet,
                    Bgcountry = bgCountry
                },
                Email = email,
                Name = customerName,
                VAT = vat,
                EIK = eik,
                RepresentativePerson =representativePerson,
                BgCustomerName = bgName,
                BgCustomerRepresentativePerson=bgRepresentativePerson,
                PhoneNumber = phoneNumber
            };

            this.dbContext.Customers.Add(customer);
            this.dbContext.SaveChanges();

            return true;
        }

        public IEnumerable<AddCustomerFormModel> CustomersData()
        {
            var listCustomers = dbContext.Customers.ToList();

            var customers = mapper.Map<IEnumerable<AddCustomerFormModel>>(listCustomers);
            return customers;
        }

        public bool EditCustomer
            (string customerName, string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string phoneNumber,
            string bgName, string bgStreet, string bgCity, string bgCountry, string bgRepresentativePerson)
        {
            var customer = dbContext.Customers
                .Where(a => a.Name.ToLower() == customerName.ToLower())
                .FirstOrDefault();
            if (customer == null) return false;

            customer.Name = customerName;
            customer.VAT = vat;
            customer.EIK = eik;
            customer.RepresentativePerson = representativePerson;
            customer.BgCustomerName = bgName;
            customer.BgCustomerRepresentativePerson = bgRepresentativePerson;

            var address = dbContext.Addresses
                .Where(c => c.Id == customer.AddressId)
                .FirstOrDefault();

            address.Country = country;
            address.City = city;
            address.Street = street;
            address.BgCity = bgCity;
            address.Bgcountry = bgCountry;
            address.BgStreet = bgStreet;

            customer.Email = email;
            customer.PhoneNumber = phoneNumber;

            dbContext.SaveChanges();
            return true;
        }

        public AddCustomerFormModel GetCustomer(int id)
        {
            var customer = dbContext.Customers.Where(a => a.Id == id).FirstOrDefault();
            var getCustomer = mapper.Map<AddCustomerFormModel>(customer);
            return getCustomer;
        }

        public EditCustomerFormModel GetCustomerForEdit(string customerName)
        {
            if (String.IsNullOrEmpty(customerName))
            {
                return null;
            }
            var customer = dbContext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower()).FirstOrDefault();
            if (customer == null)
            {
                return null;
            }
            var address = dbContext.Addresses.Where(a => a.Id == customer.AddressId).FirstOrDefault();
            var addressForEdit = mapper.Map<CustomerForEditAddressFormModel>(address);
            var getCustomer = mapper.Map<EditCustomerFormModel>(customer);
            getCustomer.CustomerAddress = new CustomerForEditAddressFormModel
            {
                City = addressForEdit.City,
                Country = addressForEdit.Country,
                Street = addressForEdit.Street
            };

            return getCustomer;
        }

        public IEnumerable<string> GetCustomerNames()
        {
            return dbContext.Customers.Select(a => a.Name).ToList();
        }

        public IEnumerable<CustomerListView> GetCustomerNamesAndId()
        {
            var customers = dbContext.Customers.Select(a => new CustomerListView
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            return customers; ;
        }



        //public IEnumerable<int> GetInvoices(string name = null)
        //{
        //    return dbContext.Documents
        //        .Where(a => a.CustomerOrder.Number == name)
        //        .ToList();

        //}

    }
}
