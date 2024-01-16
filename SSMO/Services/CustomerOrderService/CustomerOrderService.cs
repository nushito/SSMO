using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Products;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Repository;
using SSMO.Services.MyCompany;
using SSMO.Services.Products;
using SSMO.Services.TransportService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.CustomerOrderService
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        private readonly IProductService productService;
        private readonly IProductRepository productRepository;
        private readonly IMycompanyService mycompanyService;
        public CustomerOrderService(ApplicationDbContext dbContext, IConfigurationProvider mapper,
            IProductService productService,IProductRepository productRepository,
            IMycompanyService mycompanyService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productService;
            this.productRepository = productRepository;
            this.mycompanyService = mycompanyService;
        }

        public bool CheckOrderNumberExist(int number)
        {
            if(dbContext.CustomerOrders.Any(o => o.OrderConfirmationNumber == number))
            {
                return true;
            }
            return false;
        }

        public async Task<int> CreateOrder(string num, DateTime date, int customerId, int company, string deliveryTerms,
            string loadingAddress, string deliveryAddress,int currency,string origin, 
            int vat, int statusId, List<int> supplierOrders, string comment, 
            List<int> banks, string type, int? fiscalAgentId, string dealType, string dealDescription, 
            int? fscText, string paymentTerms, string eta, string etd)
        {
           
            var fscClaim = dbContext.MyCompanies
                .Where(a => a.Id == company)
                .Select(a => a.FscClaim).FirstOrDefault();

            var fscCertificate = dbContext.MyCompanies
               .Where(a => a.Id == company)
               .Select(a => a.FscSertificate).FirstOrDefault();

            var banksForOrder = dbContext.BankDetails
                .Include(a=>a.CustomerOrders)
                .Where(i=>banks.Contains(i.Id))
                .ToList();

            var lastConfirmationNumber = dbContext.CustomerOrders
                .OrderBy(a=>a.OrderConfirmationNumber)
                .Select(oc=>oc.OrderConfirmationNumber)
                .LastOrDefault();

            var order = new SSMO.Data.Models.CustomerOrder
            {
                OrderConfirmationNumber = lastConfirmationNumber + 1,
                CustomerPoNumber = num,
                Date = date,
                CustomerId = customerId,
                MyCompanyId = company,
                DeliveryTerms = deliveryTerms,
                LoadingPlace = loadingAddress,
                DeliveryAddress = deliveryAddress,
                FSCClaim = fscClaim,
                FSCSertificate = fscCertificate,
                CurrencyId = currency,
                StatusId = statusId,
                Origin = origin,               
                Vat = vat,
                SupplierOrders = new List<SupplierOrder>(),
                Payments = new List<Payment>(),
                Comment = comment,
                BankDetails= new List<BankDetails>(),
                Type= type,
                DealType= dealType,
                DealDescription= dealDescription,
                PaymentTerms= paymentTerms,
                Eta = eta,
                Etd = etd
            };

            if (fiscalAgentId != null)
            {
                order.FiscalAgentId = fiscalAgentId;
            }

            if (fscText != null)
            {
                order.FscTextId = fscText;
            }

            var supplierOrdersList = dbContext.SupplierOrders
                .Where(i => supplierOrders.Contains(i.Id))
                .ToList();

            order.SupplierOrders = supplierOrdersList;

            banksForOrder.ForEach(i => order.BankDetails.Add(i));
           
           await dbContext.CustomerOrders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            return order.Id;
        }

        public void CustomerOrderCounting(int customerorderId)
        {
            var thisorder = OrderPerIndex(customerorderId);

            thisorder.SubTotal = thisorder.Amount * thisorder.Vat / 100 ?? 0;

            thisorder.TotalAmount = (decimal)(thisorder.Amount + thisorder.SubTotal);
           
            thisorder.TotalQuantity = thisorder.CustomerOrderProducts.Sum(a=>a.Quantity);
            thisorder.TotalPallets = thisorder.CustomerOrderProducts.Sum(a => a.Pallets);
            thisorder.TotalSheets = thisorder.CustomerOrderProducts.Sum(a => a.TotalSheets);

            dbContext.SaveChanges();
        }
        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id)
        {
            return dbContext.CustomerOrders.Where(a => a.Id == id).FirstOrDefault();
        }
        public Data.Models.CustomerOrder OrderPerNumber(int number)
        {
            return dbContext.CustomerOrders.Where(a => a.OrderConfirmationNumber == number).FirstOrDefault();
        }
        public bool AnyCustomerOrderExist()
        {
            var anyCustomerOrders = dbContext.CustomerOrders.Any();
            if (!anyCustomerOrders)
            {
                return false;
            }

            return true;
        }
        public async Task<int> CreateFirstOrder(int number, string num, DateTime date, 
            int customerId, int company, string deliveryTerms, string loadingAddress, 
            string deliveryAddress, int currency, string origin, 
           int vat, int statusId, List<int> supplierOrders, string comment, 
           List<int> banks, string type, int? fiscalAgentId, string dealType, string dealDescription, 
           int? fscText, string paymentTerms, string eta, string etd)
        {
            var fscClaim = dbContext.MyCompanies
                 .Where(a => a.Id == company)
                 .Select(a => a.FscClaim).FirstOrDefault();

            var fscCertificate = dbContext.MyCompanies
               .Where(a => a.Id == company)
               .Select(a => a.FscSertificate).FirstOrDefault();

            var banksForOrder = dbContext.BankDetails
                .Include(c=>c.CustomerOrders)
               .Where(i => banks.Contains(i.Id))
               .ToList();

            var order = new SSMO.Data.Models.CustomerOrder
            {
                OrderConfirmationNumber = number,
                CustomerPoNumber = num,
                Date = date,
                CustomerId = customerId,
                MyCompanyId = company,
                DeliveryTerms = deliveryTerms,
                LoadingPlace = loadingAddress,
                DeliveryAddress = deliveryAddress,
                FSCClaim = fscClaim,
                FSCSertificate = fscCertificate,
                CurrencyId = currency,
                StatusId = statusId,
                Origin = origin,                
                Vat = vat,
                SupplierOrders = new List<SupplierOrder>(),
                Comment = comment,
                BankDetails = new List<BankDetails>(),
                Type = type,
                DealType = dealType,
                DealDescription = dealDescription,
                PaymentTerms= paymentTerms,
                Eta = eta,
                Etd = etd
            };

            banksForOrder.ForEach(i => order.BankDetails.Add(i));

            if (fiscalAgentId != null)
            {
                order.FiscalAgentId = fiscalAgentId;
            }

            if(fscText!= null)
            {
                order.FscTextId = fscText;
            }

            var supplierOrdersList = dbContext.SupplierOrders
                .Where(i => supplierOrders.Contains(i.Id))
                .ToList();

            order.SupplierOrders = supplierOrdersList;

           await dbContext.CustomerOrders.AddAsync(order);
           await dbContext.SaveChangesAsync();
            return order.Id;
        }
        public ICollection<CustomerOrderForInvoiceViewModel> AllCustomerOrderNumbers()
        {
            return dbContext.CustomerOrders
                .OrderByDescending(n=>n.OrderConfirmationNumber)
                .Select(a => new CustomerOrderForInvoiceViewModel 
                {
                    Id= a.Id,
                    OrderConfirmationNumber= a.OrderConfirmationNumber,                
                })
                .ToList();
        }
        public EditCustomerOrderPaymentModel GetCustomerOrderPaymentForEdit(int orderConfirmationNumber)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(num => num.OrderConfirmationNumber == orderConfirmationNumber);

            var customerOrderforEdit = customerOrder.ProjectTo<EditCustomerOrderPaymentModel>(mapper).FirstOrDefault();

            return customerOrderforEdit;
        }
        public bool EditCustomerOrdersPayment(int orderConfirmationNumber, bool paidStatus, decimal paidAdvance, DateTime date)
        {
            if (orderConfirmationNumber == 0)
            {
                return false;
            }

            var customerOrder = dbContext.CustomerOrders
                .Where(num => num.OrderConfirmationNumber == orderConfirmationNumber)
                .FirstOrDefault();

            var invoices = dbContext.Documents
                .Where(a=>a.CustomerOrders.Select(i=>i.Id).Contains(customerOrder.Id))
                .ToList();
           
            customerOrder.PaidAmountStatus = paidStatus;            
            customerOrder.Balance = customerOrder.TotalAmount - customerOrder.PaidAvance;

            var newPayment = new Payment
            {
                 PaidAmount = paidAdvance,
                 Date= DateTime.Now,
                 CustomerOrderId = customerOrder.Id,
                 CurrencyId = customerOrder.CurrencyId
            };
                       
            if (customerOrder.Balance == 0)
            {
                customerOrder.PaidAmountStatus = true;     
                if(invoices != null)
                {
                    invoices.ForEach(a=>a.PaidStatus = true);
                }
            }
            else
            {
                customerOrder.PaidAmountStatus = false;
            }

            dbContext.Payments.Add(newPayment);
            customerOrder.Payments.Add(newPayment);
            dbContext.SaveChanges();

            return true;
        }

        public int CustomerOrderNumber(int supplierOrderId)
        {
            var customerOrderId = dbContext.SupplierOrders
                .Where(id => id.Id == supplierOrderId)
                .Select(cnum => cnum.CustomerOrderId)
                .FirstOrDefault();

            var customerOrdeNum = dbContext.CustomerOrders
                .Where(id => id.Id == customerOrderId)
                .Select(num => num.OrderConfirmationNumber)
                .FirstOrDefault();

            return customerOrdeNum;
        }

        public int CustomerOrderNumberById(int id)
        {
           return dbContext.CustomerOrders
                .Where(i=>i.Id == id)
                .Select(n=>n.OrderConfirmationNumber)
                .FirstOrDefault();
        }
        public List<CustomerOrdersJsonList> CustomerOrderCollection
            (int customerId,int mycompanyId)
        {
            return dbContext.CustomerOrders
                .Where(c=>c.CustomerId == customerId && c.MyCompanyId == mycompanyId && c.CustomerOrderProducts.Where(a=>a.AutstandingQuantity > 0.01M).Count() > 0)
                .Select(i=> new CustomerOrdersJsonList
                {
                    CustomerOrderId = i.Id,
                    CustomerOrderNumber = i.OrderConfirmationNumber.ToString()
                })
                .ToList();
        }
        public void CheckCustomerOrderStatus(int id)
        {
            var customerOrderAustandingQuantity = dbContext.CustomerOrderProductDetails
                .Where(ic=>ic.CustomerOrderId == id)
                .Select(a=>a.AutstandingQuantity).ToList();

            var customerOrder = dbContext.CustomerOrders
                .Where(i => i.Id == id)                
                .FirstOrDefault();
           
            if(customerOrderAustandingQuantity.Sum() <= 0.001m)           
            {
                customerOrder.StatusId = dbContext.Statuses
                    .Where(n=>n.Name == "Finished")
                    .Select(i=>i.Id)
                    .FirstOrDefault();
            }

        }

        public ICollection<CustomerOrderNumbersByCustomerViewModel> CustomerOrderNumbersPerInvoice(int id)
        {
            return dbContext.Documents
               .Where(c => c.Id == id)
               .Select(i => new CustomerOrderNumbersByCustomerViewModel
               {
                   Id = i.CustomerOrders.Select(i=>i.Id).FirstOrDefault(),
                   OrderConfirmationNumber = i.CustomerOrders.Select(i => i.OrderConfirmationNumber).FirstOrDefault(),
               })
               .ToList();
        }

        public ICollection<BankDetailsViewModel> GetBanks()
        {
            var banks = dbContext.BankDetails
                .Select(a => new BankDetailsViewModel
                {
                    BankName = a.BankName,
                    Iban = a.Iban,
                    Id = a.Id,
                    CurrencyId = a.CurrencyId,
                    MyCompanyName = mycompanyService.GetCompanyName(a.CompanyId)
                }).ToList();

            foreach (var bank in banks)
            {
                bank.CurrencyName = dbContext.Currencies
                    .Where(i=>i.Id == bank.CurrencyId)
                    .Select(i=>i.Name) .FirstOrDefault();
            }

            return banks;
        }

        public CustomerOrderPrintViewModel GetCustomerOrderPrint(int id)
        {
            var order = dbContext.CustomerOrders
                .Where(i => i.Id == id)
                .Include(b => b.BankDetails)
                .FirstOrDefault();
              
            if (order == null)
            {
                return null;
            }
            var currency = dbContext.Currencies
                .Where(i=>i.Id == order.CurrencyId)
                .Select(order=> order.Name) .FirstOrDefault();

            var model = new CustomerOrderPrintViewModel()
            {
                Amount = order.Amount,
                Comment = order.Comment,
                CustomerPoNumber = order.CustomerPoNumber,
                Date = order.Date,
                DeliveryAddress = order.DeliveryAddress,
                DeliveryTerms = order.DeliveryTerms,
                LoadingPlace = order.LoadingPlace,
                OrderConfirmationNumber = order.OrderConfirmationNumber,
                Origin = order.Origin,
                SubTotal = order.SubTotal,
                TotalAmount = order.TotalAmount,
                Type = order.Type,
                TotalQuantity = order.TotalQuantity,
                Vat = order.Vat,
                DealType = order.DealType,
                DealDescription = order.DealDescription,
                BankDetails = new List<BankDetailsViewModel>(),
                PaymentTerms = order.PaymentTerms,
                Eta = order.Eta,
                Currency = currency
            };

            var myCompany = dbContext.MyCompanies
                .Where(i=>i.Id == order.MyCompanyId)
                .FirstOrDefault();

            if(order.FscTextId != null)
            {
                model.FscText = dbContext.FscTexts
                    .Where(i=>i.Id == order.FscTextId)
                    .Select(n=>n.FscTextEng)
                    .FirstOrDefault();
            }

            model.FscCertificate = myCompany.FscSertificate;

            var companyAddress = dbContext.Addresses
                .Where(I => I.Id == myCompany.AddressId)
                .FirstOrDefault();

            var customer = dbContext.Customers
                .Where(i=>i.Id== order.CustomerId) 
                .FirstOrDefault();

            var customerAddress = dbContext.Addresses.
                Where(i=>i.Id == customer.AddressId)
                .FirstOrDefault();

            model.Customer = new AddCustomerFormModel
            {
                BgCustomerName = customer.Name,
                Name = customer.Name,
                EIK = customer.EIK,
                VAT = customer.VAT,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                RepresentativePerson = customer.RepresentativePerson,
                Country = customerAddress.Country,
                City = customerAddress.City,
                Street = customerAddress.Street
            };
          
            model.MyCompany = new MyCompanyDetailsPrintViewModel
            {
                MyCompanyName = myCompany.Name,
                Eik = myCompany.Eik,
                RepresentativePerson = myCompany.RepresentativePerson,
                Vat = myCompany.VAT,
                City = companyAddress.City,
                Country= companyAddress.Country,
                Street= companyAddress.Street,
                FscCertificate = myCompany.FscSertificate
            };

            foreach (var bankDetail in order.BankDetails)
            {
                model.BankDetails.Add(new BankDetailsViewModel
                {
                    BankName = bankDetail.BankName,
                    Iban = bankDetail.Iban,
                    Swift = bankDetail.Swift,
                    CurrencyName = dbContext.Currencies
                    .Where(i=>i.Id == bankDetail.CurrencyId)
                    .Select(n=>n.Name).FirstOrDefault()
                });
            }

            model.Products = dbContext.CustomerOrderProductDetails
                .Where(i=>i.CustomerOrderId == order.Id)
                .ProjectTo<ProductCustomerFormModel>(mapper)
                .ToList();

            foreach (var product in model.Products)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                product.Description = productService.GetDescriptionName(mainProduct.DescriptionId);
                product.Size = productService.GetSizeName(mainProduct.SizeId);
                product.Grade = productService.GetGradeName(mainProduct.GradeId);
                product.HsCode = mainProduct.HsCode;
                if(product.Unit == Data.Enums.Unit.pcs.ToString() || product.Unit == Data.Enums.Unit.sheets.ToString())
                {
                    product.Quantity = Math.Round(product.Quantity, 0);
                }
            }

            if(order.FiscalAgentId != null)
            {
                model.FiscalAgent = dbContext.FiscalAgents
                    .Where(i => i.Id == order.FiscalAgentId)
                    .Select(n => new FiscalAgentPrintViewModel
                    {
                         Details = n.Details,
                         Name = n.Name
                    }).FirstOrDefault();                 
            };

           return model;
        }

        public ICollection<CustomerOrderJsonListForServiceOrder> CustomerOrdersForService(int id)
        {
            return dbContext.CustomerOrders
                .Where(i=>i.CustomerId == id)
                .Select(n=> new CustomerOrderJsonListForServiceOrder
                {
                    CustomerOrderId = n.Id,
                    CustomerOrderNumber = n.OrderConfirmationNumber
                }).ToList();
        }
    }
}
