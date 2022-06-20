using SSMO.Data;
using SSMO.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext dbContext;
        public CustomerService(ApplicationDbContext dbContex)
        {
            this.dbContext = dbContex;
        }

        public ICollection<AddCustomerFormModel> AllCustomers()
        {

            return (ICollection<AddCustomerFormModel>)dbContext.MyCompanies.ToList();
        }

        public IEnumerable<string> GetCustomers()
        {
            return dbContext.Clients.Select(a => a.Name).ToList();
        }

        //public IEnumerable<int> GetInvoices(string name = null)
        //{
        //    return dbContext.Documents
        //        .Where(a => a.CustomerOrder.Number == name)
        //        .ToList();

        //}

    }
}
