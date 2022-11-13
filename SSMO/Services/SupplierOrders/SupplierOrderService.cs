using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using SSMO.Models.Reports.PaymentsModels;
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
            string number, int customerOrderNumber, int statusId, int currencyId, string fscClaim, int vat,
            DateTime datePaidAmount, decimal paidAvance, bool paidStatus,string loadingAddress, string deliveryAddress)
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
                VAT = vat,
                FSCClaim = fscClaim,
                Products = new List<Product>(),
                PaidAvance = paidAvance,               
                PaidStatus = paidStatus,
                LoadingAddress = loadingAddress,
                DeliveryAddress = deliveryAddress
            };

           if(datePaidAmount.ToString() != null)
            {
                supplierSpec.DatePaidAmount = datePaidAmount.ToString();
            }

            dbContext.SupplierOrders.Add(supplierSpec);
            dbContext.SaveChanges();

            return supplierSpec.Id;
        }

        public bool EditSupplierOrderPayment
            (string supplierOrderNumber, decimal paidAdvance, DateTime date, bool paidStatus)
        {
            if (supplierOrderNumber == null) return false;

            var supplierOrder = dbContext.SupplierOrders
                .Where(num => num.Number.ToLower() == supplierOrderNumber.ToLower())
                .FirstOrDefault();
            if(supplierOrder.Balance < paidAdvance)
            {
                return false;
            }

            supplierOrder.PaidAvance = paidAdvance;
            supplierOrder.PaidStatus = paidStatus;
            supplierOrder.DatePaidAmount = date.ToString();
            supplierOrder.Balance = supplierOrder.TotalAmount - supplierOrder.PaidAvance;

           if(supplierOrder.Balance > 0)
            {
                supplierOrder.PaidStatus = false;
            }
            else
            {
                supplierOrder.PaidStatus = true;
            }

            return true;
        }

        public EditSupplierOrderPaymentModel GetSupplierOrderForEdit(string supplierOrderNumber)
        {
            if (supplierOrderNumber == null) return null;

           
            var supplierOrder = dbContext.SupplierOrders
                .Where(num=>num.Number.ToLower() == supplierOrderNumber.ToLower());

            var supplierOrderForEdit = supplierOrder.ProjectTo<EditSupplierOrderPaymentModel>(mapper).FirstOrDefault();
            return supplierOrderForEdit;
        }

        public IEnumerable<SupplierOrdersPaymentDetailsModel> GetSupplierOrders(string supplierName)
        {
            if(supplierName == null)
            {
                return null;
            }

            var supplierId = dbContext.Suppliers
                .Where(name => name.Name.ToLower() == supplierName.ToLower())
                .Select(i => i.Id)
                .FirstOrDefault();

            var supplierOrders = dbContext.SupplierOrders
                .Where(sup=>sup.SupplierId == supplierId);

            var supplierOrdersCollection = supplierOrders.ProjectTo<SupplierOrdersPaymentDetailsModel>(mapper).ToList();
            return supplierOrdersCollection;    
        }

        public IEnumerable<string> GetSuppliers()
        {
            return dbContext.Suppliers.Select(a=>a.Name).ToList();
        }

        public void TotalAmountAndQuantitySum(int supplierOrderId)
        {
           var spOrder = dbContext.SupplierOrders.Find(supplierOrderId);
           spOrder.TotalAmount = spOrder.Amount + (spOrder.Amount * spOrder.VAT / 100)??0;
           spOrder.Balance = spOrder.TotalAmount - spOrder.PaidAvance;
            spOrder.TotalQuantity = spOrder.Products.Sum(a => a.LoadedQuantityM3);
           dbContext.SaveChanges();    
          
        }
    }
}
