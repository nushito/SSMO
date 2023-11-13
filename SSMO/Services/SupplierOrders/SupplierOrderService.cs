using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.Purchase;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.Products;
using SSMO.Services.TransportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SSMO.Services.SupplierOrders
{
    public class SupplierOrderService : ISupplierOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        private readonly ISupplierService supplierService;
        private readonly IProductService productService;
        public SupplierOrderService(ApplicationDbContext dbContext, IMapper mapper, ISupplierService supplierService, 
            IProductService productService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper.ConfigurationProvider;
            this.supplierService = supplierService;
            this.productService = productService;
        }

        public async Task<int> CreateSupplierOrder(int myCompanyId, int supplierId, DateTime Date, 
            string number, int statusId, int currencyId, string fscClaim, int vat,            
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
                LoadingAddress = loadingAddress,
                DeliveryAddress = deliveryAddress,
                DeliveryTerms = deliveryTerms
            };
          
          await dbContext.SupplierOrders.AddAsync(supplierSpec);
           await dbContext.SaveChangesAsync();

            return supplierSpec.Id;
        }
        public bool EditSupplierOrderPurchasePayment
            (int id, decimal? paidAdvance, DateTime? date, string newCurrency, string action, decimal? exchangeRate,
             ICollection<PurchaseNewpaymentsPerOrderFormModel> purchasePayments)
        {           
            var supplierOrder = dbContext.SupplierOrders
                .Where(i=>i.Id == id)
                .FirstOrDefault();

            var purchaseInvoices = dbContext.Documents
                .Where(s => s.SupplierOrderId == id && s.DocumentType == Data.Enums.DocumentTypes.Purchase)
                .ToList();

            if(supplierOrder.Balance < paidAdvance)
            {
                return false;
            }

            var newPayment = new Payment();

            if(paidAdvance > 0.001m)            
            {
                newPayment.PaidAmount = (decimal)paidAdvance;
                newPayment.Date = (DateTime)date;
                newPayment.SupplierOrderId = id;
                newPayment.CurrencyId = supplierOrder.CurrencyId;
                newPayment.CurruncyRateExchange = exchangeRate;

                if (exchangeRate != 0 && newCurrency != null)
                {
                    var currencyId = dbContext.Currencies
                        .Where(i=>i.Name.ToLower() == newCurrency.ToLower())
                        .Select(i=>i.Id).FirstOrDefault();
                    newPayment.CurrencyForCalculationsId = currencyId;

                    switch (action)
                    {
                        case "/":
                            newPayment.NewAmountPerExchangeRate = (decimal)paidAdvance / exchangeRate ?? 0;                            
                            break;
                        case "*":
                            newPayment.NewAmountPerExchangeRate = (decimal)paidAdvance * exchangeRate ?? 0; 
                            break;
                        default: break;
                    }
                }

                supplierOrder.Balance -= paidAdvance ?? 0;

                dbContext.Payments.Add(newPayment);
                supplierOrder.Payments.Add(newPayment);

                dbContext.SaveChanges();
            }
                        
            if(supplierOrder.Balance <= 0.001m)
            {
                supplierOrder.PaidStatus = true;
                purchaseInvoices.ToList().ForEach(t=>t.PaidStatus = true);
                return true;
            }
            else
            {
                supplierOrder.PaidStatus = false;
            }

            foreach (var payment in purchasePayments)
            {
                if(payment.NewPaidAmount > 0.001m)
                {
                    var purchaseInvoice = dbContext.Documents.Find(payment.Id);
                    DateTime newDate = (DateTime)payment.NewDatePaidAmount;

                    newPayment.PaidAmount = payment.NewPaidAmount;
                    newPayment.Date = newDate;
                    newPayment.DocumentId = purchaseInvoice.Id;
                    newPayment.CurrencyId = purchaseInvoice.CurrencyId;
                    newPayment.CurruncyRateExchange = exchangeRate;
                    newPayment.SupplierOrderId = supplierOrder.Id;                                     

                    if (exchangeRate != 0 && newCurrency != null)
                    {
                        var currencyId = dbContext.Currencies
                            .Where(i => i.Name.ToLower() == newCurrency.ToLower())
                            .Select(i => i.Id).FirstOrDefault();
                        newPayment.CurrencyForCalculationsId = currencyId;

                        switch (action)
                        {
                            case "/":
                                newPayment.NewAmountPerExchangeRate = (decimal)payment.NewPaidAmount / exchangeRate ?? 0;
                                break;
                            case "*":
                                newPayment.NewAmountPerExchangeRate = (decimal)payment.NewPaidAmount * exchangeRate ?? 0;
                                break;
                            default: break;
                        }
                                               
                        supplierOrder.Balance -= payment.NewPaidAmount;
                        purchaseInvoice.Balance -= payment.NewPaidAmount;

                        if (purchaseInvoice.Balance <= 0.001m)
                        {
                            purchaseInvoice.PaidStatus = true;
                        }
                        if (supplierOrder.Balance <= 0.001m)
                        {
                            supplierOrder.PaidStatus = true;
                            return true;
                        }
                        continue;
                    }

                    dbContext.Payments.Add(newPayment);
                    purchaseInvoice.Payments.Add(newPayment);                                  

                    supplierOrder.Balance -= payment.NewPaidAmount;
                    purchaseInvoice.Balance -= payment.NewPaidAmount;

                    if (purchaseInvoice.Balance <= 0.001m)
                    {
                        purchaseInvoice.PaidStatus = true;
                    }
                    if (supplierOrder.Balance <= 0.001m)
                    {
                        supplierOrder.PaidStatus = true;                       
                        return true;
                    }
                }

                if (payment.IsChecked == true)
                {
                    newPayment.DocumentId = payment.Id;
                    dbContext.SaveChanges();
                    productService.CalculateCostPriceInDiffCurrency(payment.Id, action);
                }
            }

            dbContext.SaveChanges();
            
            return true;
        }

        public EditSupplierOrderPaymentModel GetPaymentsPerOrderForEdit(int id)
        {
            var supplierOrder = dbContext.SupplierOrders
                .Where(i=>i.Id == id)
                .FirstOrDefault();

            var purchases = dbContext.Documents
               .Where(s => s.SupplierOrderId == id && s.DocumentType == Data.Enums.DocumentTypes.Purchase)
               .ToList();            

            var currencies = purchases.Select(i=>i.CurrencyId).ToList();

            var costPriceCurrency = dbContext.Currencies
                .Where(i=> purchases.Select(i=>i.CostPriceCurrencyId).Distinct().FirstOrDefault() != supplierOrder.CurrencyId)                
                .Select(n=>n.Name)
                .FirstOrDefault();

            var currency = dbContext.Currencies
                   .Where(i => i.Id == supplierOrder.CurrencyId)
                   .Select(n => n.Name).FirstOrDefault();

            var supplierOrderForEdit = new EditSupplierOrderPaymentModel
            { 
                Number = supplierOrder.Number,
                PurchasePaymentsCollection = new List<PurchaseNewpaymentsPerOrderFormModel>(),
                Currency= currency,
                ConvertToThatCurrency = costPriceCurrency
            };           
           
            foreach (var purchase in purchases)
            {               
                var purchasePayment = new PurchaseNewpaymentsPerOrderFormModel
                {
                    Id= purchase.Id,
                    PurchaseNumber = purchase.PurchaseNumber                    
                };

                supplierOrderForEdit.PurchasePaymentsCollection.Add(purchasePayment);
            }
           
            return supplierOrderForEdit;
        }

        public SupplierOrderPaymentCollectionModel GetSupplierOrders
            (string supplierName, DateTime startDate, DateTime endDate, int currentpage, int supplierOrdersPerPage)
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
                .Where(sup=>sup.SupplierId == supplierId && sup.Date >= startDate && sup.Date <= endDate);

            var supplierOrdersCollection = supplierOrders.ProjectTo<SupplierOrdersPaymentDetailsModel>(mapper).ToList();

            var supplierOrderPaymentCollection = new SupplierOrderPaymentCollectionModel
            {
                TotalSupplierOrders = supplierOrders.Count(),
                SupplierOrderPaymentCollection = supplierOrdersCollection.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage)
            };

            foreach (var order in supplierOrderPaymentCollection.SupplierOrderPaymentCollection.ToList())
            {
                order.PurchaseCurrency = dbContext.Currencies
                    .Where(i => i.Id == order.CurrencyId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();                   

                order.Payments = new List<SupplierPaymentDetailsViewModel>();
                order.PurchasePaymentsCollection = new List<PurchasePerOrderPaymentsViewModel>();

                var payments = dbContext.Payments
               .Where(s => s.SupplierOrderId == order.Id)
               .ToList();

                if (payments != null)
                {
                    foreach (var payment in payments)
                    {
                        var supplierPayment = new SupplierPaymentDetailsViewModel
                        {
                            Date = payment.Date,
                            PaidAmount = payment.PaidAmount,
                        };
                        order.Payments.Add(supplierPayment);
                    }
                }
                var purchases = dbContext.Documents
                    .Where(s => s.SupplierOrderId == order.Id && s.DocumentType == Data.Enums.DocumentTypes.Purchase)
                    .ToList();

                foreach (var purchase in purchases)
                {
                    var purchasePayment = new PurchasePerOrderPaymentsViewModel
                    {
                        Id = purchase.Id,
                        Balance = purchase.Balance,
                        Amount = purchase.Amount,
                        DatePaidAmount = purchase.DatePaidAmount,
                        PaidAdvance = purchase.PaidAvance,
                        PurchaseNumber = purchase.PurchaseNumber,                       
                        PurchasePaymentsDetails = new List<PurchaseAllPaymentsViewModel>()
                    };

                    var purchasePayments = dbContext.Payments
                    .Where(s => s.DocumentId == purchase.Id)
                    .ToList();

                    if (purchasePayments != null)
                    {
                        foreach (var payment in purchasePayments)
                        {
                            purchasePayment.PurchasePaymentsDetails.Add(new PurchaseAllPaymentsViewModel
                            {
                                Date = payment.Date,
                                PaidAmount = payment.PaidAmount
                            });
                        }
                    }

                    order.PurchasePaymentsCollection.Add(purchasePayment);
                }

            }

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

        //izchislqva sumata i balansa na specifikaciyata ot dostavchika
        public async Task TotalAmountAndQuantitySum(int supplierOrderId)
        {
           var spOrder = dbContext.SupplierOrders.Find(supplierOrderId);
           spOrder.VatAmount = spOrder.Amount * spOrder.VAT / 100;
           spOrder.TotalAmount = (decimal)(spOrder.Amount + spOrder.VatAmount);
           spOrder.Balance = spOrder.TotalAmount - spOrder.PaidAvance;
          // spOrder.TotalQuantity = spOrder.Products.Sum(a => a.OrderedQuantity);
          await dbContext.SaveChangesAsync();    
          
        }

        public ICollection<SupplierOrderJsonListForServiceOrder> GetSupplierOrder(int id)
        {
            var supplierId = dbContext.Suppliers.Where(a => a.Id.Equals(id)).Select(i => i.Id).FirstOrDefault();
            return dbContext.SupplierOrders
                .Where(s => s.SupplierId == supplierId)
                .Select(n => new SupplierOrderJsonListForServiceOrder
                {
                    SupplierOrderId = n.Id,
                    SupplierOrderNumber = n.Number
                   
                }).ToList();
        }

    }
}
