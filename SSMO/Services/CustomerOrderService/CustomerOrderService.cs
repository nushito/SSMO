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

        public bool CheckOrderNumberExist(string number)
        {
            if(dbContext.CustomerOrders.Any(o => o.Number.ToLower() == number.ToLower()))
            {
                return true;
            }

            return false;
        }




        public int CreateOrder(string num, DateTime date, int customerId, int company, string deliveryTerms,
            string loadingAddress, string deliveryAddress,int currency,string origin, 
            bool paidStatus, decimal paidAdvance, int vat)
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
               Origin = origin,
               PaidAmountStatus = paidStatus,   
               PaidAvance  = paidAdvance, 
               Vat = vat
            };

           
            dbContext.CustomerOrders.Add(order);
            dbContext.SaveChanges();
            return order.Id;

        }

        public void CustomerOrderCounting(int customerorderId)
        {
            var thisorder = OrderPerIndex(customerorderId);

            thisorder.TotalAmount = (decimal)(thisorder.Amount + thisorder.Vat);

            if(thisorder.PaidAmountStatus == false)
            {
                thisorder.Balance = thisorder.TotalAmount - thisorder.PaidAvance;
            }
            else
            {
                thisorder.Balance = 0;
            }

            dbContext.SaveChanges();

        }

        //public bool EditProductAsPerSupplierSpec(int productId, int descriptionId, int sizeId, 
        //    int gradeId, string fscClaim, string fscCertificate,
        //    int cusomerOrderId, decimal quantity, 
        //    decimal purchasePrice, int pallets, int sheetsPerPallet)
        //{
        //    var customerOrder = dbContext.CustomerOrders.Find(cusomerOrderId);

        //    if(customerOrder == null)
        //    {
        //        return false;
        //    }


        //    var productList = customerOrder.Products;


        //    return true;
        //}


        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id)
        {
            return dbContext.CustomerOrders.Where(a => a.Id == id).FirstOrDefault();
        }

        public Data.Models.CustomerOrder OrderPerNumber(string number)
        {
            return dbContext.CustomerOrders.Where(a => a.Number.ToLower() == number.ToLower()).FirstOrDefault();
        }





    }
}
