using AutoMapper;
using SSMO.Data;
using SSMO.Data.Models;
using System;
using System.Linq;

namespace SSMO.Services.SupplierOrders
{
    public class SupplierOrderService : ISupplierOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public SupplierOrderService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public int CreateSupplierOrder(int myCompanyId, int supplierId, DateTime Date, 
            string number, string customerOrderNumber, int statusId, int currencyId, int vat)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(a => a.Number.ToLower() == customerOrderNumber)
                .FirstOrDefault();

            var supplierSpec = new SupplierOrder
            {
                MyCompanyId = myCompanyId,
                SupplierId = supplierId,
                Date = Date,
                Number = number,
                CustomerOrderId = customerOrder.Id,
                StatusId = statusId,
                CurrencyId = currencyId,
                VAT = vat
            };

            dbContext.SupplierOrders.Add(supplierSpec);
            dbContext.SaveChanges();

            return supplierSpec.Id;
        }

        public void TotalAmountSum(int supplierOrderId)
        {
           var spOrder = dbContext.SupplierOrders.Find(supplierOrderId);
           spOrder.TotalAmount = spOrder.Amount + (spOrder.Amount * spOrder.VAT / 100)??0;
            dbContext.SaveChanges();    
          
        }
    }
}
