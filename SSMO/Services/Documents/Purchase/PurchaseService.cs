﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Reports.PaymentsModels;
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
            decimal procentComission, decimal purchaseTransportCost, decimal bankExpenses, decimal otherExpenses, 
            int vat, string truckNumber, string fscCertificate, string fscClaim)
        {
            var supplierOrder = dbContext.SupplierOrders.FirstOrDefault(o => o.Number.ToLower() == supplierOrderNumber.ToLower());
            var amount = supplierOrder.Amount;
            //var totalAmount = supplierOrder.TotalAmount;
            var customerOrder = dbContext.CustomerOrders.Where(sp => sp.Id == supplierOrder.CustomerOrderId)
                                 .FirstOrDefault();
            customerOrder.NetWeight = netWeight;
            customerOrder.GrossWeight = brutoWeight;
            supplierOrder.NetWeight = netWeight;
            supplierOrder.GrossWeight = brutoWeight;

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
                Products = new List<Product>(),
                Vat = vat,
                TruckNumber = truckNumber,
                SupplierId = supplierOrder.SupplierId,
                MyCompanyId = supplierOrder.MyCompanyId,
                FSCClaim = fscClaim,
                FSCSertificate = fscCertificate
            };

            purchase.TotalAmount = purchase.Amount + purchase.Amount * vat / 100;
            var expenses = purchase.Duty + purchase.Factoring * amount / 100 +
                       purchase.CustomsExpenses + purchase.FiscalAgentExpenses +
                       purchase.ProcentComission * amount / 100 + purchase.PurchaseTransportCost + purchase.BankExpenses + purchase.OtherExpenses;

            var productList = dbContext.Products.Where(s => s.SupplierOrderId == supplierOrder.Id).ToList();

            foreach (var product in productList)
            {
                product.LoadedQuantityM3 = product.OrderedQuantity;
                product.CostPrice = (product.Amount + (expenses / supplierOrder.TotalQuantity * product.LoadedQuantityM3)) / product.LoadedQuantityM3;
                purchase.Products.Add(product);
            }

            if (purchase == null)
            {
                return false;
            }

            if (purchase.PaidStatus == true)
            {
                supplierOrder.PaidStatus = true;
                supplierOrder.Balance = 0;
                purchase.Balance = 0;
                purchase.PaidAvance = purchase.Amount;
                supplierOrder.PaidAvance = purchase.Amount;
            }

            dbContext.Documents.Add(purchase);
            dbContext.SaveChanges();

            return true;
        }

        public bool EditPurchasePayment
            (string number, bool paidStatus, decimal paidAvance, DateTime datePaidAmount)
        {
            if (number == null)
            {
                return false;
            }

            var purchase = dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Purchase && type.Number.ToLower() == number.ToLower())
                .FirstOrDefault(); 
           
            purchase.PaidStatus = paidStatus;
            purchase.PaidAvance = paidAvance;
            purchase.DatePaidAmount = datePaidAmount;
            purchase.Balance = purchase.TotalAmount - purchase.PaidAvance;
            if (purchase.Balance == 0)
            {
                purchase.PaidStatus = true;
            }
            else
            {
                purchase.PaidStatus = false;
            }

            return true;
        }

        public EditPurchasePaymentDetails GetPurchaseForPaymentEdit(string number)
        {
            var purchase = dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Purchase && type.Number.ToLower() == number.ToLower());


            var purchaseForEdit = purchase.ProjectTo<EditPurchasePaymentDetails>(mapper).FirstOrDefault();

            return purchaseForEdit;
        }

        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrdersForPurchase(string supplierName
            , int currentpage, int supplierOrdersPerPage)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return new List<PurchaseModelAsPerSpec>();
            }


            var supplierId = dbContext.Suppliers.Where(a => a.Name.ToLower() == supplierName.ToLower())
                .Select(a => a.Id).FirstOrDefault();

            var queryOrders = dbContext.SupplierOrders.
                Where(a => a.SupplierId == supplierId).OrderByDescending(a => a.Date);

            var totalOrders = queryOrders.Count();

            var orders = queryOrders.ProjectTo<PurchaseModelAsPerSpec>(this.mapper).ToList();

            var supplierOrdersList = orders.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage);

            return supplierOrdersList;
        }


    }
}
