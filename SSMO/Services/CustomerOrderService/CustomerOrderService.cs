using AutoMapper;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using System;
using System.Linq;

namespace SSMO.Services.CustomerOrderService
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public CustomerOrderService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
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
                CustomerId = customerId,
                MyCompanyId = company,
                DeliveryTerms = deliveryTerms,
                LoadingPlace = loadingAddress,
                DeliveryAddress = deliveryAddress,
                FSCClaim = fscClaim,
                FSCSertificate = fscCertificate,
                CurrencyId = currency,
                Status = status,
               
            };


            dbContext.CustomerOrders.Add(order);
            dbContext.SaveChanges();
            return order.Id;

        }

        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id)
        {
            return dbContext.CustomerOrders.Where(a => a.Id == id).FirstOrDefault();
        }
    }
}
