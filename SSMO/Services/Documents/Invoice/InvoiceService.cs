using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Documents.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductService productService;

        public InvoiceService(ApplicationDbContext dbContext, IMapper mapper, IProductService productSevice)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productSevice;
        }

        public bool CheckFirstInvoice()
        {
            var invoiceCheck = dbContext.Documents.Where(a => a.DocumentType == Data.Enums.DocumentTypes.Invoice).Any();
            if (!invoiceCheck)
            {
                return false;
            }
            return true;
        }

        public InvoicePrintViewModel CreateInvoice(
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN, int number, string myCompanyName, string truckNumber)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(on => on.OrderConfirmationNumber == orderConfirmationNumber).FirstOrDefault();

            var supplierOdrerForThisInvoiceId = dbContext.SupplierOrders.Where(c => c.CustomerOrderId == customerOrder.Id)
                .Select(i => i.Id).FirstOrDefault();

            var productList = dbContext.Products.
                Where(co => co.CustomerOrderId == customerOrder.Id).ToList();

            if (customerOrder == null)
            {
                return null;
            }

            var invoiceCreate = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.Invoice,
                Amount = customerOrder.Amount,
                TotalAmount = customerOrder.TotalAmount,
                Vat = customerOrder.Vat,
                Balance = customerOrder.Balance,
                CurrencyExchangeRateUsdToBGN = currencyExchangeRateUsdToBGN,
                CustomerOrderId = customerOrder.Id,
                Date = date,
                PaidStatus = customerOrder.PaidAmountStatus,
                Products = productList,
                SupplierOrderId = supplierOdrerForThisInvoiceId,
                TruckNumber = truckNumber,
                Incoterms = customerOrder.DeliveryTerms,
                CustomerId = customerOrder.CustomerId,
                NetWeight = customerOrder.NetWeight,
                GrossWeight = customerOrder.GrossWeight
            };

            if (CheckFirstInvoice())
            {
                var lastInvoiceNumber = dbContext.Documents.Where(n => n.DocumentType == Data.Enums.DocumentTypes.Invoice)
                    .OrderBy(n => n.DocumentNumber)
                    .Select(n => n.DocumentNumber).LastOrDefault();

                invoiceCreate.DocumentNumber = lastInvoiceNumber + 1;
            }
            else
            {
                invoiceCreate.DocumentNumber = number;
            }

            var invoiceForPrint = this.mapper.Map<InvoicePrintViewModel>(invoiceCreate);

            var customerId = dbContext.CustomerOrders.Where(i => i.Id == customerOrder.Id)
                .Select(c => c.CustomerId).FirstOrDefault();

            var customerDetails = dbContext.Customers.Where(i => i.Id == customerId).Select(a => new CustomerForInvoicePrint
            {
                Name = a.Name,
                EIK = a.EIK,
                RepresentativePerson = a.RepresentativePerson,
                VAT = a.VAT,
                ClientAddress = new AddressCustomerForInvoicePrint
                {
                    City = a.ClientAddress.City,
                    Country = a.ClientAddress.Country,
                    Street = a.ClientAddress.Street
                }
            }).FirstOrDefault();

            invoiceForPrint.Customer = customerDetails;
            invoiceForPrint.OrderConfirmationNumber = orderConfirmationNumber;

            var myCompany = dbContext.MyCompanies.Where(n => n.Name.ToLower() == myCompanyName.ToLower()).FirstOrDefault();
            var addressCompany = dbContext.Addresses.Where(co => co.Id == myCompany.AddressId).FirstOrDefault();
            invoiceCreate.MyCompanyId = myCompany.Id;
            invoiceForPrint.Seller = new MyCompanyForInvoicePrint
            {
                Name = myCompany.Name,
                City = addressCompany.City,
                Country = addressCompany.Country,
                Street = addressCompany.Street,
                EIK = myCompany.Eik,
                FSCClaim = myCompany.FSCClaim,
                FSCSertificate = myCompany.FSCSertificate,
                RepresentativePerson = myCompany.RepresentativePerson,
                VAT = myCompany.VAT
            };
            invoiceForPrint.Products = this.mapper.Map<ICollection<ProductsForInvoiceModel>>(productList);

            foreach (var product in invoiceForPrint.Products)
            {
                product.Description = productService.GetDescriptionName(product.DescriptionId);
                product.Grade = productService.GetGradeName(product.GradeId);
                product.Size = productService.GetSizeName(product.SizeId);
            }

            invoiceForPrint.VatAmount = customerOrder.Amount * customerOrder.Vat / 100 ?? 0;

            dbContext.Documents.Add(invoiceCreate);
            dbContext.SaveChanges();

            CreatePackingListForThisInvoice(invoiceCreate.Id);

            return invoiceForPrint;
        }

        private void CreatePackingListForThisInvoice(int id)
        {
            var invoice = dbContext.Documents
                .Where(i => i.Id == id)
                .FirstOrDefault();
            if (invoice == null) return;

            var packingList = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.PackingList,
                DocumentNumber = invoice.DocumentNumber,
                Date = invoice.Date,
                CustomerId = invoice.CustomerId,
                CustomerOrderId = invoice.CustomerOrderId,
                Products = invoice.Products,
                TruckNumber = invoice.TruckNumber,
                Incoterms = invoice.Incoterms,
                MyCompanyId = invoice.MyCompanyId,
                NetWeight = invoice.NetWeight,
                GrossWeight = invoice.GrossWeight,
                SupplierOrderId = invoice.SupplierOrderId
            };

            dbContext.Documents.Add(packingList);
            dbContext.SaveChanges();
        }

        public EditInvoicePaymentModel InvoiceForEditById(int id)
        {
            return dbContext.Documents
                .Where(i => i.Id == id)
                .Select(n => new EditInvoicePaymentModel
                {
                    DocumentNumber = n.DocumentNumber,
                    Date = n.Date,
                    PaidAvance = n.PaidAvance,
                    Balance = n.Balance,
                    DatePaidAmount = n.DatePaidAmount,
                    PaidStatus = n.PaidStatus
                }).FirstOrDefault();
        }
    }
}
