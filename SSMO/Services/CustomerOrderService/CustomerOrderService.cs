using SSMO.Data;
using SSMO.Data.Models;
using System;
using System.Linq;

namespace SSMO.Services.CustomerOrderService
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ApplicationDbContext dbContext;
        public CustomerOrderService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public int CreateOrder(string num, DateTime date, int customerId, int company, string deliveryTerms,
            string loadingAddress, string deliveryAddress,int currency)
        {
           
            var fscClaim = dbContext.MyCompanies
                .Where(a => a.Id == company)
                .Select(a => a.FSCClaim).FirstOrDefault();

            var fscCertificate = dbContext.MyCompanies
               .Where(a => a.Id == company)
               .Select(a => a.FSCSertificate).FirstOrDefault();

            var status = dbContext.Statuses.Where(a => a.Name == "Active").FirstOrDefault();


            var order = new SSMO.Data.Models.CustomerOrder
            {
                Number = num,
                Date = date,
                ClientId = customerId,
                MyCompanyId = company,
                DeliveryTerms = deliveryTerms,
                LoadingPlace = loadingAddress,
                DeliveryAddress = deliveryAddress,
                FSCClaim = fscClaim,
                FSCSertificate = fscCertificate,
                CurrencyId = currency,
                Status = status   
            };


            dbContext.CustomerOrders.Add(order);
            dbContext.SaveChanges();
            return order.Id;

        }
    }
}
