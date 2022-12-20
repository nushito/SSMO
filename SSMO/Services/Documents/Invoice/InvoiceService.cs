﻿using AutoMapper;
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
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN,
            int number, string myCompanyName, string truckNumber, decimal deliveryCost, decimal grossWeight, decimal netWeight)
        {
            var customerOrder = dbContext.CustomerOrders
                .Where(on => on.OrderConfirmationNumber == orderConfirmationNumber).FirstOrDefault();

            var supplierOdrerForThisInvoice = dbContext.SupplierOrders
                .Where(c => c.CustomerOrderId == customerOrder.Id)
                .FirstOrDefault();

            var productList = dbContext.Products.
                Where(co => co.CustomerOrderId == customerOrder.Id).ToList();

            foreach (var product in productList)
            {
                product.DeliveryTrasnportCost = productService.CalculateDeliveryCostOfTheProductInCo
                    (product.LoadedQuantityM3, customerOrder.TotalQuantity, deliveryCost);
            }

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
                SupplierOrderId = supplierOdrerForThisInvoice.Id,
                TruckNumber = truckNumber,
                Incoterms = customerOrder.DeliveryTerms,
                CustomerId = customerOrder.CustomerId,
                NetWeight = netWeight,
                GrossWeight = grossWeight,
                FSCSertificate = customerOrder.FSCSertificate
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
            
            invoiceForPrint.CompanyBankDetails = new List<InvoiceBankDetailsViewModel>();

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

            var myCompany = dbContext.MyCompanies
                .Where(n => n.Name.ToLower() == myCompanyName.ToLower())
                .FirstOrDefault();
            var bankList = dbContext.BankDetails
                .Where(c => c.CompanyId == myCompany.Id)
                .ToList();

            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencys
                    .Where(i => i.Id == bank.CurrencyId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                invoiceForPrint.CompanyBankDetails.Add(new InvoiceBankDetailsViewModel
                {
                    BankName = bank.BankName,
                    Currency = currency,
                    CurrencyId = bank.CurrencyId,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                });
            }

            var addressCompany = dbContext.Addresses
                .Where(co => co.Id == myCompany.AddressId)
                .FirstOrDefault();
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

            supplierOdrerForThisInvoice.StatusId = 1;
            customerOrder.StatusId = 1;

            dbContext.Documents.Add(invoiceCreate);
            dbContext.SaveChanges();

            foreach (var product in productList)
            {
                productService.ClearProductQuantityWhenDealIsFinished(product.Id);
                product.DocumentId = invoiceCreate.Id;
            }

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

        public EditInvoicePaymentModel InvoiceForEditByNumber(int documentNumber)
        {
            return dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber)
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

        public bool EditInvoicePayment
            (int documentNumber, bool paidStatus, decimal paidAdvance, DateTime datePaidAmount)
        {
            if (documentNumber == 0)
            {
                return false;
            }

            var invoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber)
                .FirstOrDefault();

            invoice.PaidAvance = paidAdvance;
            invoice.DatePaidAmount = datePaidAmount;
            invoice.PaidStatus = paidStatus;


            invoice.Balance = invoice.TotalAmount - paidAdvance;

            if (invoice.Balance > 0)
            {
                invoice.PaidStatus = false;
            }
            else
            {
                invoice.PaidStatus = true;
            }

            dbContext.SaveChanges();
            return true;
        }

        public ICollection<int> GetInvoiceDocumentNumbers()
        => dbContext.Documents
            .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Invoice)
            .Select(num => num.DocumentNumber)
            .ToList();

        public BgInvoiceViewModel CreateBgInvoice(int documentNumber, decimal currencyExchangeRateUsdToBGN)
        {
            var invoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber)
                .FirstOrDefault();

            if (invoice == null) return null;

            var mycompanyId = invoice.MyCompanyId;
            var mycompany = dbContext.MyCompanies
                .Where(i => i.Id == mycompanyId)
                .FirstOrDefault();
            var mycompanyAddress = dbContext.Addresses
                .Where(id => id.Id == mycompany.AddressId)
                .FirstOrDefault();

            var customerId = invoice.CustomerId;
            var customer = dbContext.Customers
                .Where(c => c.Id == customerId)
                .FirstOrDefault();
            var customerAdress = dbContext.Addresses.
                Where(id => id.Id == customer.AddressId)
                .FirstOrDefault();

            var products = dbContext.Products
                .Where(i => i.DocumentId == invoice.Id);

            var bgProducts = mapper.ProjectTo<BGProductsForBGInvoiceViewModel>(products).ToList();

            var bgInvoice = new BgInvoiceViewModel
            {
                DocumentNumber = documentNumber,
                Date = invoice.Date,
                Amount = invoice.Amount * currencyExchangeRateUsdToBGN,
                Vat = invoice.Vat,
                VatAmount = invoice.Amount * currencyExchangeRateUsdToBGN * invoice.Vat / 100 ?? 0,
                TotalAmount = invoice.TotalAmount * currencyExchangeRateUsdToBGN,
                BgCustomer = new BGCustomerForInvoicePrint
                {
                    BgName = customer.BgCustomerName,
                    BgRepresentativePerson = customer.BgCustomerRepresentativePerson,
                    EIK = customer.EIK,
                    VAT = customer.VAT,
                    ClientAddress = new BGAddressCustomerForInvoicePrint
                    {
                        BgCity = customerAdress.BgCity,
                        BgCountry = customerAdress.Bgcountry,
                        BgStreet = customerAdress.BgStreet
                    },
                },
                BgMyCompany = new BGMyCompanyInvoicePrintViewModel
                {
                    BgName = mycompany.BgName,
                    BgRepresentativePerson = mycompany.BgRepresentativePerson,
                    EIK = mycompany.Eik,
                    VAT = mycompany.VAT,
                    BgCity = mycompanyAddress.BgCity,
                    BgCountry = mycompanyAddress.Bgcountry,
                    BgStreet = mycompanyAddress.BgStreet
                }
            };
            bgInvoice.CompanyBankDetails = new List<InvoiceBankDetailsViewModel>();

            var bankList = dbContext.BankDetails
                .Where(c => c.CompanyId == mycompanyId)
                .ToList();

            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencys
                    .Where(i => i.Id == bank.CurrencyId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                bgInvoice.CompanyBankDetails.Add(new InvoiceBankDetailsViewModel
                {
                    BankName = bank.BankName,
                    Currency = currency,
                    CurrencyId = bank.CurrencyId,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                });
            }

            foreach (var product in bgProducts)
            {
                product.BgDescription = dbContext.Descriptions
                    .Where(i => i.Id == product.DescriptionId)
                    .Select(bgN => bgN.BgName)
                    .FirstOrDefault();

                product.Grade = dbContext.Grades
                    .Where(i => i.Id == product.GradeId)
                    .Select(bgN => bgN.Name)
                    .FirstOrDefault();

                product.Size = dbContext.Sizes
                    .Where(i => i.Id == product.SizeId)
                    .Select(bgN => bgN.Name)
                    .FirstOrDefault();
            }
            bgInvoice.BgProducts = bgProducts;
            return bgInvoice;
        }
    }
}
