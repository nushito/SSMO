using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.Products;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.Documents.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.SupplierOrders
{
    public class SupplierOrderService : ISupplierOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        private readonly ISupplierService supplierService;
        public SupplierOrderService(ApplicationDbContext dbContext, IMapper mapper, ISupplierService supplierService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
            this.supplierService = supplierService; 
        }

        public int CreateSupplierOrder(int myCompanyId, int supplierId, DateTime Date, 
            string number, int statusId, int currencyId, string fscClaim, int vat,
            DateTime datePaidAmount, decimal paidAvance, bool paidStatus,
            string loadingAddress, string deliveryAddress, string deliveryTerms)
        {
            
            var thisSupplier = dbContext.Suppliers.Where(a=>a.Id == supplierId).FirstOrDefault();   

            var supplierSpec = new SupplierOrder
            {
                MyCompanyId = myCompanyId,
                SupplierId = supplierId,
                Supplier = thisSupplier,
                Date = Date,
                Number = number,                
                StatusId = statusId,
                CurrencyId = currencyId,
                VAT = vat,
                FscClaim = fscClaim,
                Products = new List<Product>(),
                PaidAvance = paidAvance,               
                PaidStatus = paidStatus,
                LoadingAddress = loadingAddress,
                DeliveryAddress = deliveryAddress,
                DeliveryTerms = deliveryTerms
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

        public SupplierOrderPaymentCollectionModel GetSupplierOrders
            (string supplierName, int currentpage, int supplierOrdersPerPage)
        {
            if(String.IsNullOrEmpty(supplierName))
            {
                return new SupplierOrderPaymentCollectionModel();
            }

            var supplierId = dbContext.Suppliers
                .Where(name => name.Name.ToLower() == supplierName.ToLower())
                .Select(i => i.Id)
                .FirstOrDefault();

            var supplierOrders = dbContext.SupplierOrders
                .Where(sup=>sup.SupplierId == supplierId);

            var supplierOrdersCollection = supplierOrders.ProjectTo<SupplierOrdersPaymentDetailsModel>(mapper).ToList();

            var supplierOrderPaymentCollection = new SupplierOrderPaymentCollectionModel
            {
                TotalSupplierOrders = supplierOrders.Count(),
                SupplierOrderPaymentCollection = supplierOrdersCollection.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage)
            };

            return supplierOrderPaymentCollection;    
        }

        public ICollection<SupplierOrdersListForPurchaseEditModel> GetSupplierOrdersNumbers()
        {
            return dbContext.SupplierOrders
                .Select(n=>new SupplierOrdersListForPurchaseEditModel
                {
                    Id = n.Id,
                    Number = n.Number
                })
                .ToList();
        }

        public ICollection<SupplierOrdersNumbersListViewModel> GetSupplierOrdersNumbersJsonList(int id)
        {
           return dbContext.SupplierOrders
                .Where(s=>s.SupplierId == id)
                .Select(l=>new SupplierOrdersNumbersListViewModel   
                {
                    SupplierOrderId = l.Id,  
                    SupplierOrderNumber = l.Number
                })
                .ToList();
        }

        public ICollection<SupplierOrdersBySupplier> SuppliersAndOrders()
        {
            var supplierOrders = dbContext.SupplierOrders
                .Where(a=> a.Products.Any(i=>i.SupplierOrderId == a.Id && i.QuantityAvailableForCustomerOrder > 0m))
                .Select(a=> new SupplierOrdersBySupplier
                {
                    SupplierId= a.SupplierId,
                    SupplierOrderId = a.Id,
                    SupplierOrderNumber = a.Number,                   
                })                
                .ToList();

            foreach (var item in supplierOrders)
            {
                item.SupplierName = supplierService.SupplierNameById(item.SupplierId);
                item.SupplierAndOrderSelect = item.SupplierName +"-" + item.SupplierOrderNumber;
            }

            supplierOrders = supplierOrders.OrderBy(a=>a.SupplierName).ToList();    

            return supplierOrders;
        }

        public void TotalAmountAndQuantitySum(int supplierOrderId)
        {
           var spOrder = dbContext.SupplierOrders.Find(supplierOrderId);
           spOrder.VatAmount = spOrder.Amount * spOrder.VAT / 100;
           spOrder.TotalAmount = (decimal)(spOrder.Amount + spOrder.VatAmount);
           spOrder.Balance = spOrder.TotalAmount - spOrder.PaidAvance;
          // spOrder.TotalQuantity = spOrder.Products.Sum(a => a.OrderedQuantity);
           dbContext.SaveChanges();    
          
        }
    }
}
