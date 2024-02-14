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
using SSMO.Models.CustomerOrders;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SSMO.Repository;
using SSMO.Services.Images;

namespace SSMO.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IProductService productService;
        private readonly IProductRepository productRepository;
        private readonly IImageService imageService;

        public DocumentService
            (ApplicationDbContext dbContext, IMapper mapper, IProductService productService,
            IProductRepository productRepository,IImageService imageService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productService;   
            this.productRepository= productRepository;  
            this.imageService = imageService;
        }
        public string GetLastNumOrder()
        {
            var n = dbContext.CustomerOrders.OrderByDescending(a => a.CustomerPoNumber).Select(a => a.CustomerPoNumber).FirstOrDefault();
            return n;
        }

        public ICollection<int> GetBgInvoices(int myCompanyId)
        {
            return dbContext.Documents
                .Where(t => t.DocumentType == Data.Enums.DocumentTypes.BGInvoice
                && t.MyCompanyId == myCompanyId)
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
        //vrashta modela na viziq za print na paking lista
        public PackingListForPrintViewModel PackingListForPrint(int packingListId)
        {
            if (packingListId == 0) return null;

            var packingList = dbContext.Documents
                .Where(num => num.Id == packingListId &&
                num.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .FirstOrDefault();

            var invoice = dbContext.Documents
                .Where(num => num.DocumentNumber == packingList.DocumentNumber &&
                num.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();

            var products = dbContext.InvoiceProductDetails
                .Where(co => co.InvoiceId == invoice.Id)
                .ToList();

            var packing = new PackingListForPrintViewModel
            {
                DocumentType = Data.Enums.DocumentTypes.PackingList.ToString(),
                Date = packingList.Date,
                DocumentNumber = packingList.DocumentNumber,
                CustomerId = packingList.CustomerId,
                Incoterms = packingList.Incoterms,
                FSCClaim = packingList.FscClaim,
                FSCSertificate = packingList.FSCSertificate,
                MyCompanyId = packingList.MyCompanyId,
                NetWeight = packingList.NetWeight,
                GrossWeight = packingList.GrossWeight,
                TruckNumber = packingList.TruckNumber,               
                Products = new List<ProductsForPackingListModel>(),
                CustomerPoNumber = new List<string>(),
                DeliveryAddress = packingList.DeliveryAddress
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

        public void CreateBgInvoice(int documentNumberId,int myCompanyId)
        {
            //TODO for credit and debit note
            var invoice = dbContext.Documents
                .Where(i => i.Id == documentNumberId)
                .FirstOrDefault();

            if (invoice == null) return;

            if (dbContext.Documents
                .Any(d => d.DocumentNumber == invoice.DocumentNumber && 
                d.DocumentType == Data.Enums.DocumentTypes.BGInvoice && d.MyCompanyId == myCompanyId))
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
                CustomerId = invoice.CustomerId,
                Date = invoice.Date,
                FscClaim = invoice.FscClaim,
                FSCSertificate = invoice.FSCSertificate,
                MyCompanyId = invoice.MyCompanyId,                     
                SupplierId = invoice.SupplierId,
                TruckNumber = invoice.TruckNumber,
                NetWeight = invoice.NetWeight,
                GrossWeight = invoice.GrossWeight,
                Incoterms = invoice.Incoterms,
                Swb = invoice.Swb,
                CurrencyExchangeRateUsdToBGN = currencyExchange,
                CurrencyId = invoice.CurrencyId,
                TotalQuantity= invoice.TotalQuantity,
                DealDescriptionBg = invoice.DealDescriptionBg,
                DealTypeBg= invoice.DealTypeBg,
                PlaceOfIssue = invoice.PlaceOfIssue
            };

            if(invoice.FiscalAgentId!= null)
            {
                bgInvoice.FiscalAgentId = invoice.FiscalAgentId;
            }

            if(invoice.FscTextId!= null)
            {
                bgInvoice.FscTextId= invoice.FscTextId; 
            }

            switch (invoice.DocumentType)
            {
                case Data.Enums.DocumentTypes.Invoice:
                    bgInvoice.TotalAmount = invoice.TotalAmount * currencyExchange; break;
                case Data.Enums.DocumentTypes.CreditNote: 
                    bgInvoice.CreditToInvoiceId = invoice.CreditToInvoiceId; 
                    bgInvoice.CreditToInvoiceDate = invoice.CreditToInvoiceDate;
                    bgInvoice.CreditNoteTotalAmount = invoice.CreditNoteTotalAmount * currencyExchange; break;
                case Data.Enums.DocumentTypes.DebitNote:
                    bgInvoice.DebitToInvoiceId = invoice.DebitToInvoiceId;
                    bgInvoice.DebitToInvoiceDate = invoice.DebitToInvoiceDate;
                    bgInvoice.DebitNoteTotalAmount = invoice.DebitNoteTotalAmount * currencyExchange;  break;
                default: break;
            }

            dbContext.Documents.Add(bgInvoice);
            dbContext.SaveChanges();
        }
        //vrashta modela na viziq za print na creditno i debitno
        public CreditAndDebitNoteViewModel PrintCreditAndDebitNote(int id)       
        {
            var creditOrDebitNote = dbContext.Documents
                .Find(id);

            var invoice = dbContext.Documents
                .Where(a => a.Id == creditOrDebitNote.DebitToInvoiceId || a.Id == creditOrDebitNote.CreditToInvoiceId)
                .FirstOrDefault();

            var creditOrDebitNoteForPrint = new CreditAndDebitNoteViewModel
            {
                ClientId = creditOrDebitNote.CustomerId ?? 0,
                Amount = creditOrDebitNote.Amount,
                Date = creditOrDebitNote.Date,
                Number = creditOrDebitNote.DocumentNumber,               
                SellerId = creditOrDebitNote.MyCompanyId,
                VatPercent = creditOrDebitNote.Vat ?? 0,
                VatAmount = creditOrDebitNote.VatAmount ?? 0,                
                CurrencyId = creditOrDebitNote.CurrencyId,
                Products = new List<AddProductsToCreditAndDebitNoteFormModel>(),
                CompanyBankDetails = new List<InvoiceBankDetailsViewModel>(),
                HeaderUrl = imageService.HeaderUrl(invoice.HeaderId??0),
                FooterUrl = imageService.FooterUrl(invoice.FooterId??0),
                DealDescription = invoice.DealDescriptionEng,
                DealType = invoice.DealTypeEng,
                PlaceOfIssue = invoice.PlaceOfIssue,
                LoadingAddress = creditOrDebitNote.LoadingAddress
            };

            if(creditOrDebitNote.DocumentType == Data.Enums.DocumentTypes.CreditNote) 
            {
                creditOrDebitNoteForPrint.InvoiceNumber = invoice.DocumentNumber;
                creditOrDebitNoteForPrint.InvoiceDate = creditOrDebitNote.CreditToInvoiceDate;
                creditOrDebitNoteForPrint.Total = creditOrDebitNote.CreditNoteTotalAmount;
            }
            else if(creditOrDebitNote.DocumentType == Data.Enums.DocumentTypes.DebitNote) 
            {
                creditOrDebitNoteForPrint.InvoiceNumber = invoice.DebitToInvoiceId;
                creditOrDebitNoteForPrint.InvoiceDate = creditOrDebitNote.DebitToInvoiceDate;
                creditOrDebitNoteForPrint.Total = creditOrDebitNote.DebitNoteTotalAmount;
            }   

            var customer = dbContext.Customers
                .Where(i => i.Id == creditOrDebitNote.CustomerId)
                .FirstOrDefault();

            var seller = dbContext.MyCompanies
                .Where(i => i.Id == creditOrDebitNote.MyCompanyId)
                .FirstOrDefault();

            var productsFromInvoice = new List<InvoiceProductDetails>();
            
            productsFromInvoice = dbContext.InvoiceProductDetails
                .Where(a=>a.CreditNoteId == creditOrDebitNote.Id || a.DebitNoteId == creditOrDebitNote.Id)
                .ToList() as List<InvoiceProductDetails>;

            var bankDetails = dbContext.Documents
                .Include(a => a.BankDetails)
                .Where(i => i.Id == invoice.Id)
                .Select(a=>a.BankDetails.FirstOrDefault())
                .FirstOrDefault();

            foreach (var product in productsFromInvoice)
                {
                    var mainProduct = productRepository.GetMainProduct(product.ProductId);

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
                FSCSertificate = seller.FscSertificate,
                FSCClaim = seller.FscClaim,
                RepresentativePerson = seller.RepresentativePerson,
                City = addressSeller.City,
                Country = addressSeller.Country,
                Street = addressSeller.Street
            };
            
                var currency = dbContext.Currencies.FirstOrDefault(c => c.Id == bankDetails.CurrencyId);
                creditOrDebitNoteForPrint.CompanyBankDetails.Add(new InvoiceBankDetailsViewModel
                {
                    BankName = bankDetails.BankName,
                    Iban = bankDetails.Iban,
                    Swift = bankDetails.Swift,
                    Currency = currency.Name
                });
           

            return creditOrDebitNoteForPrint;
        }
       //redaktira BG fakturata
        public void EditBgInvoice(int documentNumber, int myCompanyId)
        {
            var invoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber && i.MyCompanyId == myCompanyId)
                .FirstOrDefault();

            var bgInvoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber 
                && i.DocumentType == Data.Enums.DocumentTypes.BGInvoice
                && i.MyCompanyId == myCompanyId)
                .FirstOrDefault();

            if (invoice == null) return;
            //valuten kurs za kakvato i da e valuta, tryabva da go preimenuvam 
            var currencyExchange = invoice.CurrencyExchangeRateUsdToBGN;

            bgInvoice.Amount = invoice.Amount * currencyExchange;
            bgInvoice.Vat = invoice.Vat;
            bgInvoice.VatAmount = invoice.VatAmount * currencyExchange;
            bgInvoice.CustomerId = invoice.CustomerId;
            bgInvoice.Date = invoice.Date;
            bgInvoice.FscClaim = invoice.FscClaim;
            bgInvoice.FSCSertificate = invoice.FSCSertificate;
            bgInvoice.SupplierId = invoice.SupplierId;
            bgInvoice.TruckNumber = invoice.TruckNumber;
            bgInvoice.NetWeight = invoice.NetWeight;
            bgInvoice.GrossWeight = invoice.GrossWeight;
            bgInvoice.Incoterms = invoice.Incoterms;
            bgInvoice.Swb = invoice.Swb;
            bgInvoice.CurrencyExchangeRateUsdToBGN = currencyExchange;
            bgInvoice.CurrencyId = invoice.CurrencyId;
            bgInvoice.TotalQuantity = invoice.TotalQuantity;
            bgInvoice.FiscalAgentId = invoice.FiscalAgentId;

            switch (invoice.DocumentType)
            {
                case Data.Enums.DocumentTypes.Invoice:
                    bgInvoice.TotalAmount = invoice.TotalAmount * currencyExchange; break;
                case Data.Enums.DocumentTypes.CreditNote:
                    bgInvoice.CreditToInvoiceId = invoice.CreditToInvoiceId;
                    bgInvoice.CreditToInvoiceDate = invoice.CreditToInvoiceDate;
                    bgInvoice.CreditNoteTotalAmount = invoice.CreditNoteTotalAmount * currencyExchange; break;
                case Data.Enums.DocumentTypes.DebitNote:
                    bgInvoice.DebitToInvoiceId = invoice.DebitToInvoiceId;
                    bgInvoice.DebitToInvoiceDate = invoice.DebitToInvoiceDate;
                    bgInvoice.DebitNoteTotalAmount = invoice.DebitNoteTotalAmount * currencyExchange; break;
                default: break;
            }

            dbContext.SaveChanges();
        }
        //kolekciq ot banki i detaili   
        public List<InvoiceBankDetailsViewModel> BankDetails(int id)
        {
            var bankList = dbContext.BankDetails
                .Where(c => c.CompanyId == id)
                .ToList();

            var bankDetails = new List<InvoiceBankDetailsViewModel>();

            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencies
                    .Where(i => i.Id == bank.CurrencyId)
                    .Select(n => n.Name)
                    .FirstOrDefault();

                bankDetails.Add(new InvoiceBankDetailsViewModel
                {
                    Id= bank.Id,
                    BankName = bank.BankName,
                    Currency = currency,
                    CurrencyId = bank.CurrencyId,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                });
            }

            return bankDetails;
        }
        //dobavq fiscalen agent
        public void AddFiscalAgent
            (string name, string bgName, string details, string bgDetails, string userId)
        {
            var agent = new Data.Models.FiscalAgent
            {
                Name = name,
                BgName = bgName,
                Details = details,
                BgDetails = bgDetails,
                UserId = userId
            };

           dbContext.FiscalAgents.Add(agent);
           dbContext.SaveChanges();           
        }

        public ICollection<FiscalAgentViewModel> GetFiscalAgents()
        {
            var agents  = dbContext.FiscalAgents
                .ProjectTo<FiscalAgentViewModel>(mapper.ConfigurationProvider)
                .ToList();
            return agents;
            
        }
        
    }
}
