using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Services.Suppliers;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using SSMO.Services.SupplierOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSMO.Models.Reports.Purchase;
using SSMO.Data.Enums;
using SSMO.Models.CustomerOrders;
using Microsoft.EntityFrameworkCore;
using SSMO.Repository;
using SSMO.Services.CustomerOrder.Models;
using SSMO.Models.Reports.ServiceOrders;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SSMO.Services.Reports
{
    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext dbcontext;
        private readonly IConfigurationProvider mapper;
        private readonly IProductService productService;
        private readonly IInvoiceService invoiceService;
        private readonly ISupplierOrderService supplierOrderService;
        private readonly IMycompanyService myCompanyService;
        private readonly ICustomerOrderService customerOrderService;
        private readonly ISupplierService supplierService;
        private readonly IProductRepository productRepository;

        public ReportsService
            (ApplicationDbContext context, IConfigurationProvider mapper,
            IProductService productService, IInvoiceService invoiceService,
            ISupplierOrderService supplierOrderService, IMycompanyService mycompanyService,
            ICustomerOrderService customerOrderService, ISupplierService supplierService,
            IProductRepository productRepository)
        {
            dbcontext = context;
            this.mapper = mapper;//.ConfigurationProvider;
            this.productService = productService;
            this.invoiceService = invoiceService;
            this.supplierOrderService = supplierOrderService;
            this.myCompanyService = mycompanyService;
            this.customerOrderService = customerOrderService;
            this.supplierService = supplierService;
            this.productRepository = productRepository;
        }
        public CustomerOrdersQueryModel AllCustomerOrders
            (string customerName, DateTime startDate, DateTime endDate,
            int currentpage, int customerOrdersPerPage)
        {

            if (String.IsNullOrEmpty(customerName))
            {
                
                return new CustomerOrdersQueryModel();
            }

            var customerId = dbcontext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryOrders = dbcontext.CustomerOrders
                     .Where(a => a.CustomerId == customerId && a.Date >= startDate && a.Date <= endDate);

            // var totalOrders = queryOrders.Count();  

            var orders = queryOrders.ProjectTo<CustomerOrderDetailsModel>(this.mapper).ToList();
           
            var customerOrdersList = orders.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            foreach (var order in customerOrdersList)
            {
                order.StatusName = dbcontext.Statuses
                    .Where(i => i.Id == order.StatusId)
                    .Select(n => n.Name)
                    .FirstOrDefault();
            }

            var customerOrdersCollection = new CustomerOrdersQueryModel
            {
                TotalCustomerOrders = orders.Count(),
                CustomerOrders = customerOrdersList
            };

            return customerOrdersCollection;
        }

        public CustomerOrderForEdit CustomerOrderDetailsForEdit(int id)
        {
            var corder = dbcontext.CustomerOrders.Find(id);
            var customerId = dbcontext.CustomerOrders.Where(a => a.Id == id).Select(a => a.CustomerId).FirstOrDefault();

            var orderForEdit = new CustomerOrderForEdit
            {
                CurrencyId = corder.CurrencyId,
                CustomerPoNumber = corder.CustomerPoNumber,
                CustomerId = customerId,
                Date = corder.Date,
                DeliveryAddress = corder.DeliveryAddress,
                LoadingPlace = corder.LoadingPlace,
                DeliveryTerms = corder.DeliveryTerms,
                FscCertificate = corder.FSCSertificate,
                FscClaim = corder.FSCClaim,
                MyCompanyId = corder.MyCompanyId,               
                Origin = corder.Origin,
                StatusId = corder.StatusId,
                Vat = corder.Vat,
                PaymentTerms = corder.PaymentTerms
            };
           
            if(corder.FiscalAgentId != 0)
            {

                orderForEdit.FiscalAgent = dbcontext.FiscalAgents
                    .Where(i => i.Id == corder.FiscalAgentId)
                    .Select(n => new FiscalAgentViewModel
                    {
                        Name = n.Name,
                        Id = n.Id
                    })
                    .FirstOrDefault();
            }

            orderForEdit.Products = (List<ProductCustomerFormModel>)productService.DetailsPerCustomerOrder(id);

            return orderForEdit;
        }

        public CustomerInvoicesPaymentCollectionViewModel CustomersInvoicesPaymentDetails
            (string customerName, int currentpage, int customerInvoicePerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
                return new CustomerInvoicesPaymentCollectionViewModel();
            }

            var customerId = dbcontext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryInvoices = dbcontext.Documents
                    .Where(a => a.DocumentType == Data.Enums.DocumentTypes.Invoice && a.CustomerId == customerId);

            var invoices = queryInvoices.ProjectTo<CustomerInvoicePaymentDetailsModel>(this.mapper).ToList();

            foreach (var invoice in invoices)
            {
                invoice.CustomerOrders = dbcontext.CustomerOrders
                               .Where(i => invoice.Id == i.Documents.Select(a => a.Id).FirstOrDefault())
                               .Select(i => new CustomerOrdersPaymentDetailsPerInvoice
                               {
                                   Id = i.Id,
                                   OrderConfirmationNumber = i.OrderConfirmationNumber,
                                   PaidAvance = i.PaidAvance,    
                                   Payments = i.Payments.Select(a=> new PaymentViewModel
                                   {
                                       PaidAmount = a.PaidAmount,
                                       Date = a.Date
                                   }).ToList()
                               })
                               .ToList();   
                
                invoice.Payments = dbcontext.Payments
                    .Where(i=>i.DocumentId == invoice.Id)
                    .Select(i=> new PaymentViewModel
                    {
                        PaidAmount = i.PaidAmount,
                        Date = i.Date
                    }).ToList();
             }

            var customerPaymentList = invoices.Skip((currentpage - 1) * customerInvoicePerPage).Take(customerInvoicePerPage);

            var custpmerOrdersPayments = new CustomerInvoicesPaymentCollectionViewModel
            {
                TotalInvoices = queryInvoices.Count(),
                CustomerInvoices = customerPaymentList
            };

            return custpmerOrdersPayments;
        }

        public SupplierInvoiceCollectionViewModel SuppliersInvoicesPaymentDetails
            (string supplierName, int currentpage, int supplierInvoicePerPage)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return new SupplierInvoiceCollectionViewModel();
            }

            var supplierId = dbcontext.Suppliers
                .Where(n => n.Name.ToLower() == supplierName.ToLower())
                .Select(i => i.Id)
                .FirstOrDefault();


            var purchaseInvoices = dbcontext.Documents
                .Where(n => n.SupplierId == supplierId && n.DocumentType == Data.Enums.DocumentTypes.Purchase);

            var purchases = purchaseInvoices.ProjectTo<SupplierInvoicePaymentDetailsModel>(mapper).ToList();
            var purchasePaymentList = purchases.Skip((currentpage - 1) * supplierInvoicePerPage).Take(supplierInvoicePerPage);

            var purchaseInvoiceCollection = new SupplierInvoiceCollectionViewModel
            {
                TotalPurchaseInvoices = purchaseInvoices.Count(),
                PurchaseInvoices = purchasePaymentList
            };

            return purchaseInvoiceCollection;
        }
        public CustomerOrderDetailsModel CustomerOrderDetails(int id)
        {
            var findorder = dbcontext.CustomerOrders.Where(a => a.Id == id);
            CustomerOrderDetailsModel order = findorder.ProjectTo<CustomerOrderDetailsModel>(mapper).FirstOrDefault();

            var products = dbcontext.CustomerOrderProductDetails
                .Where(i => i.CustomerOrderId == id);
            
           order.Products = products.ProjectTo<ProductsForCustomerOrderDetailsViewModel>(mapper).ToList();

            foreach (var product in order.Products)
            {
                var mainProduct = dbcontext.Products
                    .Where(i => i.Id == product.ProductId)
                    .FirstOrDefault();

                product.DescriptionName = productService.GetDescriptionName(mainProduct.DescriptionId);
                product.GradeName = productService.GetGradeName(mainProduct.GradeId);
                product.SizeName = productService.GetSizeName(mainProduct.SizeId);

                var supplierId = dbcontext.SupplierOrders
                    .Where(i => i.Id == product.SupplierOrderId)
                    .Select(i => i.SupplierId).FirstOrDefault();


               product.SupplierOrderNumber = dbcontext.SupplierOrders
                    .Where(i => i.Id == product.SupplierOrderId)
                    .Select(i => i.Number).FirstOrDefault();


                product.SupplierName = dbcontext.Suppliers
                    .Where(i=>i.Id == supplierId)
                    .Select(n=>n.Name).FirstOrDefault();
            }

            var myCompanyId = findorder.
                Select(id => id.MyCompanyId).
                FirstOrDefault();

            var myCompanyName = dbcontext.MyCompanies
                .Where(id => id.Id == myCompanyId)
                .Select(n => n.Name)
                .FirstOrDefault();

            order.MyCompanyName = myCompanyName;

            if(order.FIscalAgentId != 0)
            {
                order.FiscalAgentName = dbcontext.FiscalAgents
                    .Where(i=>i.Id == order.FIscalAgentId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();
            }
            return order;
        }

        public async Task<bool> EditCustomerOrder(int id,
            string number, System.DateTime date,
            int myCompanyId, string deliveryTerms,
            string loadingPlace, string deliveryAddress,
            int currencyId, int status, string fscClaim,
            string fscCertificate, 
             IList<ProductCustomerFormModel> products, List<int> banks, 
             int? fiscalAgentId, int? fscText, string paymentTerms, string eta, string etd)
        {
            var order = dbcontext.CustomerOrders                
                .Where(a=>a.Id == id)
                .Include(a => a.BankDetails)
                .FirstOrDefault();                     

            if (order == null)
            {
                return false;
            }

            order.CustomerPoNumber = number;
            order.Date = date;
            order.MyCompanyId = myCompanyId;
            order.DeliveryTerms = deliveryTerms;
            order.LoadingPlace = loadingPlace;
            order.DeliveryAddress = deliveryAddress;
            order.CurrencyId = currencyId;
            order.StatusId = status;
            order.FSCClaim = fscClaim;
            order.FSCSertificate = fscCertificate;           
            order.Amount = 0;
            order.PaymentTerms = paymentTerms;
            order.Eta = eta;
            order.Etd = etd;
         
            if(fiscalAgentId != null)
            {
                order.FiscalAgentId = fiscalAgentId;
            }

            if(fscText!= null)
            {
                order.FscTextId = fscText;
            }
            

            var newList = new List<BankDetails>();  

           var bankDetails = dbcontext.BankDetails                
                .Where(i=> banks.Contains(i.Id))
                .ToList();

            var oldBanks = order.BankDetails                
                .ToList();

            oldBanks?.ForEach(i => order.BankDetails.Remove(i));

            if (products != null)
            {
                foreach (var product in products)
                {
                    var mainProduct = productRepository.GetMainProduct(product.ProductId);
                    var size = productService.GetSizeName(mainProduct.SizeId);

                    var productPerCorder = dbcontext.CustomerOrderProductDetails
                                       .Where(co => co.Id == product.Id)
                                       .FirstOrDefault();
                    var oldQuantity = productPerCorder.Quantity;
                    var oldTotalSheets = productPerCorder.TotalSheets;

                    if (product.Quantity == 0)
                    {
                        productPerCorder.Quantity = 0;
                        productPerCorder.AutstandingQuantity = 0;
                        continue;
                    }
                    else
                    {
                        var invoicedQuantity = dbcontext.InvoiceProductDetails
                            .Where(p => p.ProductId == mainProduct.Id
                            && p.CustomerOrderId == id
                            && p.CustomerOrderProductDetailsId == productPerCorder.Id)
                            .Select(q => q.InvoicedQuantity)
                            .FirstOrDefault();
                           

                        productPerCorder.Pallets = product.Pallets;
                        productPerCorder.SellPrice = product.SellPrice;
                        productPerCorder.SheetsPerPallet = product.SheetsPerPallet;
                        productPerCorder.Unit = Enum.Parse<Unit>(product.Unit.ToString());
                        productPerCorder.AutstandingQuantity = product.Quantity - invoicedQuantity;
                        productPerCorder.Quantity = product.Quantity;
                        productPerCorder.TotalSheets = product.Pallets * product.SheetsPerPallet;
                        productPerCorder.Amount = Math.Round(product.SellPrice * product.Quantity, 5);

                        if(invoicedQuantity > 0m)
                        {
                            productPerCorder.AutstandingQuantity -= invoicedQuantity;
                        }

                        if (product.Unit == mainProduct.Unit.ToString())
                        {
                            mainProduct.QuantityAvailableForCustomerOrder = mainProduct.QuantityAvailableForCustomerOrder +oldQuantity - productPerCorder.Quantity;
                        }
                        else
                        {
                            var oldAvailableQuantity = productService.ConvertUnitQuantityToDiffUnitQuantity
                            (mainProduct.Unit.ToString(), product.Unit.ToString(), oldQuantity,
                                size, oldTotalSheets);

                            var quantity = productService.ConvertUnitQuantityToDiffUnitQuantity
                            (mainProduct.Unit.ToString(), product.Unit.ToString(), product.Quantity,
                                size, productPerCorder.TotalSheets);

                            mainProduct.QuantityAvailableForCustomerOrder = mainProduct.QuantityAvailableForCustomerOrder + oldAvailableQuantity - quantity;
                        }
                    }
                    order.Amount += productPerCorder.Amount;
                }
            }

                order.SubTotal = order.Amount * order.Vat / 100 ?? 0;
                order.TotalAmount = order.Amount + order.SubTotal;                

            await dbcontext.SaveChangesAsync();

            bankDetails.ForEach(i => order.BankDetails.Add(i));

            await dbcontext.SaveChangesAsync();
            return true;
        }

        public IEnumerable<CustomerOrderListViewBySupplier> GetCustomerOrdersBySupplier(int customerId, string supplierId)
        {
            if (supplierId == null)
            {
                return Enumerable.Empty<CustomerOrderListViewBySupplier>();
            }

            var supplierIdInt = int.Parse(supplierId.ToString());
            var costumercustomerOrderList = dbcontext.SupplierOrders
                .Where(o => o.SupplierId == supplierIdInt)
                .Select(co => co.CustomerOrderId)
                .ToList();

            var customerOrdersBySupplier = dbcontext.CustomerOrders
                .Where(co => costumercustomerOrderList.Contains(co.Id))
                .Select(n => new CustomerOrderListViewBySupplier
                {
                    OrderConfirmationNumber = n.OrderConfirmationNumber,
                    CustomerPoNumber = n.CustomerPoNumber,
                    Date = n.Date,
                    DeliveryAddress = n.DeliveryAddress,
                    LoadingPlace = n.LoadingPlace,
                    StatusId = n.StatusId,
                    PaidAmountStatus = n.PaidAmountStatus,
                    StatusName = dbcontext.Statuses.Where(a => a.Id == n.StatusId).Select(a => a.Name).FirstOrDefault()
                }).ToList();

            return customerOrdersBySupplier;
        }

        public CustomerOrderPaymentCollectionViewModel CustomerOrdersPaymentDetails
            (string customerName, int currentpage, int customerOrdersPerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
                return new CustomerOrderPaymentCollectionViewModel();
            }

            var customerId = dbcontext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var customerOrders = dbcontext.CustomerOrders.Where(c => c.CustomerId == customerId);

            var incustomerOrdersCollectionvoices = customerOrders.ProjectTo<CustomerOrderDetailsPaymentModel>(this.mapper).ToList();

            var customerOrdersPaymentList = incustomerOrdersCollectionvoices.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            foreach (var customerOrder in customerOrdersPaymentList)
            {
                var customerOrderPayments = dbcontext.Payments
                    .Where(i => i.CustomerOrderId == customerOrder.Id)
                    .Select(i => new CustomerOrderPaymentsDetailsViewModel
                    {
                        PaidAmount= i.PaidAmount,
                        DatePaidAmount = i.Date,
                    }).ToList();

                customerOrder.CustomerOrderPayments= customerOrderPayments;

                var invoices = dbcontext.CustomerOrders
                    .Where(i=>i.Id == customerOrder.Id)
                    .Select(i=>i.Documents)
                    .FirstOrDefault();

                if (invoices != null)
                {
                    var invoicesPaymentDetailsCollection = new List<CustomerOrderInvoicesPaymentDetailsViewModel>();
                    foreach (var invoice in invoices)
                    {
                        var detail = new CustomerOrderInvoicesPaymentDetailsViewModel
                        {
                            AdvancePayment = invoice.PaidAvance,
                            DateAdvancePayment = invoice.DatePaidAmount,
                            DocumentNumber = invoice.DocumentNumber,
                            Date = invoice.Date,
                            Payments = new List<CustomerOrderInvoicesPaymentCollectionViewModel>()
                        };

                       var payments = dbcontext.Payments
                            .Where(i=>i.DocumentId == invoice.Id)
                            .ToList();  
                        if(payments != null)
                        {
                            detail.Payments = payments
                                .Select(i=> new CustomerOrderInvoicesPaymentCollectionViewModel
                            {
                                Date = i.Date,
                                Payment = i.PaidAmount
                            })
                                .ToList();
                        }

                        invoicesPaymentDetailsCollection.Add(detail);
                    }

                    customerOrder.InvoicesDetails = invoicesPaymentDetailsCollection;
                }
            }
            
            var customerOrdersPayments = new CustomerOrderPaymentCollectionViewModel
            {
                TotalCustomerOrders = customerOrders.Count(),
                CustomerPaymentCollection = customerOrdersPaymentList
            };
            return customerOrdersPayments;

        }

        public SupplierOrdersQueryModel AllSupplierOrders
            (string name, DateTime startDate, DateTime endDate, int currentpage, int supplierOrdersPerPage)
        {
            if (String.IsNullOrEmpty(name))
            {
                return new SupplierOrdersQueryModel();
            }

            var supplierId = dbcontext.Suppliers.Where(a => a.Name.ToLower() == name.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryOrders = dbcontext.SupplierOrders
                    .Where(a => a.SupplierId == supplierId && a.Date >= startDate && a.Date <= endDate);

            var supplierOrders = queryOrders.ProjectTo<SupplierOrderDetailsModel>(this.mapper).ToList();
            
            var supplierOrdersList = supplierOrders.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage);

            var supplierOrdersQery = new SupplierOrdersQueryModel
            {
                TotalSupplierOrders = supplierOrders.Count(),
                SupplierOrders = supplierOrdersList
            };

            return supplierOrdersQery;
        }

        public SupplierOrderForEditModel SupplierOrderForEditDetails(int supplierOrderId)
        {
            var supplierOrder = dbcontext.SupplierOrders
                .Where(num => num.Id == supplierOrderId)
                .FirstOrDefault();

            if (supplierOrderId == 0) return null;

            var supplierOrderForEdit = new SupplierOrderForEditModel
            {
                Number = supplierOrder.Number,
                Amount = supplierOrder.Amount,
                CurrencyId = supplierOrder.CurrencyId,
                Date = supplierOrder.Date,
                DatePaidAmount = supplierOrder.DatePaidAmount,
                DeliveryAddress = supplierOrder.DeliveryAddress,
                LoadingAddress = supplierOrder.LoadingAddress,
                DeliveryTerms = supplierOrder.DeliveryTerms,
                FSCClaim = supplierOrder.FscClaim,
                FSCSertificate = supplierOrder.FscSertificate,
                GrossWeight = supplierOrder.GrossWeight,
                TotalQuantity = supplierOrder.TotalQuantity,
                MyCompanyId = supplierOrder.MyCompanyId,
                NetWeight = supplierOrder.NetWeight,
                VAT = supplierOrder.VAT,
                NewProducts = new List<NewProductsForSupplierOrderModel>()
            };

            return supplierOrderForEdit;
        }

        public async Task<bool> EditSupplierOrder
            (int supplierOrderId, string supplierOrderNumber,
            DateTime date, int myCompanyId, string deliveryTerms,
            string loadingPlace, string deliveryAddress,
            decimal grossWeight, decimal netWeight, int currencyId,
            int status, int customerOrderNumber, string fscClaim, string fscCertificate,
            decimal paidAdvance, bool paidStatus, int? vat, List<ProductsForEditSupplierOrder> products, 
            List<NewProductsForSupplierOrderModel> newProducts)
        {
            if (supplierOrderNumber == null) return false;

            var supplierOrder = dbcontext.SupplierOrders
                .Where(num => num.Number.ToLower() == supplierOrderNumber.ToLower())
                .FirstOrDefault();

            if (supplierOrder == null) return false;

            supplierOrder.Number = supplierOrderNumber;
            supplierOrder.Date = date;
            supplierOrder.MyCompanyId = myCompanyId;
            supplierOrder.DeliveryAddress = deliveryAddress;
            supplierOrder.LoadingAddress = loadingPlace;
            supplierOrder.VAT = vat;
            supplierOrder.GrossWeight = grossWeight;
            supplierOrder.NetWeight = netWeight;
            supplierOrder.CurrencyId = currencyId;
            supplierOrder.StatusId = status;
            supplierOrder.FscClaim = fscClaim;
            supplierOrder.FscSertificate = fscCertificate;
            supplierOrder.PaidStatus = paidStatus;
            supplierOrder.Amount = 0;

            if (products.Count != 0)
            {
                foreach (var item in products)
                {
                    var product = dbcontext.Products
                       .Where(i => i.Id == item.Id).FirstOrDefault();

                    if(item.OrderedQuantity == 0)
                    {
                        product.QuantityAvailableForCustomerOrder = 0;
                        product.QuantityLeftForPurchaseLoading = 0;
                        continue;
                    }

                    product.DescriptionId = item.DescriptionId;
                    product.GradeId = item.GradeId;
                    product.SizeId = item.SizeId;
                    product.PurchasePrice = item.PurchasePrice;
                    product.Pallets = item.Pallets;
                    product.SheetsPerPallet= item.SheetsPerPallet;
                    product.Unit = Enum.Parse<Unit>(item.Unit);
                    product.TotalSheets = item.Pallets * item.SheetsPerPallet;
                    product.Amount = item.OrderedQuantity * item.PurchasePrice;
                    product.OrderedQuantity = item.OrderedQuantity;
                    product.QuantityLeftForPurchaseLoading = item.OrderedQuantity - product.LoadedQuantity;

                    var sumProductInCustomerOrders = dbcontext.CustomerOrderProductDetails
                        .Where(p => p.ProductId == item.Id)
                        .Sum(q => q.Quantity);
                    product.QuantityAvailableForCustomerOrder = item.OrderedQuantity - sumProductInCustomerOrders;
                    supplierOrder.Amount += product.Amount;
                }

                if(newProducts.Count > 0)
                {
                    foreach (var item in newProducts)
                    {
                        productService.CreateNewProductOnEditSupplierOrder(item);
                    }
                }

               await supplierOrderService.TotalAmountAndQuantitySum(supplierOrderId);
            }

            await dbcontext.SaveChangesAsync();
            return true;
        }

        public SupplierOrderDetailsModel SupplierOrderDetail(int id)
        {
            var supplierOrder = dbcontext.SupplierOrders.Where(o => o.Id == id);

            if (supplierOrder == null) return null;

            SupplierOrderDetailsModel supplierOrderDetail = supplierOrder.ProjectTo<SupplierOrderDetailsModel>(mapper).FirstOrDefault();

            var myCompanyId = supplierOrder.
              Select(id => id.MyCompanyId).
              FirstOrDefault();

            var myCompanyName = dbcontext.MyCompanies
                .Where(id => id.Id == myCompanyId)
                .Select(n => n.Name)
                .FirstOrDefault();

            supplierOrderDetail.Currency = dbcontext.Currencies
                .Where(i => i.Id == supplierOrderDetail.CurrencyId)
                .Select(n=>n.Name)
                .FirstOrDefault();

            supplierOrderDetail.MyCompanyName = myCompanyName;

            var supplierId = supplierOrder.Select(id => id.SupplierId).FirstOrDefault();
            supplierOrderDetail.SupplierName = dbcontext.Suppliers
                .Where(i => i.Id == supplierId)
                .Select(n => n.Name)
                .FirstOrDefault();

            //var customerId = dbcontext.CustomerOrders
            //    .Where(i => i.Id == supplierOrderDetail.CustomerOrderId)
            //    .Select(c => c.CustomerId)
            //    .FirstOrDefault();

            //supplierOrderDetail.CustomerName = dbcontext.Customers
            //     .Where(i => i.Id == customerId)
            //     .Select(n => n.Name)
            //     .FirstOrDefault();

            supplierOrderDetail.StatusName = dbcontext.Statuses
                .Where(i => i.Id == supplierOrderDetail.StatusId)
                .Select(n => n.Name)
                .FirstOrDefault();

            //supplierOrderDetail.CustomerOrderConfirmationNumber = dbcontext.CustomerOrders
            //    .Where(id => id.Id == supplierOrderDetail.CustomerOrderId)
            //    .Select(num => num.OrderConfirmationNumber)
            //    .FirstOrDefault();

            supplierOrderDetail.PurchaseInvoiceNumbers = dbcontext.Documents
                .Where(s => s.SupplierOrderId == id)
                .Select(n => n.PurchaseNumber)
                .ToList();

            var products = dbcontext.Products
                .Where(s => s.SupplierOrderId == id);

            supplierOrderDetail.Products = products.ProjectTo<ProductsSupplierOrderDetailsViewModel>(mapper).ToList();  

            return supplierOrderDetail;
        }

        public InvoiceReportModel InvoiceCollection
            (string myCompanyName, DateTime startDate, DateTime endDate, int currentpage =1, int invoicesPerPage = int.MaxValue)
        {
            if (myCompanyName == null) return new InvoiceReportModel();

            var companyId = myCompanyService.GetMyCompanyId(myCompanyName);

            var invoices = dbcontext.Documents
                .Where(type => type.DocumentType == DocumentTypes.Invoice || 
                       type.DocumentType == DocumentTypes.CreditNote || 
                       type.DocumentType == DocumentTypes.DebitNote &&
                       type.Date >= startDate && type.Date <= endDate)
                .Where(m => m.MyCompanyId == companyId).OrderByDescending(d=>d.DocumentNumber);
           
            var invoiceDetailsCollection = invoices.ProjectTo<InvoiceCollectionViewModel>(this.mapper).ToList();

            foreach (var invoice in invoiceDetailsCollection)
            {
                if(invoice.CreditToInvoiceId != 0)
                {
                    invoice.CreditToInvoiceDocumentNumber = dbcontext.Documents
                        .Where(id=>id.Id == invoice.CreditToInvoiceId)
                        .Select(n=>n.DocumentNumber).FirstOrDefault();
                    invoice.TotalAmount = invoice.CreditNoteTotalAmount;
                }
                else if(invoice.DebitToInvoiceId != 0)
                {
                    invoice.DebitToInvoiceDocumentNumber = dbcontext.Documents
                        .Where(id => id.Id == invoice.DebitToInvoiceId)
                        .Select(n => n.DocumentNumber).FirstOrDefault();
                    invoice.TotalAmount = invoice.DebitNoteTotalAmount;
                }                
            }

            var collection = new InvoiceReportModel
            {
                InvoiceCollection = invoiceDetailsCollection.Skip((currentpage - 1) * invoicesPerPage).Take(invoicesPerPage),
                TotalInvoices = invoiceDetailsCollection.Count(),
                CurrentPage = currentpage,
                InvoicesPerPage= invoicesPerPage
            };

            foreach (var invoice in collection.InvoiceCollection)
            {
                foreach (var order in invoice.CustomerOrders)
                {
                    var customerOrderNumber = customerOrderService.CustomerOrderNumberById(order.Id);
                    var customerName = dbcontext.Customers
                        .Where(i => i.Id == invoice.CustomerId)
                        .Select(n => n.Name)
                        .FirstOrDefault();
                    
                    invoice.CustomerName = customerName;
                }
            }
            return collection;
        }

        public InvoiceDetailsViewModel InvoiceDetails(int id)
        {
            var invoice = dbcontext.Documents
                .Include(b=>b.BankDetails)
                .Where(i => i.Id == id);

            if (invoice == null) return null;

            var invoiceDetails = invoice.ProjectTo<InvoiceDetailsViewModel>(mapper).FirstOrDefault();
            var productsDetails = new List<InvoiceProductsDetailsViewModel>();

            if (invoiceDetails.DocumentType == Data.Enums.DocumentTypes.Invoice.ToString())
            {
                var products = dbcontext.InvoiceProductDetails
                .Where(inv => inv.InvoiceId == id)
                .ProjectTo<InvoiceProductsDetailsViewModel>(mapper).ToList();

                productsDetails.AddRange(products);
            }
            else if(invoiceDetails.DocumentType == Data.Enums.DocumentTypes.CreditNote.ToString())
            {
                var products = dbcontext.InvoiceProductDetails
               .Where(inv => inv.CreditNoteId == id)
               .ProjectTo<InvoiceProductsDetailsViewModel>(mapper).ToList();

                productsDetails.AddRange(products) ;

                var invoiceNumber = dbcontext.Documents
                    .Where(i => i.Id == invoiceDetails.CreditToInvoiceId)
                    .Select(num => num.DocumentNumber)
                    .FirstOrDefault();

               invoiceDetails.CreditToInvoice= invoiceNumber;

            }
            else if(invoiceDetails.DocumentType == Data.Enums.DocumentTypes.DebitNote.ToString())
            {
                var products = dbcontext.InvoiceProductDetails
               .Where(inv => inv.DebitNoteId == id)
               .ProjectTo<InvoiceProductsDetailsViewModel>(mapper).ToList();

                productsDetails.AddRange(products);
                var invoiceNumber = dbcontext.Documents
                    .Where(i => i.Id == invoiceDetails.DebitToInvoiceId)
                    .Select(num => num.DocumentNumber)
                    .FirstOrDefault();

                invoiceDetails.DebitToInvoice = invoiceNumber;
            }

            foreach (var product in productsDetails)
            {
                var mainProduct = dbcontext.Products
                    .Where(i => i.Id == product.ProductId).FirstOrDefault();

                product.Description = productService.GetDescriptionName(mainProduct.DescriptionId);
                product.Size = productService.GetSizeName(mainProduct.SizeId);
                product.Grade = productService.GetGradeName(mainProduct.GradeId);
            }

            invoiceDetails.Products = productsDetails;

            var seller = dbcontext.MyCompanies
                .Where(i => i.Id == invoiceDetails.MyCompanyId);

            if (seller == null) return null;

            invoiceDetails.Seller = seller.ProjectTo<MyCompanyInvoiceDetailsModel>(mapper).FirstOrDefault();

            var sellerAddress = dbcontext.Addresses
                .Where(i => i.Id == invoiceDetails.Seller.AddressId)
                .FirstOrDefault();
            invoiceDetails.Seller.Street = sellerAddress.Street;
            invoiceDetails.Seller.City = sellerAddress.City;
            invoiceDetails.Seller.Country = sellerAddress.Country;
            //TODO dali vyarno darpa izbranite banki
            var bankDetails = dbcontext.BankDetails
                .Where(i => i.CompanyId == invoiceDetails.MyCompanyId && i.Documents.Select(i=>i.Id).Contains(id));

            var invoiceBankDetails = bankDetails.ProjectTo<InvoiceBankDetailsModel>(mapper).ToList();

            foreach (var bankDetail in invoiceBankDetails)
            {
                bankDetail.CurrencyName = dbcontext.Currencies
                     .Where(i => i.Id == bankDetail.CurrencyId)
                     .Select(n => n.Name)
                     .FirstOrDefault();
            }

            invoiceDetails.CompanyBankDetails = invoiceBankDetails;

            var customer = dbcontext.Customers
                .Where(i => i.Id == invoiceDetails.CustomerId);
            if (customer == null) return null;

            invoiceDetails.Customer = customer.ProjectTo<InvoiceCustomerDetailsModel>(mapper).FirstOrDefault();

            var customerAddress = dbcontext.Addresses
                .Where(i => i.Id == invoiceDetails.Customer.AddressId)
                .FirstOrDefault();

            invoiceDetails.Customer.Street = customerAddress.Street;
            invoiceDetails.Customer.City = customerAddress.City;
            invoiceDetails.Customer.Country = customerAddress.Country;
                       
            var customerOrders = dbcontext.CustomerOrders
                .Where(i=>i.Documents.Select(i=>i.Id).Contains(id))
                .ToList();
           
            invoiceDetails.OrderConfirmationNumber = customerOrders.Select(a => a.OrderConfirmationNumber).ToList();

            invoiceDetails.CustomerPoNumbers = customerOrders.Select(a => a.CustomerPoNumber).ToList();

            return invoiceDetails;
        }

        public PurchaseCollectionQueryModel PurchaseInvoices
            (string supplierName, DateTime startDate, DateTime endDate, int currentpage = 1, int invoiceperPage = int.MaxValue)
        {
            var purchaseDocuments = dbcontext.Documents
                .Where(d => d.DocumentType == Data.Enums.DocumentTypes.Purchase);

            if (supplierName != null)
            {
                var supplierId = dbcontext.Suppliers
                .Where(n => n.Name.ToLower() == supplierName.ToLower())
                .Select(i => i.Id)
                .FirstOrDefault();

                 purchaseDocuments = purchaseDocuments
                    .Where(s => s.SupplierId == supplierId);
            }
            var start = Convert.ToDateTime(startDate.ToShortDateString()); //startDate.Date.ToShortDateString();
            var end = Convert.ToDateTime(endDate.ToShortDateString()); //endDate.Date.ToShortDateString();

            if (start > end)
            {
                return new PurchaseCollectionQueryModel();
            }

            purchaseDocuments = purchaseDocuments
                    .Where(d => d.Date.CompareTo(startDate) > 0 && d.Date.CompareTo(endDate) < 0)
                    .OrderByDescending(d => d.Date);

            var purchases = purchaseDocuments.ProjectTo<PurchaseInvoicesViewModel>(this.mapper).ToList();

            var purchaseCollection = purchases.Skip((currentpage - 1) * invoiceperPage).Take(invoiceperPage);

            var purchaseInvoices = new PurchaseCollectionQueryModel 
            { 
                PurchaseInvoiceCollection = purchaseCollection,
                TotalPurchaseInvoices = purchases.Count
            };

            foreach (var purchase in purchaseInvoices.PurchaseInvoiceCollection)
            {
               // var customerId = dbcontext.CustomerOrders
               //      .Where(n => n.Id == purchase.CustomerOrderId)
               //      .Select(n => n.CustomerId)
               //      .FirstOrDefault();

               // purchase.CustomerName = dbcontext.Customers
               //.Where(i => i.Id == customerId)
               //.Select(n => n.Name)
               //.FirstOrDefault();

               // purchase.OrderConfirmationNumber = dbcontext.CustomerOrders
               //  .Where(i => i.Id == purchase.CustomerOrderId)
               //  .Select(o => o.OrderConfirmationNumber)
               //  .FirstOrDefault();

                purchase.SupplierName = dbcontext.Suppliers
                .Where(i => i.Id == purchase.SupplierId)
                .Select(n => n.Name).FirstOrDefault();

                purchase.SupplierOrderNumber = dbcontext.SupplierOrders
                .Where(i => i.Id == purchase.SupplierOrderId)
                .Select(n => n.Number).FirstOrDefault();                
            }

            return purchaseInvoices;
        }
        public ServiceOrdersQueryModel ServiceOrdersCollection
            (int companyId, DateTime startDate, DateTime endDate, int currentPage, int ordersPerPage)
        {            
            var queryOrders = dbcontext.ServiceOrders
                    .Where(a => a.MyCompanyId == companyId && a.Date >= startDate && a.Date <= endDate);

            var serviceOrders = queryOrders.ProjectTo<ServiceOrderCollectionDetailViewModel>(this.mapper).ToList();

            var serviceOrdersList = serviceOrders.Skip((currentPage - 1) * ordersPerPage).Take(ordersPerPage);

            foreach (var order in serviceOrdersList)
            { 
                if(order.CustomerOrderId != null)
                {
                    var customerOrder = dbcontext.CustomerOrders.Find(order.CustomerOrderId);
                    order.CustomerOrderNumber = customerOrder.OrderConfirmationNumber;
                    order.Customer = dbcontext.Customers
                        .Where(a=>a.Id == customerOrder.CustomerId)
                        .Select(a=>a.Name).FirstOrDefault();
                }
                else
                {
                    var supplierOrder = dbcontext.SupplierOrders.Find(order.SupplierOrderId);
                    order.SupplierOrderNumber = supplierOrder.Number;
                    order.Supplier = dbcontext.Suppliers
                        .Where(a => a.Id == supplierOrder.SupplierId)
                        .Select(a => a.Name).FirstOrDefault();
                }
                order.TransportCompanyName = dbcontext.TransportCompanies
                    .Where(i=>i.Id == order.TransportCompanyId)
                    .Select(n=>n.Name)
                    .FirstOrDefault();
            }

            var serviceOrdersQery = new ServiceOrdersQueryModel
            {
                TotalServiceOrders = serviceOrders.Count,
                ServiceOrders = serviceOrdersList
            };

            return serviceOrdersQery;
            
        }
    }
}
