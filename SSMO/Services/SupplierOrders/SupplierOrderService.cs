using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Services.Documents.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.SupplierOrders
{
    public class SupplierOrderService : ISupplierOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;

        public SupplierOrderService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
            
        }

        public int CreateSupplierOrder(int myCompanyId, int supplierId, DateTime Date, 
            string number, int customerOrderNumber, int statusId, decimal paidAdvance, int currencyId, int vat)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(a => a.OrderConfirmationNumber == customerOrderNumber)
                .FirstOrDefault();

            var thisSupplier = dbContext.Suppliers.Where(a=>a.Id == supplierId).FirstOrDefault();   

            var supplierSpec = new SupplierOrder
            {
                MyCompanyId = myCompanyId,
                SupplierId = supplierId,
                Supplier = thisSupplier,
                Date = Date,
                Number = number,
                CustomerOrderId = customerOrder.Id,
                StatusId = statusId,
                CurrencyId = currencyId,
                PaidAvance = paidAdvance,
                VAT = vat
            };

           
            dbContext.SupplierOrders.Add(supplierSpec);
            dbContext.SaveChanges();

            return supplierSpec.Id;
        }

        

        public IEnumerable<string> GetSuppliers()
        {
            return dbContext.Suppliers.Select(a=>a.Name).ToList();
        }

        public void TotalAmountSum(int supplierOrderId)
        {
           var spOrder = dbContext.SupplierOrders.Find(supplierOrderId);
           spOrder.TotalAmount = spOrder.Amount + (spOrder.Amount * spOrder.VAT / 100)??0;
           spOrder.Balance = spOrder.TotalAmount - spOrder.PaidAvance;
           dbContext.SaveChanges();    
          
        }
    }
}
