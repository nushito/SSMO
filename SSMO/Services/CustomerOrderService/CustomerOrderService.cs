using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using SSMO.Models.Reports.PaymentsModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.CustomerOrderService
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        public CustomerOrderService(ApplicationDbContext dbContext, IConfigurationProvider mapper)
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
            bool paidStatus, int vat, int statusId  )
        {
           
            var fscClaim = dbContext.MyCompanies
                .Where(a => a.Id == company)
                .Select(a => a.FSCClaim).FirstOrDefault();

            var fscCertificate = dbContext.MyCompanies
               .Where(a => a.Id == company)
               .Select(a => a.FSCSertificate).FirstOrDefault();

            var lastConfirmationNumber = dbContext.CustomerOrders.OrderBy(a=>a.OrderConfirmationNumber).Select(oc=>oc.OrderConfirmationNumber).LastOrDefault();

            var order = new SSMO.Data.Models.CustomerOrder
            {
                OrderConfirmationNumber = lastConfirmationNumber + 1,
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
                StatusId = statusId,
                Origin = origin,
                PaidAmountStatus = paidStatus,
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
           int vat, int statusId)
        {
            var fscClaim = dbContext.MyCompanies
                 .Where(a => a.Id == company)
                 .Select(a => a.FSCClaim).FirstOrDefault();

            var fscCertificate = dbContext.MyCompanies
               .Where(a => a.Id == company)
               .Select(a => a.FSCSertificate).FirstOrDefault();

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
                StatusId = statusId,
                Origin = origin,
                PaidAmountStatus = paidStatus,
                Vat = vat
            };


            dbContext.CustomerOrders.Add(order);
            dbContext.SaveChanges();
            return order.Id;
        }

        public ICollection<int> AllCustomerOrderNumbers()
        {
            return dbContext.CustomerOrders
                .OrderByDescending(n=>n.OrderConfirmationNumber)
                .Select(a => a.OrderConfirmationNumber)
                .ToList();
        }

        public EditCustomerOrderPaymentModel GetCustomerOrderPaymentForEdit(int orderConfirmationNumber)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(num => num.OrderConfirmationNumber == orderConfirmationNumber);

            var customerOrderforEdit = customerOrder.ProjectTo<EditCustomerOrderPaymentModel>(mapper).FirstOrDefault();

            return customerOrderforEdit;
        }

        public bool EditCustomerOrdersPayment(int orderConfirmationNumber, bool paidStatus, decimal paidAdvance)
        {
            if (orderConfirmationNumber == 0)
            {
                return false;
            }

            var customerOrder = dbContext.CustomerOrders
                .Where(num => num.OrderConfirmationNumber == orderConfirmationNumber)
                .FirstOrDefault();
           
            customerOrder.PaidAmountStatus = paidStatus;
            customerOrder.PaidAvance = paidAdvance;
            customerOrder.Balance = customerOrder.TotalAmount - customerOrder.PaidAvance;
            if (customerOrder.Balance == 0)
            {
                customerOrder.PaidAmountStatus = true;
            }
            else
            {
                customerOrder.PaidAmountStatus = false;
            }

            return true;
        }

        public int CustomerOrderNumber(int supplierOrderId)
        {
            var customerOrderId = dbContext.SupplierOrders
                .Where(id => id.Id == supplierOrderId)
                .Select(cnum => cnum.CustomerOrderId)
                .FirstOrDefault();

            var customerOrdeNum = dbContext.CustomerOrders
                .Where(id => id.Id == customerOrderId)
                .Select(num => num.OrderConfirmationNumber)
                .FirstOrDefault();

            return customerOrdeNum;
        }

        public int CustomerOrderNumberById(int id)
        {
           return dbContext.CustomerOrders
                .Where(i=>i.Id == id)
                .Select(n=>n.OrderConfirmationNumber)
                .FirstOrDefault();
        }
    }
}
