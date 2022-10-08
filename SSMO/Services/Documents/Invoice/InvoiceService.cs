using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents.Invoice;
using System;
using System.Linq;

namespace SSMO.Services.Documents.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public InvoiceService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
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
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN, int number, string myCompanyName)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(on => on.OrderConfirmationNumber == orderConfirmationNumber).FirstOrDefault();

            var supplierOdrerForThisInvoiceId = dbContext.SupplierOrders.Where(c => c.CustomerOrderId == customerOrder.Id)
                .Select(i=>i.Id).FirstOrDefault();
            if (customerOrder == null)
            {
                return null;
            }

            var invoiceCreate = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.Invoice,
                
                TotalAmount = customerOrder.TotalAmount,
                Vat = customerOrder.Vat,
                Balance = customerOrder.Balance,
                CurrencyExchangeRateUsdToBGN = currencyExchangeRateUsdToBGN,
                CustomerOrderId = customerOrder.Id,
                Date = date,
                PaidStatus = customerOrder.PaidAmountStatus,
                Products = (System.Collections.Generic.ICollection<Product>)customerOrder.Products,
                SupplierOrderId = supplierOdrerForThisInvoiceId,
                
             };

            if (CheckFirstInvoice())
            {
                var lastInvoiceNumber = dbContext.Documents.Where(n => n.DocumentType == Data.Enums.DocumentTypes.Invoice)
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

            // var customer = .FirstOrDefault();

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

            var myCompany = dbContext.MyCompanies.Where(n => n.Name.ToLower() == myCompanyName.ToLower()).FirstOrDefault();

            var addressCompany = dbContext.Addresses.Where(co=>co.Id == myCompany.AddressId).FirstOrDefault();

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


            dbContext.Documents.Add(invoiceCreate);
            dbContext.SaveChanges();

            return invoiceForPrint;
        }
    }
}
//SqlException: Invalid column name 'Amount'.
//Invalid column name 'Number'.
//Invalid column name 'SupplierOrderId'.
