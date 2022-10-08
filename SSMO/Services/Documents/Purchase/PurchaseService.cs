﻿using AutoMapper;
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
            string supplierOrderNumber, string number, DateTime date, bool paidStatus,
            decimal netWeight, decimal brutoWeight,
            decimal duty, decimal factoring, decimal customsExpenses, decimal fiscalAgentExpenses,
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses)
        {
            var supplierOrder = dbContext.SupplierOrders.FirstOrDefault(o => o.Number.ToLower() == supplierOrderNumber.ToLower());
            var amount = supplierOrder.TotalAmount;
            var customerOrder = dbContext.CustomerOrders.Where(sp => sp.Id == supplierOrder.CustomerOrderId)
                                 .FirstOrDefault();

            var purchase = new Document
            {
                Number = number,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.Purchase,
                SupplierOrder = supplierOrder,
                SupplierOrderId = supplierOrder.Id,
                CustomerOrderId = supplierOrder.CustomerOrderId,
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
                Amount = amount,
                Incoterms = customerOrder.DeliveryTerms,
                Products = new List<Product>()

            };

            var expenses = purchase.Duty + purchase.Factoring*amount/100 +
                       purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                       purchase.ProcentComission*amount/100 + purchase.PurchaseTransportCost + purchase.BankExpenses + purchase.OtherExpenses;

            var productList = dbContext.Products.Where(s=>s.SupplierOrderId == supplierOrder.Id).ToList();

            foreach (var product in productList)
            {
               product.CostPrice = (product.Amount + (expenses / supplierOrder.TotalQuantity * product.LoadedQuantityM3)) / product.LoadedQuantityM3;
               purchase.Products.Add(product);                                
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
