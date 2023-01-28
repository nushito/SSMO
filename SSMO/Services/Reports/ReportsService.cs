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

        public ReportsService
            (ApplicationDbContext context, IConfigurationProvider mapper,
            IProductService productService, IInvoiceService invoiceService,
            ISupplierOrderService supplierOrderService, IMycompanyService mycompanyService,
            ICustomerOrderService customerOrderService, ISupplierService supplierService)
        {
            dbcontext = context;
            this.mapper = mapper;//.ConfigurationProvider;
            this.productService = productService;
            this.invoiceService = invoiceService;
            this.supplierOrderService = supplierOrderService;
            this.myCompanyService = mycompanyService;
            this.customerOrderService = customerOrderService;
            this.supplierService = supplierService;
        }
        public IEnumerable<CustomerOrderDetailsModel> AllCustomerOrders
            (string customerName, int currentpage, int customerOrdersPerPage)
        {

            if (String.IsNullOrEmpty(customerName))
            {
                var query = dbcontext.CustomerOrders.ProjectTo<CustomerOrderDetailsModel>(this.mapper).ToList();
                return query.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);
            }

            var customerId = dbcontext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryOrders = dbcontext.CustomerOrders
                     .Where(a => a.CustomerId == customerId);

            // var totalOrders = queryOrders.Count();  

            var orders = queryOrders.ProjectTo<CustomerOrderDetailsModel>(this.mapper).ToList();

            var customerOrdersList = orders.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            return customerOrdersList;
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
                PaidAmountStatus = corder.PaidAmountStatus,
                Origin = corder.Origin,
                StatusId = corder.StatusId,
                Vat = corder.Vat
            };

            orderForEdit.Products = (List<ProductCustomerFormModel>)productService.DetailsPerCustomerOrder(id);

            return orderForEdit;
        }

        public IEnumerable<CustomerInvoicePaymentDetailsModel> CustomersInvoicesPaymentDetails
            (string customerName, int currentpage, int customerInvoicePerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
                return new List<CustomerInvoicePaymentDetailsModel>();
            }

            var customerId = dbcontext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var customerOrdersId = dbcontext.CustomerOrders.Where(c => c.CustomerId == customerId).Select(i => i.Id).ToList();

            var queryInvoices = dbcontext.Documents
                    .Where(a => customerOrdersId.Contains(a.CustomerOrderId) || a.CustomerId == customerId && a.DocumentType == Data.Enums.DocumentTypes.Invoice);

            var invoices = queryInvoices.ProjectTo<CustomerInvoicePaymentDetailsModel>(this.mapper).ToList();

            var customerPaymentList = invoices.Skip((currentpage - 1) * customerInvoicePerPage).Take(customerInvoicePerPage);

            return customerPaymentList;
        }

        public IEnumerable<SupplierInvoicePaymentDetailsModel> SuppliersInvoicesPaymentDetails
            (string supplierName, int currentpage, int supplierInvoicePerPage)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return new List<SupplierInvoicePaymentDetailsModel>();
            }

            var supplierId = dbcontext.Suppliers
                .Where(n => n.Name.ToLower() == supplierName.ToLower())
                .Select(i => i.Id)
                .FirstOrDefault();


            var purchaseInvoices = dbcontext.Documents
                .Where(n => n.SupplierId == supplierId && n.DocumentType == Data.Enums.DocumentTypes.Purchase);

            var purchases = purchaseInvoices.ProjectTo<SupplierInvoicePaymentDetailsModel>(mapper).ToList();
            var purchasePaymentList = purchases.Skip((currentpage - 1) * supplierInvoicePerPage).Take(supplierInvoicePerPage);

            return purchasePaymentList;
        }
        public CustomerOrderDetailsModel CustomerOrderDetails(int id)
        {
            var findorder = dbcontext.CustomerOrders.Where(a => a.Id == id);
            CustomerOrderDetailsModel order = findorder.ProjectTo<CustomerOrderDetailsModel>(mapper).FirstOrDefault();
            var supplierOrderDetail = dbcontext.SupplierOrders
                .Where(o => o.CustomerOrderId == id)
                .Select(a => new
                {
                    supplierNumber = a.Number,
                    supplierId = a.SupplierId
                }).FirstOrDefault();

            var myCompanyId = findorder.
                Select(id => id.MyCompanyId).
                FirstOrDefault();

            var myCompanyName = dbcontext.MyCompanies
                .Where(id => id.Id == myCompanyId)
                .Select(n => n.Name)
                .FirstOrDefault();

            order.SupplierOrderNumber = supplierOrderDetail.supplierNumber;
            var supplierName = dbcontext.Suppliers
                .Where(id => id.Id == supplierOrderDetail.supplierId)
                .Select(n => n.Name)
                .FirstOrDefault();
            order.SupplierName = supplierName;
            order.MyCompanyName = myCompanyName;
            return (CustomerOrderDetailsModel)order;
        }

        public bool EditCustomerOrder(int id,
            string number, System.DateTime date,
            int myCompanyId, string deliveryTerms,
            string loadingPlace, string deliveryAddress,
            int currencyId, int status, string fscClaim,
            string fscCertificate, decimal paidAdvance, bool paidStatus,
             List<ProductCustomerFormModel> products)
        {
            var order = dbcontext.CustomerOrders.Find(id);

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
            order.PaidAvance = paidAdvance;
            order.PaidAmountStatus = paidStatus;
            order.Amount = 0;

            var productsPerCorder = dbcontext.Products.Where(co => co.CustomerOrderId == order.Id).ToList();

            if (products.Count != 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    var descriptionId = dbcontext.Descriptions
                        .Where(n => n.Name == products[i].Description)
                        .Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).DescriptionId = descriptionId;
                    var gradeId = dbcontext.Grades.Where(n => n.Name == products[i].Grade).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).GradeId = gradeId;
                    var sizeId = dbcontext.Sizes.Where(n => n.Name == products[i].Size).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).SizeId = sizeId;
                    productsPerCorder.ElementAt(i).Price = products.ElementAt(i).Price;
                    productsPerCorder.ElementAt(i).Pallets = products.ElementAt(i).Pallets;
                    productsPerCorder.ElementAt(i).SheetsPerPallet = products.ElementAt(i).SheetsPerPallet;
                    productsPerCorder.ElementAt(i).Unit = Enum.Parse<Unit>(products.ElementAt(i).Unit.ToString());

                    var sizeName = dbcontext.Sizes
                        .Where(a => a.Id == sizeId).Select(n => n.Name).FirstOrDefault();
                    var dimensionArray = sizeName.Split('/').ToArray();
                    var countArray = dimensionArray.Count();
                    decimal sum = 1M;

                    for (int j = 0; j < countArray; j++)
                    {
                        sum *= Math.Round(decimal.Parse(dimensionArray[j]) / 1000, 7);
                    }

                    productsPerCorder.ElementAt(i).TotalSheets = products.ElementAt(i).Pallets * products.ElementAt(i).SheetsPerPallet;

                    if (products.ElementAt(i).QuantityM3 != 0)
                    {
                        productsPerCorder.ElementAt(i).OrderedQuantity = products.ElementAt(i).QuantityM3;
                    }
                    else
                    {
                        productsPerCorder.ElementAt(i).OrderedQuantity = Math.Round(sum * productsPerCorder.ElementAt(i).TotalSheets, 4);
                    }

                    productsPerCorder.ElementAt(i).Amount = Math.Round
                        (productsPerCorder.ElementAt(i).Price * productsPerCorder.ElementAt(i).OrderedQuantity, 4);

                    order.Amount += productsPerCorder.ElementAt(i).Amount;
                }

                order.SubTotal = order.Amount * order.Vat / 100 ?? 0;
                order.TotalAmount = order.Amount + order.SubTotal;
                order.Balance = order.TotalAmount - paidAdvance;
            }

            dbcontext.SaveChanges();

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

        public IEnumerable<CustomerOrderDetailsPaymentModel> CustomerOrdersPaymentDetails(string customerName, int currentpage, int customerOrdersPerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
                return new List<CustomerOrderDetailsPaymentModel>();
            }

            var customerId = dbcontext.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var customerOrders = dbcontext.CustomerOrders.Where(c => c.CustomerId == customerId);

            var incustomerOrdersCollectionvoices = customerOrders.ProjectTo<CustomerOrderDetailsPaymentModel>(this.mapper).ToList();

            var customerOrdersPaymentList = incustomerOrdersCollectionvoices.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            return customerOrdersPaymentList;

        }

        public IEnumerable<SupplierOrderDetailsModel> AllSupplierOrders(string name, int currentpage, int supplierOrdersPerPage)
        {
            if (String.IsNullOrEmpty(name))
            {
                return new List<SupplierOrderDetailsModel>();
            }

            var supplierId = dbcontext.Suppliers.Where(a => a.Name.ToLower() == name.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryOrders = dbcontext.SupplierOrders
                    .Where(a => a.SupplierId == supplierId);

            //   var customerOrdersNumber = queryOrders.Select(num => num.CustomerOrderId);

            // var totalOrders = queryOrders.Count();  

            var supplierOrders = queryOrders.ProjectTo<SupplierOrderDetailsModel>(this.mapper).ToList();
            foreach (var order in supplierOrders)
            {
                order.CustomerOrderConfirmationNumber = dbcontext.CustomerOrders
                    .Where(id => id.Id == order.CustomerOrderId)
                    .Select(num => num.OrderConfirmationNumber)
                    .FirstOrDefault();
            }

            var supplierOrdersList = supplierOrders.Skip((currentpage - 1) * supplierOrdersPerPage).Take(supplierOrdersPerPage);

            return supplierOrdersList;
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
                CustomerOrderId = supplierOrder.CustomerOrderId,
                Date = supplierOrder.Date,
                DatePaidAmount = supplierOrder.DatePaidAmount,
                DeliveryAddress = supplierOrder.DeliveryAddress,
                LoadingAddress = supplierOrder.LoadingAddress,
                DeliveryTerms = supplierOrder.DeliveryTerms,
                FSCClaim = supplierOrder.FSCClaim,
                FSCSertificate = supplierOrder.FSCSertificate,
                GrossWeight = supplierOrder.GrossWeight,
                TotalQuantity = supplierOrder.TotalQuantity,
                MyCompanyId = supplierOrder.MyCompanyId,
                NetWeight = supplierOrder.NetWeight,
                VAT = supplierOrder.VAT
            };

            return supplierOrderForEdit;
        }

        public bool EditSupplierOrder
            (int supplierOrderId, string supplierOrderNumber,
            DateTime date, int myCompanyId, string deliveryTerms,
            string loadingPlace, string deliveryAddress,
            decimal grossWeight, decimal netWeight, int currencyId,
            int status, int customerOrderNumber, string fscClaim, string fscCertificate,
            decimal paidAdvance, bool paidStatus, int? vat, List<ProductsForEditSupplierOrder> products)
        {
            if (supplierOrderNumber == null) return false;

            var supplierOrder = dbcontext.SupplierOrders
                .Where(num => num.Number.ToLower() == supplierOrderNumber.ToLower())
                .FirstOrDefault();

            var customerOrderId = dbcontext.CustomerOrders
                .Where(num => num.OrderConfirmationNumber == customerOrderNumber)
                .Select(id => id.Id)
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
            supplierOrder.CustomerOrderId = customerOrderId;
            supplierOrder.FSCClaim = fscClaim;
            supplierOrder.FSCSertificate = fscCertificate;
            supplierOrder.PaidStatus = paidStatus;
            supplierOrder.Amount = 0;



            var productsPerCorder = dbcontext.Products.Where(co => co.CustomerOrderId == customerOrderId).ToList();

            if (products.Count != 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    var descriptionId = dbcontext.Descriptions.Where(n => n.Name == products[i].Description).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).DescriptionId = descriptionId;
                    var gradeId = dbcontext.Grades.Where(n => n.Name == products[i].Grade).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).GradeId = gradeId;
                    var sizeId = dbcontext.Sizes.Where(n => n.Name == products[i].Size).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).SizeId = sizeId;
                    productsPerCorder.ElementAt(i).PurchasePrice = products.ElementAt(i).PurchasePrice;
                    productsPerCorder.ElementAt(i).Pallets = products.ElementAt(i).Pallets;
                    productsPerCorder.ElementAt(i).SheetsPerPallet = products.ElementAt(i).SheetsPerPallet;
                    productsPerCorder.ElementAt(i).Unit = Enum.Parse<Unit>(products.ElementAt(i).Unit);

                    var sizeName = dbcontext.Sizes
                        .Where(a => a.Id == sizeId).Select(n => n.Name).FirstOrDefault();
                    var dimensionArray = sizeName.Split('/').ToArray();
                    var countArray = dimensionArray.Count();
                    decimal sum = 1M;

                    for (int j = 0; j < countArray; j++)
                    {
                        sum *= Math.Round(decimal.Parse(dimensionArray[j]) / 1000, 4);
                    }

                    productsPerCorder.ElementAt(i).TotalSheets = products.ElementAt(i).Pallets * products.ElementAt(i).SheetsPerPallet;

                    if (products.ElementAt(i).QuantityM3 != 0)
                    {
                        productsPerCorder.ElementAt(i).OrderedQuantity = products.ElementAt(i).QuantityM3;
                    }
                    else
                    {
                        productsPerCorder.ElementAt(i).OrderedQuantity = Math.Round(sum * productsPerCorder.ElementAt(i).TotalSheets, 4);
                    }

                    productsPerCorder.ElementAt(i).Amount = Math.Round
                        (productsPerCorder.ElementAt(i).PurchasePrice * productsPerCorder.ElementAt(i).OrderedQuantity, 4);

                    supplierOrder.Amount += productsPerCorder.ElementAt(i).Amount;
                }
                supplierOrderService.TotalAmountAndQuantitySum(supplierOrderId);
            }

            dbcontext.SaveChanges();
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

            supplierOrderDetail.MyCompanyName = myCompanyName;

            var supplierId = supplierOrder.Select(id => id.SupplierId).FirstOrDefault();
            supplierOrderDetail.SupplierName = dbcontext.Suppliers
                .Where(i => i.Id == supplierId)
                .Select(n => n.Name)
                .FirstOrDefault();

            var customerId = dbcontext.CustomerOrders
                .Where(i => i.Id == supplierOrderDetail.CustomerOrderId)
                .Select(c => c.CustomerId)
                .FirstOrDefault();

            supplierOrderDetail.CustomerName = dbcontext.Customers
                 .Where(i => i.Id == customerId)
                 .Select(n => n.Name)
                 .FirstOrDefault();

            supplierOrderDetail.StatusName = dbcontext.Statuses
                .Where(i => i.Id == supplierOrderDetail.StatusId)
                .Select(n => n.Name)
                .FirstOrDefault();

            supplierOrderDetail.CustomerOrderConfirmationNumber = dbcontext.CustomerOrders
                .Where(id => id.Id == supplierOrderDetail.CustomerOrderId)
                .Select(num => num.OrderConfirmationNumber)
                .FirstOrDefault();

            return supplierOrderDetail;
        }

        public IEnumerable<InvoiceCollectionViewModel> InvoiceCollection(string myCompanyName, int currentpage, int invoicesPerPage)
        {
            if (myCompanyName == null) return new List<InvoiceCollectionViewModel>();

            var companyId = myCompanyService.GetMyCompanyId(myCompanyName);

            var invoices = dbcontext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .Where(m => m.MyCompanyId == companyId);
            // .ToList();

            var invoiceDetailsCollection = invoices.ProjectTo<InvoiceCollectionViewModel>(this.mapper).ToList();

            foreach (var invoice in invoiceDetailsCollection)
            {
                var customerOrderNumber = customerOrderService.CustomerOrderNumberById(invoice.CustomerOrderId);
                var customerName = dbcontext.Customers
                    .Where(i => i.Id == invoice.CustomerId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                var supplierName = supplierService.SupplierNameById(invoice.SupplierOrderId);
                invoice.SupplierName = supplierName;
                invoice.OrderConfirmationNumber = customerOrderNumber;
                invoice.CustomerName = customerName;
            }

            var invoiceCollection = invoiceDetailsCollection.Skip((currentpage - 1) * invoicesPerPage).Take(invoicesPerPage);

            return invoiceCollection;
        }

        public InvoiceDetailsViewModel InvoiceDetails(int id)
        {
            var invoice = dbcontext.Documents
                .Where(i => i.Id == id);

            var invoiceDetails = invoice.ProjectTo<InvoiceDetailsViewModel>(mapper).FirstOrDefault();

            if (invoice == null) return null;

            var products = dbcontext.Products
                .Where(inv => inv.DocumentId == id);

            var productsDetails = products.ProjectTo<InvoiceProductsDetailsViewModel>(mapper).ToList();

            foreach (var product in productsDetails)
            {
                product.Description = productService.GetDescriptionName(product.DescriptionId);
                product.Size = productService.GetSizeName(product.SizeId);
                product.Grade = productService.GetGradeName(product.GradeId);
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

            var bankDetails = dbcontext.BankDetails
                .Where(i => i.CompanyId == invoiceDetails.MyCompanyId);

            var invoiceBankDetails = bankDetails.ProjectTo<InvoiceBankDetailsModel>(mapper).ToList();

            foreach (var bankDetail in invoiceBankDetails)
            {
                bankDetail.CurrencyName = dbcontext.Currencys
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

            return invoiceDetails;
        }

        ICollection<PurchaseInvoicesViewModel> IReportsService.PurchaseInvoices
            (string supplierName, DateTime startDate, DateTime endDate, int invoiceperPage, int currentpage)
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
            var start = startDate.Date.ToShortDateString();
            var end = endDate.Date.ToShortDateString();

            //TODO Why in the view dates are 1/1/0001 
            if (start != "1/1/0001" && end != "1/1/0001")
            {
                purchaseDocuments = purchaseDocuments
                    .Where(d => d.Date.CompareTo(startDate) > 0 && d.Date.CompareTo(endDate) < 0)
                    .OrderByDescending(d => d.Date);
            }

            var purchaseInvoices = new List<PurchaseInvoicesViewModel>();

            foreach (var invoice in purchaseDocuments.ToList())
            {
                var customerId = dbcontext.CustomerOrders
                     .Where(n => n.Id == invoice.CustomerOrderId)
                     .Select(n => n.CustomerId)
                     .FirstOrDefault();
                var purchaseInvoice = new PurchaseInvoicesViewModel
                {
                    Id = invoice.Id,
                    CustomerName = dbcontext.Customers
                   .Where(i => i.Id == customerId)
                   .Select(n => n.Name)
                   .FirstOrDefault(),
                    OrderConfirmationNumber = dbcontext.CustomerOrders
                     .Where(i => i.Id == invoice.CustomerOrderId)
                     .Select(o => o.OrderConfirmationNumber)
                     .FirstOrDefault(),
                    Date = invoice.Date,
                    GrossWeight = invoice.GrossWeight,
                    Incoterms = invoice.Incoterms,
                    Number = invoice.PurchaseNumber,
                    NetWeight = invoice.NetWeight,
                    TruckNumber = invoice.TruckNumber,
                    Swb = invoice.Swb,
                    TotalAmount = invoice.TotalAmount,
                    SupplierName = dbcontext.Suppliers
                    .Where(i => i.Id == invoice.SupplierId)
                    .Select(n => n.Name).FirstOrDefault(),
                    SupplierOrderNumber = dbcontext.SupplierOrders
                    .Where(i => i.Id == invoice.SupplierOrderId)
                    .Select(n => n.Number).FirstOrDefault()
                };

                purchaseInvoices.Add(purchaseInvoice);
            }
            return purchaseInvoices;
        }


    }
}
