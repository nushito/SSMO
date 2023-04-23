﻿using AutoMapper;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Documents;
using SSMO.Models.Documents.Packing_List;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductService productService;

        public DocumentService(ApplicationDbContext dbContext, IMapper mapper, IProductService productService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productService;   
        }

        public string GetLastNumOrder()
        {
            var n = dbContext.CustomerOrders.OrderByDescending(a => a.CustomerPoNumber).Select(a => a.CustomerPoNumber).FirstOrDefault();
            return n;
        }

        public ICollection<int> GetBgInvoices()
        {
            return dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.BGInvoice)
                .OrderByDescending(n=>n.DocumentNumber)
                .Select(num => num.DocumentNumber)
                .ToList();
        }

        public ICollection<int> GetPackingList()
        {
            return dbContext.Documents
                .Where(type => type.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .OrderByDescending(n=>n.DocumentNumber)
                .Select(num => num.DocumentNumber)
                .ToList();
        }

        public PackingListForPrintViewModel PackingListForPrint(int packingListNumber)
        {
            if (packingListNumber == 0) return null;

            var invoice = dbContext.Documents
                .Where(num => num.DocumentNumber == packingListNumber &&
                num.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();

            var packingList = dbContext.Documents
                .Where(num => num.DocumentNumber == packingListNumber && 
                num.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .FirstOrDefault();

            var products = dbContext.InvoiceProductDetails
                .Where(co => co.InvoiceId == invoice.Id)
                .ToList();


            var packing = new PackingListForPrintViewModel
            {
                DocumentType = Data.Enums.DocumentTypes.PackingList.ToString(),
                Date = packingList.Date,
                DocumentNumber = packingListNumber,
                CustomerId = packingList.CustomerId,
                Incoterms = packingList.Incoterms,
                FSCClaim = packingList.FSCClaim,
                FSCSertificate = packingList.FSCSertificate,
                MyCompanyId = packingList.MyCompanyId,
                NetWeight = packingList.NetWeight,
                GrossWeight = packingList.GrossWeight,
                TruckNumber = packingList.TruckNumber,               
                Products = new List<ProductsForPackingListModel>(),
                CustomerPoNumber = new List<string>()
            };

            var customerOrders = dbContext.CustomerOrders
                .Where(a => products.Select(c => c.CustomerOrderId).Contains(a.Id))
                .ToList();

            foreach (var order in customerOrders)
            {
                packing.CustomerPoNumber.Add(order.CustomerPoNumber);
            }

            var myCompany = dbContext.MyCompanies
                .Where(i=>i.Id == packingList.MyCompanyId)
                .FirstOrDefault();

            var myCompanyAddress = dbContext.Addresses
               .Where(id => id.Id == myCompany.AddressId)
               .FirstOrDefault();

            packing.MyCompanyForPl = new MyCompanyForPackingPrint
            {
                Name = myCompany.Name,
                EIK = myCompany.Eik,
                VAT = myCompany.VAT,
                Street = myCompanyAddress.Street,
                City = myCompanyAddress.City,
                Country = myCompanyAddress.Country
            };

            packing.Products = mapper.Map<ICollection<ProductsForPackingListModel>>(products);

            foreach (var product in packing.Products)
            {
                var mainproduct = dbContext.Products
                    .Where(i=>i.Id == product.ProductId).FirstOrDefault();

                product.DescriptionName = dbContext.Descriptions
                .Where(i => i.Id == mainproduct.DescriptionId).Select(n => n.Name).FirstOrDefault();

                product.GradeName = dbContext.Grades
                .Where(i => i.Id == mainproduct.GradeId).Select(n => n.Name).FirstOrDefault();
                product.SizeName = dbContext.Sizes
                .Where(i => i.Id == mainproduct.SizeId).Select(n => n.Name).FirstOrDefault();
                product.FSCClaim = product.FSCClaim;
                product.FSCSertificate = product.FSCSertificate;
                product.Pallets = product.Pallets;
                product.SheetsPerPallet = product.SheetsPerPallet;
                product.InvoicedQuantity = product.InvoicedQuantity;
                product.Unit = product.Unit;
            }

            var customer = dbContext.Customers
                .Where(id => id.Id == packing.CustomerId)
                .FirstOrDefault();
            packing.CustomerId = customer.Id;

            var customerAddress = dbContext.Addresses
                .Where(id => id.Id == customer.AddressId)
                .FirstOrDefault();

            packing.Customer = new CustomerForPackingListPrint
            {
                Name = customer.Name,
                EIK = customer.EIK,
                VAT = customer.VAT,
                RepresentativePerson = customer.RepresentativePerson,
                ClientAddress = new AddressCustomerForPackingList
                {
                    City = customerAddress.City,
                    Country = customerAddress.Country,
                    Street = customerAddress.Street
                }
            };

            return packing;
        }

        public void CreateBgInvoice(int documentNumberId)
        {
            var invoice = dbContext.Documents
                .Where(i => i.Id == documentNumberId && i.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();

            if (invoice == null) return;

            if (dbContext.Documents
                .Any(d => d.DocumentNumber == invoice.DocumentNumber && 
                d.DocumentType == Data.Enums.DocumentTypes.BGInvoice))
            { return; }

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

            var bgInvoice = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.BGInvoice,
                DocumentNumber = invoice.DocumentNumber,
                Amount = invoice.Amount * currencyExchange,
                Vat = invoice.Vat,
                VatAmount = invoice.VatAmount * currencyExchange,
                TotalAmount = invoice.TotalAmount * currencyExchange,
                CustomerId = invoice.CustomerId,
                Date = invoice.Date,
                FSCClaim = invoice.FSCClaim,
                FSCSertificate = invoice.FSCSertificate,
                MyCompanyId = invoice.MyCompanyId,
                SupplierOrderId = invoice.SupplierOrderId,              
                SupplierId = invoice.SupplierId,
                TruckNumber = invoice.TruckNumber,
                NetWeight = invoice.NetWeight,
                GrossWeight = invoice.GrossWeight,
                Incoterms = invoice.Incoterms,
                Swb = invoice.Swb,
                CurrencyExchangeRateUsdToBGN = currencyExchange,
                CurrencyId = invoice.CurrencyId,
                TotalQuantity= invoice.TotalQuantity,
                InvoiceProducts = new List<InvoiceProductDetails>()
            };

            var bgProducts = dbContext.InvoiceProductDetails
                .Where(i=>i.InvoiceId == invoice.Id)
                .ToList();

            bgInvoice.InvoiceProducts = bgProducts;

            dbContext.Documents.Add(bgInvoice);
            dbContext.SaveChanges();
        }

        public CreditAndDebitNoteViewModel PrintCreditAndDebitNote(int id)       
        {
            var creditOrDebitNote = dbContext.Documents
                .Find(id);

            var creditOrDebitNoteForPrint = new CreditAndDebitNoteViewModel
            {
                ClientId = creditOrDebitNote.CustomerId ?? 0,
                Amount = creditOrDebitNote.Amount,
                Date = creditOrDebitNote.Date,
                Number = creditOrDebitNote.DocumentNumber,               
                SellerId = creditOrDebitNote.MyCompanyId,
                VatPercent = creditOrDebitNote.Vat ?? 0,
                VatAmount = creditOrDebitNote.VatAmount ?? 0,
                Total = creditOrDebitNote.TotalAmount,
                CurrencyId = creditOrDebitNote.CurrencyId,
                Products = new List<AddProductsToCreditAndDebitNoteFormModel>(),
                CompanyBankDetails = new List<InvoiceBankDetailsViewModel>()
            };

            if(creditOrDebitNote.DocumentType == Data.Enums.DocumentTypes.CreditNote) 
            {
                creditOrDebitNoteForPrint.InvoiceNumber = creditOrDebitNote.CreditToInvoiceNumber;
                creditOrDebitNoteForPrint.InvoiceDate = creditOrDebitNote.CreditToInvoiceDate;
            }
            else if(creditOrDebitNote.DocumentType == Data.Enums.DocumentTypes.DebitNote) 
            {
                creditOrDebitNoteForPrint.InvoiceNumber = creditOrDebitNote.DebitToInvoiceNumber;
                creditOrDebitNoteForPrint.InvoiceDate = creditOrDebitNote.DebitToInvoiceDate;
            }   

            var customer = dbContext.Customers
                .Where(i => i.Id == creditOrDebitNote.CustomerId)
                .FirstOrDefault();

            var seller = dbContext.MyCompanies
                .Where(i => i.Id == creditOrDebitNote.MyCompanyId)
                .FirstOrDefault();

            var bankDetails = dbContext.BankDetails
                .Where(i => i.CompanyId == seller.Id)
                .ToList();

            var productList = dbContext.Products
                .Where(d => d.DocumentId == creditOrDebitNote.Id || d.DocumentId == creditOrDebitNote.Id)
                .ToList();

            var productsFromInvoice = new List<InvoiceProductDetails>();

            if (productList == null)
            {
                productsFromInvoice = dbContext.InvoiceProductDetails
                    .Where(a=>a.CreditNoteId == creditOrDebitNote.Id || a.DebitNoteId == creditOrDebitNote.Id)
                    .ToList() as List<InvoiceProductDetails>;

                foreach (var product in productsFromInvoice)
                {
                    var mainProduct = dbContext.Products
                        .Where(a=>a.Id == product.ProductId)
                        .FirstOrDefault();

                    var description = productService.GetDescriptionName(mainProduct.DescriptionId);
                    var grade = productService.GetGradeName(mainProduct.GradeId);
                    var size = productService.GetSizeName(mainProduct.SizeId);

                    var productForPrint = new AddProductsToCreditAndDebitNoteFormModel
                    {
                        Description = description,
                        Grade = grade,
                        Size = size,
                        Unit = product.Unit.ToString(),
                        FscClaim = product.FscClaim,
                        FscSertificate = product.FscCertificate,
                        Pallets = product.CreditNotePallets,
                        SheetsPerPallet = product.CreditNoteSheetsPerPallet
                    };
                    switch (creditOrDebitNote.DocumentType)
                    {
                        case Data.Enums.DocumentTypes.CreditNote:
                            productForPrint.Price = product.CreditNotePrice;
                            productForPrint.Quantity = product.CreditNoteQuantity;
                            productForPrint.Amount = product.CreditNoteProductAmount;
                            break;
                        case Data.Enums.DocumentTypes.DebitNote:
                            productForPrint.Price = product.DebitNotePrice;
                            productForPrint.Quantity = product.DebitNoteQuantity;
                            productForPrint.Amount = product.DebitNoteAmount;
                            break;
                        default: break;
                    }
                    creditOrDebitNoteForPrint.Products.Add(productForPrint);
                }
            }
            else
            {
                foreach (var product in productList)
                {
                    var description = productService.GetDescriptionName(product.DescriptionId);
                    var grade = productService.GetGradeName(product.GradeId);
                    var size = productService.GetSizeName(product.SizeId);

                    var productForPrint = new AddProductsToCreditAndDebitNoteFormModel
                    {
                        Description = description,
                        Grade = grade,
                        Size = size,
                        Unit = product.Unit.ToString(),
                        FscClaim = product.FscClaim,
                        FscSertificate = product.FscSertificate,
                        Pallets = product.Pallets,
                        SheetsPerPallet = product.SheetsPerPallet
                    };
                    switch (creditOrDebitNote.DocumentType)
                    {
                        case Data.Enums.DocumentTypes.CreditNote:
                            productForPrint.Price = product.Price;
                            productForPrint.Quantity = product.OrderedQuantity;
                            productForPrint.Amount = product.Amount;
                            break;
                        case Data.Enums.DocumentTypes.DebitNote:
                            productForPrint.Price = product.Price;
                            productForPrint.Quantity = product.OrderedQuantity;
                            productForPrint.Amount = product.OrderedQuantity;
                            break;
                        default: break;
                    }
                    creditOrDebitNoteForPrint.Products.Add(productForPrint);
                }
            }

            var addressCustomer = dbContext.Addresses
                .Where(i => i.Id == customer.AddressId)
                .FirstOrDefault();

            var addressSeller = dbContext.Addresses
                .Where(i => i.Id == seller.AddressId)
                .FirstOrDefault();

            creditOrDebitNoteForPrint.Client = new CustomerForInvoicePrint
            {
                EIK = customer.EIK,
                VAT = customer.VAT,
                Name = customer.Name,
                RepresentativePerson = customer.RepresentativePerson,
                ClientAddress = new AddressCustomerForInvoicePrint
                {
                    City = addressCustomer.City,
                    Country = addressCustomer.Country,
                    Street = addressCustomer.Street
                }
            };

            creditOrDebitNoteForPrint.Seller = new MyCompanyForInvoicePrint
            {
                EIK = seller.Eik,
                VAT = seller.VAT,
                Name = seller.Name,
                FSCSertificate = seller.FSCSertificate,
                FSCClaim = seller.FSCClaim,
                RepresentativePerson = seller.RepresentativePerson,
                City = addressSeller.City,
                Country = addressSeller.Country,
                Street = addressSeller.Street
            };

            foreach (var bank in bankDetails)
            {
                var currency = dbContext.Currencies.FirstOrDefault(c => c.Id == bank.CurrencyId);
                creditOrDebitNoteForPrint.CompanyBankDetails.Add(new InvoiceBankDetailsViewModel
                {
                    BankName = bank.BankName,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                    Currency = currency.Name
                });
            }
            return creditOrDebitNoteForPrint;
        }
    }
}
