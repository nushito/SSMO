using SSMO.Data;
using SSMO.Models.CustomerOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Reports
{
    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;

        public ReportsService(ApplicationDbContext context)
        {
            _context = context;

        }
        public IEnumerable<CustomerOrderViewModel> AllCustomerOrders(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
               return new List<CustomerOrderViewModel>();
            }
            var customerId = _context.Customers.Where(a => a.Name.ToLower() == name.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var listOrders = _context.CustomerOrders
                    .Where(a => a.ClientId == customerId)
                    .ToList();

            return (ICollection<CustomerOrderViewModel>)listOrders;
        }

        
    }
}
