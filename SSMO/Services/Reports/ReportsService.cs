using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
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
        public IEnumerable<CustomerOrderDetailsModel> AllCustomerOrders(string customerName,
            int currentpage, int customerOrdersPerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
               return new List<CustomerOrderDetailsModel>();
            }

            var customerId = _context.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryOrders = _context.CustomerOrders
                    .Where(a => a.CustomerId == customerId);
                    
           // var totalOrders = queryOrders.Count();  

            var orders = queryOrders.ProjectTo<CustomerOrderDetailsModel>(this.mapper).ToList();

            var customerOrdersList = orders.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            return customerOrdersList;
        }

        public CustomerOrderDetailsModel Details(int id)
        {
            var findorder = _context.CustomerOrders.Where(a => a.Id == id);
            var order = findorder.ProjectTo<CustomerOrderDetailsModel>(mapper).FirstOrDefault();
            return (CustomerOrderDetailsModel)order;

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

            order.CustomerPoNumber = number;
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
