using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IConfigurationProvider mapper;

        public ReportsService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper.ConfigurationProvider;

        }
        public IEnumerable<CustomerOrderViewModel> AllCustomerOrders(string customerName)
        {
            if (String.IsNullOrEmpty(customerName))
            {
               return new List<CustomerOrderViewModel>();
            }

            var customerId = _context.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var listOrders = _context.CustomerOrders
                    .Where(a => a.CustomerId == customerId);
                    

            var orders = listOrders.ProjectTo<CustomerOrderViewModel>(this.mapper).ToList();

            return orders;
        }

        public CustomerOrderViewModel Details(int id)
        {
            var findorder = _context.CustomerOrders.Where(a => a.Id == id);
            var order = findorder.ProjectTo<CustomerOrderViewModel>(mapper).FirstOrDefault();
            return (CustomerOrderViewModel)order;

        }

        public bool Edit(int id, 
            string number, System.DateTime date, int clientId, 
            int myCompanyId, string deliveryTerms,
            string loadingPlace, string deliveryAddress, 
            int currencyId, string status, string fscClaim, string fscCertificate)
        {
            var order = _context.CustomerOrders.Find(id);

            if(order == null)
            {
                return false;
            }

            order.Number = number;
            order.CustomerId = clientId;
            order.MyCompanyId = myCompanyId;
            order.DeliveryTerms = deliveryTerms;    
            order.LoadingPlace = loadingPlace;
            order.DeliveryAddress = deliveryAddress;
            order.CurrencyId = currencyId;
            order.Status.Name = status;
            order.FSCClaim = fscClaim;
            order.FSCSertificate = fscCertificate;

            _context.SaveChanges();

            return true;
        }
    }
}
