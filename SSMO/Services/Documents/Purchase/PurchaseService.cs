using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Documents.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;

        public PurchaseService(ApplicationDbContext dbContext, IConfigurationProvider mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public bool CreatePurchaseAsPerSupplierOrder(
            string supplierOrderNumber, string number, DateTime date
            , decimal paidAdvance, DateTime datePaidAmount, bool paidStatus,
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses)
        {
            var supplierOrder = dbContext.SupplierOrders.FirstOrDefault(o => o.Number.ToLower() == supplierOrderNumber.ToLower());
            var purchaseId = supplierOrder.Id;
            var amount = supplierOrder.TotalAmount;

            var purchase = new Document
            {
                Number = number,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.Purchase,
                SupplierOrder = supplierOrder,
                SupplierOrderId = purchaseId,
                PaidAvance = paidAdvance,
                DatePaidAmount = datePaidAmount,
                PaidStatus = paidStatus,    
                NetWeight = netWeight,
                GrossWeight = brutoWeight,
                Duty = duty,
                Factoring = factoring,
                CustomsExpenses = customsExpenses,  
                FiscalAgentExpenses = fiscalAgentExpenses,
                ProcentComission = procentComission,
                PurchaseTransportCost = purchaseTransportCost,
                BankExpenses = bankExpenses,
                OtherExpenses = otherExpenses,
                Amount = amount

            };

            var expenses = purchase.Duty + purchase.Factoring +
                       purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                       purchase.ProcentComission + purchase.PurchaseTransportCost + purchase.BankExpenses + purchase.OtherExpenses;

            foreach (var product in supplierOrder.Products)
            {
                product.CostPrice = (product.Amount + (expenses / supplierOrder.TotalQuantity * product.LoadedQuantityM3)) / product.LoadedQuantityM3;
                 

            }

            if(purchase == null)
            {
                return false;
            }



            dbContext.Documents.Add(purchase);
            dbContext.SaveChanges();


            return true;
        }

        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrders(string supplierName
            , int currentpage, int supplierOrdersPerPage)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return new List<PurchaseModelAsPerSpec>();
            }


            var supplierId = dbContext.Suppliers.Where(a => a.Name.ToLower() == supplierName.ToLower())
                .Select(a => a.Id).FirstOrDefault();

            var queryOrders = dbContext.SupplierOrders.
                Where(a => a.SupplierId == supplierId).OrderByDescending(a=>a.Date);

            var totalOrders = queryOrders.Count();

            var orders = queryOrders.ProjectTo<PurchaseModelAsPerSpec>(this.mapper).ToList();

            var supplierOrdersList = orders.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage);

            return supplierOrdersList;
        }

      
    }
}
