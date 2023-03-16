﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace SSMO.Services.Documents.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductService productService;
        private readonly ICurrency currencyService;
        private readonly ICustomerOrderService customerOrdersSrvice;
        private readonly IDocumentService documentService;

        public InvoiceService
            (ApplicationDbContext dbContext, IMapper mapper, 
            IProductService productSevice, ICurrency currencyService,
            ICustomerOrderService customerOrderService, IDocumentService documentService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productSevice;
            this.currencyService = currencyService; 
            this.customerOrdersSrvice = customerOrderService;
            this.documentService= documentService;
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
            int number, string myCompanyName, string truckNumber, decimal deliveryCost, string swb)
            {
            var customerOrder = dbContext.CustomerOrders
                .Where(on => on.OrderConfirmationNumber == orderConfirmationNumber)
                .FirstOrDefault();

            if (customerOrder == null)
            {
                return null;
            }

            //if (dbContext.Documents
            //    .Where(t=>t.DocumentType == Data.Enums.DocumentTypes.Invoice)
            //    .Any(a => a.CustomerOrderId == customerOrder.Id))
            //{ return null; }

            var supplierOdrerForThisInvoice = dbContext.SupplierOrders
                .Where(c => c.CustomerOrderId == customerOrder.Id)
                .FirstOrDefault();

            var productList = dbContext.Products.
                Where(co => co.CustomerOrderId == customerOrder.Id && co.LoadedQuantityM3 != 0).ToList();

            if(productList == null) return null;

            foreach (var product in productList)
            {
                product.DeliveryTrasnportCost = productService.CalculateDeliveryCostOfTheProductInCo
                    (product.LoadedQuantityM3, customerOrder.TotalQuantity, deliveryCost);

                product.CostPrice = (product.CostPrice * product.LoadedQuantityM3 + product.DeliveryTrasnportCost?? 0)
                    /product.LoadedQuantityM3;
            }

            var invoiceCreate = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.Invoice,
                Amount = customerOrder.Amount,
                TotalAmount = customerOrder.TotalAmount,
                Vat = customerOrder.Vat,
                VatAmount = customerOrder.SubTotal,
                Balance = customerOrder.Balance,
                CurrencyExchangeRateUsdToBGN = currencyExchangeRateUsdToBGN,
                CurrencyId= customerOrder.CurrencyId,
                CustomerOrderId = customerOrder.Id,
                Date = date,
                PaidStatus = customerOrder.PaidAmountStatus,
                Products = new List<Product>(),
                SupplierOrderId = supplierOdrerForThisInvoice.Id,
                TruckNumber = truckNumber,
                Incoterms = customerOrder.DeliveryTerms,
                CustomerId = customerOrder.CustomerId,
                NetWeight = supplierOdrerForThisInvoice.NetWeight,
                GrossWeight = supplierOdrerForThisInvoice.GrossWeight,
                FSCSertificate = customerOrder.FSCSertificate,
                FSCClaim = customerOrder.FSCClaim,
                DeliveryTrasnportCost = deliveryCost,
                Swb = swb
            };

            if (CheckFirstInvoice())
            {
                var lastInvoiceNumber = dbContext.Documents.Where(n => n.DocumentType == Data.Enums.DocumentTypes.Invoice 
                || n.DocumentType == DocumentTypes.CreditNote || n.DocumentType == DocumentTypes.DebitNote)
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

            invoiceForPrint.Currency = dbContext.Currencys
                .Where(i=>i.Id == customerOrder.CurrencyId)
                .Select(n=>n.Name)
                .FirstOrDefault();

            var customerId = dbContext.CustomerOrders.Where(i => i.Id == customerOrder.Id)
                .Select(c => c.CustomerId).FirstOrDefault();

            var customerDetails = dbContext.Customers
                .Where(i => i.Id == customerId)
                .Select(a => new CustomerForInvoicePrint
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
            var statusId = dbContext.Statuses
                .Where(n=>n.Name == "Finished")
                .Select(i=>i.Id)
                .FirstOrDefault();
          
            supplierOdrerForThisInvoice.StatusId = statusId;
            customerOrder.StatusId = statusId;

            dbContext.Documents.Add(invoiceCreate);
            dbContext.SaveChanges();

            foreach (var product in productList)
            {
                productService.ClearProductQuantityWhenDealIsFinished(product.Id);
                product.DocumentId = dbContext.Documents
                    .Where(d=>d.DocumentType == Data.Enums.DocumentTypes.Invoice && d.DocumentNumber == invoiceCreate.DocumentNumber)
                    .Select(i=>i.Id)
                    .FirstOrDefault();
                invoiceCreate.Products.Add(product);
            }

            dbContext.SaveChanges();

            CreatePackingListForThisInvoice(invoiceCreate.Id);
            documentService.CreateBgInvoice(invoiceCreate.Id);
            return invoiceForPrint;
        }

        private void CreatePackingListForThisInvoice(int id)
        {
            var invoice = dbContext.Documents
                .Where(i => i.Id == id && i.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();

            if (invoice == null) return;

            if (dbContext.Documents
                .Any(d => d.DocumentNumber == invoice.DocumentNumber && d.DocumentType == Data.Enums.DocumentTypes.PackingList))
                return;

            var packingList = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.PackingList,
                DocumentNumber = invoice.DocumentNumber,
                Date = invoice.Date,
                CustomerId = invoice.CustomerId,
                CustomerOrderId = invoice.CustomerOrderId,
                TruckNumber = invoice.TruckNumber,
                Incoterms = invoice.Incoterms,
                MyCompanyId = invoice.MyCompanyId,
                NetWeight = invoice.NetWeight,
                GrossWeight = invoice.GrossWeight,
                SupplierOrderId = invoice.SupplierOrderId,
                CurrencyId = invoice.CurrencyId
            };

            dbContext.Documents.Add(packingList);
            dbContext.SaveChanges();
        }
        public void EditPackingList(int id)
        {
            if (id == 0) return;

            var invoiceNumber = dbContext.Documents
               .Where(i => i.Id == id)              
               .FirstOrDefault();

            var packingList = dbContext.Documents
                .Where(n => n.DocumentNumber == invoiceNumber.DocumentNumber & n.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .FirstOrDefault();

            packingList.Date = invoiceNumber.Date;  
            packingList.CustomerId = invoiceNumber.CustomerId;  
            packingList.CustomerOrderId = invoiceNumber.CustomerOrderId;    
            packingList.Products = invoiceNumber.Products;  
            packingList.TruckNumber = invoiceNumber.TruckNumber;    
            packingList.Incoterms = invoiceNumber.Incoterms;    
            packingList.MyCompanyId = invoiceNumber.MyCompanyId;
            packingList.GrossWeight = invoiceNumber.GrossWeight;
            packingList.NetWeight = invoiceNumber.NetWeight;
            packingList.SupplierOrderId = invoiceNumber.SupplierOrderId;

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
        public ICollection<InvoiceNumbersJsonList> GetInvoiceDocumentNumbers(int id)
        => dbContext.Documents
            .Where(type => type.DocumentType == Data.Enums.DocumentTypes.Invoice && type.MyCompanyId == id)
            .Select(num => new InvoiceNumbersJsonList 
            {  
                InvoiceId = num.Id,
                InvoiceNumber = num.DocumentNumber
            })
            .ToList();
        public BgInvoiceViewModel CreateBgInvoiceForPrint(int documentNumber)
        {
            var invoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber)
                .FirstOrDefault();

            if (invoice == null) return null;

            var currencyExchange = invoice.CurrencyExchangeRateUsdToBGN;

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

            var bgInvoiceForPrint = new BgInvoiceViewModel
            {
                DocumentNumber = documentNumber,
                CreditNoteOrDebitNoteNumber = invoice.CreditToInvoiceNumber,
                CreditNoteOrDebitNoteDate= invoice.CreditToInvoiceDate,
                DocumentType = invoice.DocumentType.ToString(),
                Date = invoice.Date,
                Amount = invoice.Amount * currencyExchange,
                Vat = invoice.Vat,
                VatAmount = invoice.Amount * currencyExchange * invoice.Vat / 100 ?? 0,
                TotalAmount = invoice.TotalAmount * currencyExchange,
                BgProducts = new List<BGProductsForBGInvoiceViewModel>(),
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
            bgInvoiceForPrint.CompanyBankDetails = new List<InvoiceBankDetailsViewModel>();
            
            var bankList = dbContext.BankDetails
                .Where(c => c.CompanyId == mycompanyId)
                .ToList();

            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencys
                    .Where(i => i.Id == bank.CurrencyId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                bgInvoiceForPrint.CompanyBankDetails.Add(new InvoiceBankDetailsViewModel
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

                bgInvoiceForPrint.BgProducts.Add(product);
            }

            return bgInvoiceForPrint;
        }
        public EditInvoiceViewModel ViewEditInvoice(int id)
        {
            if(id == 0)
            {
                return null;
            }

            var invoice = dbContext.Documents
                .Where(i=>i.Id == id).FirstOrDefault();

            var customerOrder = dbContext.CustomerOrders
                .Where(i => i.Id == invoice.CustomerOrderId)
                .FirstOrDefault();

            var invoiceForEdit = new EditInvoiceViewModel
            {
                Currencies = currencyService.GetCurrency(),
                CurrencyExchangeRate = invoice.CurrencyExchangeRateUsdToBGN,
                Date = invoice.Date,
                GrossWeight = invoice.GrossWeight,
                NetWeight = invoice.NetWeight,
                DeliveryCost = invoice.DeliveryTrasnportCost,
                CurrencyId = customerOrder.CurrencyId,
                OrderConfirmationNumber = customerOrder.OrderConfirmationNumber,
                OrderConfirmationNumbers = customerOrdersSrvice.AllCustomerOrderNumbers(),
                TruckNumber = invoice.TruckNumber,
                CreditToInvoiceNumber = invoice.CreditToInvoiceNumber,
                DebitToInvoiceNumber = invoice.DebitToInvoiceNumber
            };

            if (invoice.CreditToInvoiceNumber != 0 || invoice.DebitToInvoiceNumber != 0)
            {
                var productsList = dbContext.Products
                    .Where(cd => cd.CreditNoteId == invoice.Id || cd.DebitNoteId == invoice.Id);

                invoiceForEdit.Products = mapper.ProjectTo<EditProductForCreditAndDebitViewModel>(productsList).ToList();

                foreach (var product in invoiceForEdit.Products)
                {
                    product.Description = productService.GetDescriptionName(product.DescriptionId);
                    product.Grade = productService.GetGradeName(product.GradeId);
                    product.Size = productService.GetSizeName(product.SizeId);
                    product.Descriptions = productService.DescriptionIdAndNameList();
                    product.Grades = productService.GradeIdAndNameList();
                    product.Sizes = productService.SizeIdAndNameList();
                }
            }

           return invoiceForEdit;
        }
        public bool EditInvoice
            (int id, decimal currencyExchangeRate, DateTime date, decimal grossWeight, decimal netWeight, 
            decimal deliveryCost, int orderConfirmationNumber, string truckNumber, int invoiceForCreditNote, 
            int invoiceForDebitNote, ICollection<EditProductForCreditAndDebitViewModel> products)
        {
            if(id == 0) return false;

            var invoice = dbContext.Documents
               .Where(i => i.Id == id & i.DocumentType == Data.Enums.DocumentTypes.Invoice
               || i.DocumentType == DocumentTypes.CreditNote
               || i.DocumentType == DocumentTypes.DebitNote)
               .FirstOrDefault();

            var oldProductList = dbContext.Products
                .Where(i => i.DocumentId == invoice.Id)
                .ToList();

            var customerOrder = dbContext.CustomerOrders
                .Where(i => i.OrderConfirmationNumber == orderConfirmationNumber)
                .FirstOrDefault();

            if(invoice.DocumentType == Data.Enums.DocumentTypes.CreditNote)
            {
                var checkInvoice = true;

                if(invoiceForCreditNote != invoice.CreditToInvoiceNumber)
                {
                    var oldInvoiceForCredit = dbContext.Documents
                        .Where(d=>d.DocumentNumber== invoiceForCreditNote)
                        .FirstOrDefault();
                  
                    oldProductList = dbContext.Products
                        .Where(c=>c.DocumentId == oldInvoiceForCredit.Id)
                        .ToList();
                    foreach (var oldProduct in oldProductList)
                    {
                        oldProduct.CreditNoteId = 0;
                        oldProduct.CreditNoteProductAmount = 0M;
                    }

                    oldInvoiceForCredit.CreditNoteTotalAmount = 0M;
                    invoice.CreditToInvoiceNumber = invoiceForCreditNote;
                    var dateInvoiceForCredit = dbContext.Documents
                        .Where(n => n.DocumentNumber == invoiceForCreditNote)
                        .Select(d => d.Date)
                        .FirstOrDefault();
                    invoice.CreditToInvoiceDate = dateInvoiceForCredit;  
                    checkInvoice = false;
                }

                foreach (var product in products)
                {
                    var creditedInvoice = dbContext.Documents
                        .Where(cr=>cr.DocumentNumber == invoiceForCreditNote)                        
                        .FirstOrDefault();
                    
                    var productCheck = dbContext.Products
                        .Where(c=>c.CreditNoteId== invoice.Id && c.Id == product.Id)
                        .FirstOrDefault();

                    var sumAmount = 0M;

                    if (productCheck.DocumentId == null)
                    {
                        productCheck.DescriptionId = product.DescriptionId;
                        productCheck.GradeId = product.GradeId;
                        productCheck.SizeId = product.SizeId;
                        productCheck.Unit = (Data.Enums.Unit) Enum.Parse(typeof(Data.Enums.Unit),product.Unit.ToString());
                        productCheck.FSCClaim = product.FscClaim;
                        productCheck.FSCSertificate = product.FscSertificate;
                        productCheck.CreditNoteQuantity = product.Quantity;
                        productCheck.CreditNotePrice = product.CreditNotePrice;
                        sumAmount += product.Quantity * product.CreditNotePrice;
                    }
                    else
                    {
                        if (!checkInvoice)
                        {
                           var newProductCheck = dbContext.Products
                          .Where(c => c.DocumentId == creditedInvoice.Id)
                          .ToList();

                            foreach (var newProduct in products)
                            {
                                var productForCredit = dbContext.Products
                                    .Where(a=>a.DescriptionId == newProduct.DescriptionId &&
                                    a.GradeId == newProduct.GradeId && a.SizeId == newProduct.SizeId)
                                    .FirstOrDefault();
                                productForCredit.CreditNotePrice = newProduct.CreditNotePrice;
                                productForCredit.CreditNoteQuantity = newProduct.Quantity;
                                productForCredit.FSCClaim = newProduct.FscClaim;
                                productForCredit.FSCSertificate = newProduct.FscSertificate;
                                productForCredit.Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), newProduct.Unit.ToString());
                                productForCredit.CreditNoteProductAmount = newProduct.Quantity* newProduct.CreditNotePrice;
                               
                            }
                        }
                        else
                        {
                            productCheck.CreditNoteQuantity = product.Quantity;
                            productCheck.CreditNotePrice = product.CreditNotePrice;
                            productCheck.CreditNoteProductAmount = product.Quantity * product.CreditNotePrice;
                          
                        }
                        sumAmount += product.Quantity * product.CreditNotePrice;
                    }

                    creditedInvoice.Balance += creditedInvoice.CreditNoteTotalAmount;
                    invoice.Amount = sumAmount;
                    invoice.VatAmount = sumAmount * invoice.Vat / 100;
                    invoice.CreditNoteTotalAmount = invoice.Amount + invoice.VatAmount ?? 0;
                    creditedInvoice.CreditNoteTotalAmount = invoice.CreditNoteTotalAmount;
                    creditedInvoice.Balance -= invoice.CreditNoteTotalAmount;
                }
                
            }
            else if (invoice.DocumentType == Data.Enums.DocumentTypes.DebitNote)
            {

            }
            else
            {
                if (customerOrder.Id != invoice.CustomerOrderId)
                {
                    var supplierOdrerForThisInvoice = dbContext.SupplierOrders
                    .Where(c => c.CustomerOrderId == customerOrder.Id)
                    .FirstOrDefault();

                    var oldCustomerOrder = dbContext.CustomerOrders
                        .Where(i => i.Id == invoice.CustomerOrderId)
                        .FirstOrDefault();

                    oldCustomerOrder.StatusId = dbContext.Statuses
                        .Where(s => s.Name == "Active")
                        .Select(i => i.Id)
                        .FirstOrDefault();

                    var oldSupplierOrder = dbContext.SupplierOrders
                        .Where(i => i.CustomerOrderId == oldCustomerOrder.Id)
                        .FirstOrDefault();

                    oldSupplierOrder.StatusId = dbContext.Statuses
                        .Where(s => s.Name == "Active")
                        .Select(i => i.Id)
                        .FirstOrDefault();

                    var productList = dbContext.Products.
                        Where(co => co.CustomerOrderId == customerOrder.Id).ToList();

                    foreach (var product in productList)
                    {
                        product.DeliveryTrasnportCost = productService.CalculateDeliveryCostOfTheProductInCo
                            (product.LoadedQuantityM3, customerOrder.TotalQuantity, deliveryCost);
                    }

                    var customerId = dbContext.CustomerOrders.Where(i => i.Id == customerOrder.Id)
                      .Select(c => c.CustomerId).FirstOrDefault();

                    invoice.Products = productList;
                    invoice.SupplierOrderId = supplierOdrerForThisInvoice.Id;
                    invoice.CustomerOrderId = customerOrder.Id;
                    invoice.Incoterms = customerOrder.DeliveryTerms;
                    invoice.Amount = customerOrder.Amount;
                    invoice.VatAmount = customerOrder.SubTotal;
                    invoice.Vat = customerOrder.Vat;
                    invoice.Balance = customerOrder.Balance;

                    invoice.TotalAmount = (decimal)(customerOrder.Amount + invoice.VatAmount);

                    supplierOdrerForThisInvoice.StatusId = 1;
                    customerOrder.StatusId = 1;

                    foreach (var product in productList)
                    {
                        productService.ClearProductQuantityWhenDealIsFinished(product.Id);
                        product.DocumentId = invoice.Id;
                    }

                    foreach (var product in oldProductList)
                    {
                        productService.ReleaseProductExcludedFromInvoice(product.Id);
                        product.DocumentId = null;
                    }

                    EditPackingList(invoice.Id);

                    dbContext.SaveChanges();
                }
            }
           

            invoice.CurrencyExchangeRateUsdToBGN = currencyExchangeRate;
            invoice.Date = date;
            invoice.GrossWeight = grossWeight;
            invoice.NetWeight = netWeight;
            invoice.DeliveryTrasnportCost = deliveryCost;
            invoice.TruckNumber = truckNumber;

            dbContext.SaveChanges();

            return true;
        }
        
    }
}
