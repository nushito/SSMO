using AutoMapper;
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
            
            var packingList = dbContext.Documents
                .Where(num => num.DocumentNumber == packingListNumber && num.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .FirstOrDefault();

            var products = dbContext.Products
                .Where(co => co.CustomerOrderId == packingList.CustomerOrderId)
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
                Products = new List<ProductsForPackingListPrint>()
            };

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

          //  packinglist.Products = mapper.Map<ICollection<ProductsForPackingListPrint>>(products);
            foreach (var item in products)
            {

                packing.Products.Add(new ProductsForPackingListPrint
                {
                    DescriptionName = dbContext.Descriptions
                    .Where(i => i.Id == item.DescriptionId).Select(n => n.Name).FirstOrDefault(),
                    GradeName = dbContext.Grades
                    .Where(i => i.Id == item.GradeId).Select(n => n.Name).FirstOrDefault(),
                    SizeName = dbContext.Sizes
                    .Where(i => i.Id == item.SizeId).Select(n => n.Name).FirstOrDefault(),
                    FSCClaim = item.FSCClaim,
                    FSCSertificate = item.FSCSertificate,
                    Pallets = item.Pallets,
                    SheetsPerPallet = item.SheetsPerPallet,
                    OrderedQuantity = item.OrderedQuantity,
                    Unit = item.Unit.ToString()
                }); 
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
                .Any(d => d.DocumentNumber == invoice.DocumentNumber && d.DocumentType == Data.Enums.DocumentTypes.BGInvoice))
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
                CustomerOrderId = invoice.CustomerOrderId,
                SupplierId = invoice.SupplierId,
                TruckNumber = invoice.TruckNumber,
                NetWeight = invoice.NetWeight,
                GrossWeight = invoice.GrossWeight,
                Incoterms = invoice.Incoterms,
                Swb = invoice.Swb,
                CurrencyExchangeRateUsdToBGN = currencyExchange,
                CurrencyId = invoice.CurrencyId,
                Products = new List<Product>()
            };

            var bgProducts = dbContext.Products
                .Where(i=>i.DocumentId == documentNumberId)
                .ToList();

            foreach (var product in bgProducts)
            {
                product.BgPrice = product.Price * invoice.CurrencyExchangeRateUsdToBGN;
                product.BgAmount = product.Amount * invoice.CurrencyExchangeRateUsdToBGN;
                bgInvoice.Products.Add(product);
            }
            
            dbContext.Documents.Add(bgInvoice);
            dbContext.SaveChanges();
        }

        public CreditAndDebitNoteViewModel PrintCreditAndDebitNote(int id)       
        {
            var creditNote = dbContext.Documents
                .Find(id);

            var creditNoteForPrint = new CreditAndDebitNoteViewModel
            {
                ClientId = creditNote.CustomerId ?? 0,
                Amount = creditNote.Amount,
                Date = creditNote.Date,
                Number = creditNote.DocumentNumber,
                InvoiceNumber = creditNote.CreditToInvoiceNumber,
                InvoiceDate = creditNote.CreditToInvoiceDate,
                SellerId = creditNote.MyCompanyId,
                VatPercent = creditNote.Vat ?? 0,
                VatAmount = creditNote.VatAmount ?? 0,
                Total = creditNote.TotalAmount,
                CurrencyId = creditNote.CurrencyId,
                Products = new List<AddProductsToCreditAndDebitNoteFormModel>(),
                CompanyBankDetails = new List<InvoiceBankDetailsViewModel>()
            };

            var customer = dbContext.Customers
                .Where(i => i.Id == creditNote.CustomerId)
                .FirstOrDefault();

            var seller = dbContext.MyCompanies
                .Where(i => i.Id == creditNote.MyCompanyId)
                .FirstOrDefault();

            var bankDetails = dbContext.BankDetails
                .Where(i => i.CompanyId == seller.Id)
                .ToList();

            var products = dbContext.Products
                .Where(d => d.DocumentId == creditNote.Id || d.CreditNoteId == creditNote.Id)
                .ToList();

            var addressCustomer = dbContext.Addresses
                .Where(i => i.Id == customer.AddressId)
                .FirstOrDefault();

            var addressSeller = dbContext.Addresses
                .Where(i => i.Id == seller.AddressId)
                .FirstOrDefault();

            creditNoteForPrint.Client = new CustomerForInvoicePrint
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

            creditNoteForPrint.Seller = new MyCompanyForInvoicePrint
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

            foreach (var product in products)
            {
                var description = productService.GetDescriptionName(product.DescriptionId);
                var grade = productService.GetGradeName(product.GradeId);
                var size = productService.GetSizeName(product.SizeId);

                creditNoteForPrint.Products.Add(
                    new AddProductsToCreditAndDebitNoteFormModel
                    {
                        Description = description,
                        Grade = grade,
                        Size = size,
                        Unit = product.Unit.ToString(),
                        FscClaim = product.FSCClaim,
                        FscSertificate = product.FSCSertificate,
                        Pallets = product.CreditNotePallets,
                        SheetsPerPallet = product.CreditNoteSheetsPerPallet,
                        Price = product.CreditNotePrice,
                        Quantity = product.CreditNoteQuantity,
                        Amount = product.CreditNoteProductAmount
                    });
            }

            foreach (var bank in bankDetails)
            {
                var currency = dbContext.Currencys.FirstOrDefault(c => c.Id == bank.CurrencyId);
                creditNoteForPrint.CompanyBankDetails.Add(new InvoiceBankDetailsViewModel
                {
                    BankName = bank.BankName,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                    Currency = currency.Name
                });
            }
            return creditNoteForPrint;
        }
    }
}
