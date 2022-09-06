using SSMO.Data;
using SSMO.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SSMO.Models.Reports.PrrobaCascadeDropDown;

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

        public IEnumerable<AddCustomerFormModel> CustomersData()
        {
            var listCustomers = dbContext.Customers.ToList();

            var customers = mapper.Map<IEnumerable<AddCustomerFormModel>>(listCustomers);
            return customers;
        }

        public AddCustomerFormModel GetCustomer(int id)
        {
            var customer = dbContext.Customers.Where(a => a.Id == id).FirstOrDefault();
            var getCustomer = mapper.Map<AddCustomerFormModel>(customer);
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
