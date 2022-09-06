using AutoMapper;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using System;
using System.Collections.Generic;
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

        public bool CheckOrderNumberExist(int number)
        {
            if(dbContext.CustomerOrders.Any(o => o.OrderConfirmationNumber == number))
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

            var lastConfirmationNumber = dbContext.CustomerOrders.OrderBy(a=>a.OrderConfirmationNumber).Select(oc=>oc.OrderConfirmationNumber).LastOrDefault();


            var order = new SSMO.Data.Models.CustomerOrder
            {
                OrderConfirmationNumber = lastConfirmationNumber+1,
                CustomerPoNumber = num,
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

            thisorder.TotalQuantity = thisorder.Products.Sum(a=>a.OrderedQuantity);

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

        public Data.Models.CustomerOrder OrderPerNumber(int number)
        {
            return dbContext.CustomerOrders.Where(a => a.OrderConfirmationNumber == number).FirstOrDefault();
        }


        public bool AnyCustomerOrderExist()
        {
            var anyCustomerOrders = dbContext.CustomerOrders.Any();
            if (!anyCustomerOrders)
            {
                return false;
            }

            return true;
        }

        public int CreateFirstOrder(int number, string num, DateTime date, 
            int customerId, int company, string deliveryTerms, string loadingAddress, 
            string deliveryAddress, int currency, string origin, bool paidStatus, 
            decimal paidAdvance, int vat)
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
                OrderConfirmationNumber = number,
                CustomerPoNumber = num,
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
                PaidAvance = paidAdvance,
                Vat = vat
            };


            dbContext.CustomerOrders.Add(order);
            dbContext.SaveChanges();
            return order.Id;
        }

        public ICollection<int> AllCustomerOrderNumbers()
        {
            return dbContext.CustomerOrders.Select(a => a.OrderConfirmationNumber).ToList();
        }
    }
}
