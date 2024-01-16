using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Models;
using SSMO.Infrastructure;
using SSMO.Models.CustomerOrders;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Documents.Packing_List;
using SSMO.Models.FscTexts;
using SSMO.Models.Image;
using SSMO.Models.Reports.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Repository;
using SSMO.Services.CustomerOrderService;
using SSMO.Services.FscTextDocuments;
using SSMO.Services.Images;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly HttpContextUserIdExtension httpContextAccessor;
        private readonly IProductRepository productRepository;
        private readonly IImageService imageService;
        private readonly IFscTextService fscTextService;
        public InvoiceService
            (ApplicationDbContext dbContext, IMapper mapper,
            IProductService productSevice, ICurrency currencyService,
            ICustomerOrderService customerOrderService, IDocumentService documentService,
            HttpContextUserIdExtension httpContextUserIdExtension, IProductRepository productRepository,
            IImageService imageService, IFscTextService fscTextService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.productService = productSevice;
            this.currencyService = currencyService;
            this.customerOrdersSrvice = customerOrderService;
            this.documentService = documentService;
            this.httpContextAccessor = httpContextUserIdExtension;
            this.productRepository = productRepository;
            this.imageService = imageService;
            this.fscTextService = fscTextService;
        }
        public bool CheckFirstInvoice(int myCompanyId)
        {
            var invoiceCheck = dbContext.Documents.Where(a => a.DocumentType == Data.Enums.DocumentTypes.Invoice 
            && a.MyCompanyId == myCompanyId).Any();

            if (!invoiceCheck)
            {
                return false;
            }
            return true;
        }

        public async Task<InvoicePrintViewModel> CreateInvoice(
            List<int> selectedCustomerOrderNumbers, List<ProductsForInvoiceViewModel> products,
            List<ServiceProductForInvoiceFormModel> serviceProducts,
            DateTime date, decimal currencyExchangeRateUsdToBGN,
            int number, string myCompanyName, string truckNumber, decimal deliveryCost, string swb,
            decimal netWeight, decimal grossWeight, string incoterms,int customerId, int currencyId, int vat, 
            int myCompanyId, string comment, string deliveryAddress,
            string dealTypeEng, string dealTypeBg, string descriptionEng, string descriptionBg, List<int> banks, 
            int? fiscalAgent, int? fscText, string paymentTerms, string loadingAddress,string placeOfIssue, 
            int header, int footer, decimal? factoring, decimal? comission)
        {
            var loggedUser = httpContextAccessor.ContextAccessUserId();
            var myCompany = dbContext.MyCompanies
              .Where(n => n.Id == myCompanyId)
              .FirstOrDefault();

            var bankList = dbContext.BankDetails
               .Where(c => banks.Contains(c.Id))
               .ToList();

            if (loggedUser != myCompany.UserId) { return null; }
          
            var customerOrders = dbContext.CustomerOrders
               .Where(on => selectedCustomerOrderNumbers.Contains(on.Id))
               .ToList();

            var statusFinished = dbContext.Statuses
               .Where(n => n.Name == "Finished")
               .Select(i => i.Id)
               .FirstOrDefault();

            var invoiceCreate = new Document
            {
                DocumentType = Data.Enums.DocumentTypes.Invoice,
                CurrencyExchangeRateUsdToBGN = currencyExchangeRateUsdToBGN,
                CurrencyId = currencyId,
                MyCompanyId = myCompanyId,
                Date = date,
                Vat= vat,        
                InvoiceProducts = new List<InvoiceProductDetails>(),
                TruckNumber = truckNumber,
                Incoterms = incoterms,
                CustomerId = customerId,
                NetWeight = netWeight,
                GrossWeight = grossWeight,
                DeliveryTrasnportCost = deliveryCost,
                Swb = swb,
                CustomerOrders = customerOrders,
                Comment = comment,
                DeliveryAddress = deliveryAddress,
                Payments = new List<Payment>(),
                DealTypeEng = dealTypeEng,
                DealTypeBg = dealTypeBg,
                DealDescriptionEng = descriptionEng,
                DealDescriptionBg = descriptionBg,
                BankDetails = new List<BankDetails>(),
                PaymentTerms = paymentTerms,
                LoadingAddress = loadingAddress,
                PlaceOfIssue = placeOfIssue,
                HeaderId = header,
                FooterId = footer,
                Factoring = (decimal)factoring,
                ProcentComission = (decimal)comission
            };

            if (CheckFirstInvoice(myCompanyId))
            {
                var lastInvoiceNumber = dbContext.Documents.Where(n => n.DocumentType == Data.Enums.DocumentTypes.Invoice
                || n.DocumentType == DocumentTypes.CreditNote || n.DocumentType == DocumentTypes.DebitNote &&
                n.MyCompanyId == myCompanyId)
                    .OrderBy(n => n.DocumentNumber)
                    .Select(n => n.DocumentNumber).LastOrDefault();

                invoiceCreate.DocumentNumber = lastInvoiceNumber + 1;
            }
            else
            {
                invoiceCreate.DocumentNumber = number;
            }

            if (fiscalAgent != null)
            {
                invoiceCreate.FiscalAgentId= fiscalAgent;
            }

            if(fscText != null)
            {
                invoiceCreate.FscTextId = fscText;
            }
           
            bankList.ForEach(a => invoiceCreate.BankDetails.Add(a));

            invoiceCreate.Amount = products.Where(q=>q.InvoicedQuantity > 0).Sum(o => o.Amount)+ serviceProducts.Where(q => q.InvoicedQuantity > 0).Sum(o => o.InvoicedQuantity*o.SellPrice);
            invoiceCreate.VatAmount = invoiceCreate.Amount * vat / 100;
            invoiceCreate.TotalAmount = invoiceCreate.Amount + invoiceCreate.VatAmount ?? 0;
            
           await dbContext.Documents.AddAsync(invoiceCreate);
           await dbContext.SaveChangesAsync();

            foreach (var item in customerOrders)
            {                
                item.Documents.Add(invoiceCreate);
            }

            var productsForInvoice = new List<ProductsForInvoiceModel>();
          
            foreach (var product in products)
            {
                if(product.InvoicedQuantity == 0) continue;

                var mainProduct = productRepository.GetMainProduct(product.ProductId);
                if (mainProduct == null) continue;

                mainProduct.SoldQuantity += product.InvoicedQuantity;
                
                var size = productService.GetSizeName(mainProduct.SizeId);

                var customerOrderProduct = dbContext.CustomerOrderProductDetails
                    .Where(i=>i.ProductId == product.ProductId && i.CustomerOrderId == product.CustomerOrderId)
                    .FirstOrDefault();                

                var customerOrder = dbContext.CustomerOrders
                    .Where(i=>i.Id == product.CustomerOrderId)
                    .FirstOrDefault();

                if (customerOrderProduct.AutstandingQuantity < product.InvoicedQuantity)
                {
                    continue;
                }

                customerOrderProduct.AutstandingQuantity -= product.InvoicedQuantity;

                var purchaseProductDetail = dbContext.PurchaseProductDetails
                    .Where(a => a.Id == product.PurchaseCostPriceId).FirstOrDefault();

                var supplierOrder = dbContext.SupplierOrders
                    .Where(a=>a.Id == purchaseProductDetail.SupplierOrderId).FirstOrDefault();

                var invoiceProduct = new InvoiceProductDetails
                {
                    CustomerOrderId = product.CustomerOrderId,
                    Pallets = product.Pallets,
                    SheetsPerPallet = product.SheetsPerPallet,
                    FscCertificate = product.FscCertificate,
                    FscClaim = product.FscClaim,
                    ProductId = product.ProductId,
                    PurchaseProductDetailsId = purchaseProductDetail.Id,
                    SellPrice = product.SellPrice,
                    Unit = product.Unit,
                    VehicleNumber = product.VehicleNumber,
                    InvoicedQuantity = product.InvoicedQuantity,
                    BgPrice = product.SellPrice * currencyExchangeRateUsdToBGN,
                    CustomerOrderProductDetailsId = customerOrderProduct.Id,                   
                };
              
                var quantityM3 = productService.ConvertStringSizeToQubicMeters(size);
               
                invoiceProduct.QuantityM3ForCalc = quantityM3 * invoiceProduct.Pallets * invoiceProduct.SheetsPerPallet;

                //if (products.All(u => u.Unit == Unit.m3) || products.All(u => u.Unit == Unit.m2)
                //  || products.All(u => u.Unit == Unit.pcs) || products.All(u => u.Unit == Unit.sheets)
                //  || products.All(u => u.Unit == Unit.m))
                //{
                //    invoiceCreate.TotalQuantity = (decimal)products
                //        .Sum(a => a.InvoicedQuantity);
                //}
                //else              
                //{
                    if (invoiceProduct.Unit == Data.Enums.Unit.m3)
                    {
                        invoiceCreate.TotalQuantity += invoiceProduct.InvoicedQuantity;
                    }
                    else if(!String.Equals(size, "-"))
                    {        
                        invoiceCreate.TotalQuantity += invoiceProduct.QuantityM3ForCalc;
                    }              

                invoiceProduct.TotalSheets = invoiceProduct.Pallets * invoiceProduct.SheetsPerPallet;               
                invoiceProduct.Amount = invoiceProduct.SellPrice * invoiceProduct.InvoicedQuantity;
                invoiceProduct.BgAmount = invoiceProduct.BgPrice * invoiceProduct.InvoicedQuantity;              
                mainProduct.InvoiceProductDetails.Add(invoiceProduct);
                invoiceCreate.InvoiceProducts.Add(invoiceProduct);                
            }

          await  dbContext.SaveChangesAsync();

            foreach (var product in invoiceCreate.InvoiceProducts)
            {
                var purchaseProductDetail = dbContext.PurchaseProductDetails
                    .Where(a => a.Id == product.PurchaseProductDetailsId).FirstOrDefault();

                var mainProduct = productRepository.GetMainProduct(product.ProductId);
                var size = productService.GetSizeName(mainProduct.SizeId);               

                product.DeliveryCost = productService.CalculateDeliveryCostOfTheProductInCo
                   (product.InvoicedQuantity,product.QuantityM3ForCalc, invoiceCreate.TotalQuantity, 
                   deliveryCost, product.Unit, size);
              

                var expensesFactoringPerProduct = ((factoring * invoiceCreate.TotalAmount) / 100)
                    / invoiceCreate.TotalQuantity * product.QuantityM3ForCalc;

                var comissionExpenses = (comission/invoiceCreate.TotalQuantity)*product.QuantityM3ForCalc;

                product.Profit = (decimal)((product.Amount - (purchaseProductDetail.CostPrice* product.InvoicedQuantity)) - product.DeliveryCost - expensesFactoringPerProduct - comissionExpenses);
            }

                if(serviceProducts.Count > 0)
            {
                foreach (var serviceProduct in serviceProducts)
                {
                    var product = dbContext.Products
                        .Where(a=>a.DescriptionId == serviceProduct.DescriptionId 
                        && a.SizeId == serviceProduct.SizeId
                        && a.GradeId == serviceProduct.GradeId)
                        .FirstOrDefault();                    

                    if(product == null)
                    { 
                        product = new Product
                        {
                            DescriptionId = serviceProduct.DescriptionId,
                            SizeId = serviceProduct.SizeId,
                            GradeId = serviceProduct.GradeId,
                            DocumentId = invoiceCreate.Id,
                            InvoiceProductDetails = new List<InvoiceProductDetails>(),
                            SupplierOrderId = null
                        };
                      dbContext.Products.Add(product);                       
                    }

                    var newServiceProduct = new InvoiceProductDetails
                    {
                        ProductId = product.Id,
                        InvoicedQuantity = serviceProduct.InvoicedQuantity,
                        Unit = serviceProduct.Unit,
                        VehicleNumber = serviceProduct.VehicleNumber,                       
                        CustomerOrderId = customerOrders.Select(i => i.Id).FirstOrDefault(),
                        Amount = serviceProduct.InvoicedQuantity * serviceProduct.SellPrice,
                        InvoiceId = invoiceCreate.Id,
                        Pallets = 0,
                        SheetsPerPallet = 0,
                        TotalSheets = 0,
                        FscCertificate = "-",
                        FscClaim = "-",
                        BgPrice = serviceProduct.SellPrice* currencyExchangeRateUsdToBGN,
                        BgAmount = serviceProduct.InvoicedQuantity * serviceProduct.SellPrice*currencyExchangeRateUsdToBGN,
                        Profit = serviceProduct.InvoicedQuantity * serviceProduct.SellPrice,
                        CustomerOrderProductDetailsId = null,
                        PurchaseProductDetailsId = null,
                        SellPrice = serviceProduct.SellPrice
                    };

                    await dbContext.InvoiceProductDetails.AddAsync(newServiceProduct);
                }
            }
           
            await dbContext.SaveChangesAsync();

            foreach (var customerOrder in customerOrders)
            {
                customerOrdersSrvice.CheckCustomerOrderStatus(customerOrder.Id);
            }

            var invoiceForPrint = this.mapper.Map<InvoicePrintViewModel>(invoiceCreate);

            invoiceForPrint.FiscalAgentName = dbContext.FiscalAgents
                .Where(i=>i.Id == fiscalAgent)
                .Select(s=>s.Name)
                .FirstOrDefault();

            invoiceForPrint.FiscalAgentDetail = dbContext.FiscalAgents
                .Where(i => i.Id == fiscalAgent)
                .Select(s => s.Details)
                .FirstOrDefault();
          
            invoiceForPrint.CompanyBankDetails = new List<InvoiceBankDetailsViewModel>();

            invoiceForPrint.Currency = dbContext.Currencies
                .Where(i => i.Id == currencyId)
                .Select(n => n.Name)
                .FirstOrDefault();

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
                        City = a.Address.City,
                        Country = a.Address.Country,
                        Street = a.Address.Street
                    }
                }).FirstOrDefault();

            invoiceForPrint.Customer = customerDetails;
            invoiceForPrint.OrderConfirmationNumbers = (List<int>) selectedCustomerOrderNumbers;
            invoiceForPrint.CustomerPoNumbers = customerOrders.Select(o=>o.CustomerPoNumber).ToList();  
           
            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencies
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

            invoiceCreate.MyCompanyId = myCompanyId;

            invoiceForPrint.Seller = new MyCompanyForInvoicePrint
            {
                Name = myCompany.Name,
                City = addressCompany.City,
                Country = addressCompany.Country,
                Street = addressCompany.Street,
                EIK = myCompany.Eik,
                FSCClaim = myCompany.FscClaim,
                FSCSertificate = myCompany.FscSertificate,
                RepresentativePerson = myCompany.RepresentativePerson,
                VAT = myCompany.VAT
            };

            if (fscText != null)
            {
                invoiceForPrint.FscTextEng = dbContext.FscTexts
                    .Where(i => i.Id == fscText)
                    .Select(n => n.FscTextEng).FirstOrDefault();
                invoiceForPrint.FscCertificate = myCompany.FscSertificate;
            }

            invoiceForPrint.Products = this.mapper.Map<ICollection<ProductsForInvoiceModel>>(products.Where(a => a.InvoicedQuantity != 0).ToList());
            invoiceForPrint.ServiceProducts = this.mapper.Map<ICollection<ServiceProductForInvoiceFormModel>>(serviceProducts.Where(q=>q.InvoicedQuantity !=0).ToList());   
            
            foreach (var product in invoiceForPrint.Products)
            {                
                product.Description = productService.GetDescriptionName(product.DescriptionId);
                product.Grade = productService.GetGradeName(product.GradeId);
                product.Size = productService.GetSizeName(product.SizeId);
            }

            foreach (var product in invoiceForPrint.ServiceProducts)
            {
                product.Description = productService.GetDescriptionName(product.DescriptionId);
                product.Grade = productService.GetGradeName(product.GradeId);
                product.Size = productService.GetSizeName(product.SizeId);
            }

          //  dbContext.SaveChanges();            
            
            CreatePackingListForThisInvoice(invoiceCreate.Id);
            documentService.CreateBgInvoice(invoiceCreate.Id,invoiceCreate.MyCompanyId);
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
                TruckNumber = invoice.TruckNumber,
                Incoterms = invoice.Incoterms,
                MyCompanyId = invoice.MyCompanyId,
                NetWeight = invoice.NetWeight,
                GrossWeight = invoice.GrossWeight,
                CurrencyId = invoice.CurrencyId,
                CustomerOrders = invoice.CustomerOrders,              
                TotalQuantity= invoice.TotalQuantity,
                DeliveryAddress = invoice.DeliveryAddress,
                LoadingAddress = invoice.LoadingAddress,
                HeaderId = invoice.HeaderId,
                FooterId = invoice.FooterId
            };

            dbContext.Documents.Add(packingList);
            dbContext.SaveChanges();
        }
        public void EditPackingList(int id)
        {
            if (id == 0) return;

            var invoice = dbContext.Documents
               .Where(i => i.Id == id)
               .FirstOrDefault();

            var packingList = dbContext.Documents
                .Where(n => n.DocumentNumber == invoice.DocumentNumber & n.DocumentType == Data.Enums.DocumentTypes.PackingList)
                .FirstOrDefault();

            packingList.Date = invoice.Date;
            packingList.CustomerId = invoice.CustomerId;
            packingList.CustomerOrders = invoice.CustomerOrders;
           // packingList.InvoiceProducts = invoice.InvoiceProducts;
            packingList.TruckNumber = invoice.TruckNumber;
            packingList.Incoterms = invoice.Incoterms;
            packingList.MyCompanyId = invoice.MyCompanyId;
            packingList.GrossWeight = invoice.GrossWeight;
            packingList.NetWeight = invoice.NetWeight;           
            packingList.DeliveryAddress = invoice.DeliveryAddress;
            packingList.LoadingAddress = invoice.LoadingAddress;

            dbContext.SaveChanges();
        }
        public EditInvoicePaymentModel InvoiceForEditByNumber(int documentNumber)
        {
            var invoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber && i.DocumentType == DocumentTypes.Invoice);
            if (invoice == null) { return null; }

            var invoiceForPayment = invoice
                .Select(n => new EditInvoicePaymentModel
                {   
                    Id = n.Id,
                    DocumentNumber = n.DocumentNumber,
                    Date = n.Date,
                    PaidAvance = n.PaidAvance,
                    Balance = n.Balance,
                    DatePaidAmount = (DateTime)n.DatePaidAmount,
                    PaidStatus = n.PaidStatus
                }).FirstOrDefault();

            var checkOrder = dbContext.Documents
                .Where(a=>a.Id == invoiceForPayment.Id)
                .Select(c=>c.CustomerOrders)
                .FirstOrDefault();

            var customerOrders = checkOrder
                .Select(i=> new CustomerOrdersNumbersCollection
                {
                    Id= i.Id,
                    OrderConfirmationNumber= i.OrderConfirmationNumber
                })
                .ToList();

            invoiceForPayment.CustomerOrders = customerOrders;

            return invoiceForPayment;
        }
        public bool EditInvoicePayment
            (int documentNumber, bool paidStatus, decimal paidAdvance, DateTime? datePaidAmount, int customerOrderId)
        {
            if (documentNumber == 0)
            {
                return false;
            }

            var invoice = dbContext.Documents
                .Where(i => i.DocumentNumber == documentNumber && i.DocumentType == DocumentTypes.Invoice)
                .FirstOrDefault();

            var customerOrders = dbContext.CustomerOrders
                .Where(i => i.Id == customerOrderId)
                .ToList();

            if(customerOrders != null )
            {
                foreach (var customerOrder in customerOrders)
                {
                    customerOrder.Balance -= paidAdvance;
                    if (customerOrder.Balance < 0.001m)
                    {
                        customerOrder.PaidAmountStatus = true;
                    }
                }              
            }

            var payment = new Payment
            {  
                CurrencyId = invoice.CurrencyId,
                DocumentId = invoice.Id,
                Date = (DateTime)datePaidAmount,
                PaidAmount = paidAdvance
            };

            dbContext.Payments.Add(payment);
            invoice.Payments.Add(payment);

            invoice.PaidStatus = paidStatus;

            invoice.Balance = invoice.TotalAmount - paidAdvance;

            if (invoice.Balance > 0.001m)
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
                .Where(i => i.DocumentNumber == documentNumber && 
                (i.DocumentType == DocumentTypes.Invoice 
                || i.DocumentType == DocumentTypes.CreditNote 
                || i.DocumentType == DocumentTypes.DebitNote))
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

            var products = dbContext.InvoiceProductDetails
                .Where(i => i.InvoiceId == invoice.Id || 
                i.CreditNoteId == invoice.Id || 
                i.DebitNoteId == invoice.Id);

            var agent = dbContext.FiscalAgents
                .Where(i => i.Id == invoice.FiscalAgentId)
                .FirstOrDefault();

            var bgProducts = mapper.ProjectTo<BGProductsForBGInvoiceViewModel>(products).ToList();

            var bgInvoiceForPrint = new BgInvoiceViewModel
            {
                DocumentNumber = documentNumber,
                CreditToInvoiceNumber = invoice.CreditToInvoiceId,
                CreditToInvoiceDate = invoice.CreditToInvoiceDate,
                DebitToInvoiceNumber = invoice.DebitToInvoiceId,
                DebitToInvoiceDate = invoice.DebitToInvoiceDate,
                DocumentType = invoice.DocumentType.ToString(),
                Date = invoice.Date,
                Amount = invoice.Amount * currencyExchange,
                Vat = invoice.Vat,
                VatAmount = invoice.Amount * currencyExchange * invoice.Vat / 100 ?? 0,    
                CurrencyExchangeRateUsdToBGN = invoice.CurrencyExchangeRateUsdToBGN,
                DealTypeBg= invoice.DealTypeBg,
                DealDescriptionBg= invoice.DealDescriptionBg,               
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
                },
                PlaceOfIssue = invoice.PlaceOfIssue
            };

            if(agent != null)
            {
                bgInvoiceForPrint.FiscalAgentName = agent.BgName;
                bgInvoiceForPrint.FiscalAgentDetail = agent.BgDetails;
            }

            if(invoice.FscTextId!= null)
            {
                bgInvoiceForPrint.FscTextBg = dbContext.FscTexts
                    .Where(i => i.Id == invoice.FscTextId)
                    .Select(n => n.FscTextBg)
                    .FirstOrDefault();

                bgInvoiceForPrint.FscCertificate = mycompany.FscSertificate;
            }

            if(bgInvoiceForPrint.DocumentType == "CreditNote")
            {
                bgInvoiceForPrint.TotalAmount = invoice.CreditNoteTotalAmount * currencyExchange;
            }else if(bgInvoiceForPrint.DocumentType == "DebitNote")
            {
                bgInvoiceForPrint.TotalAmount = invoice.DebitNoteTotalAmount * currencyExchange;
            }
            else
            {
                bgInvoiceForPrint.TotalAmount = invoice.TotalAmount * currencyExchange;
            }

            bgInvoiceForPrint.CompanyBankDetails = new List<InvoiceBankDetailsViewModel>();

            var bankList = dbContext.BankDetails
                .Where(c => c.CompanyId == mycompanyId)
                .ToList();

            foreach (var bank in bankList)
            {
                var currency = dbContext.Currencies
                    .Where(i => i.Id == bank.CurrencyId)
                    .Select(n => n.Name)
                    .FirstOrDefault();
                if(currency != "BGN") { continue; }
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
                var mainPproduct = productRepository.GetMainProduct(product.ProductId);

                product.BgDescription = dbContext.Descriptions
                    .Where(i => i.Id == mainPproduct.DescriptionId)
                    .Select(bgN => bgN.BgName)
                    .FirstOrDefault();

                product.Grade = dbContext.Grades
                    .Where(i => i.Id == mainPproduct.GradeId)
                    .Select(bgN => bgN.Name)
                    .FirstOrDefault();

                product.Size = dbContext.Sizes
                    .Where(i => i.Id == mainPproduct.SizeId)
                    .Select(bgN => bgN.Name)
                    .FirstOrDefault();

                if (product.Unit == Data.Enums.Unit.pcs.ToString() || product.Unit == Data.Enums.Unit.sheets.ToString())
                {
                    product.InvoicedQuantity = Math.Round(product.InvoicedQuantity, 0);
                }
                bgInvoiceForPrint.BgProducts.Add(product);
            }

            return bgInvoiceForPrint;
        }
        public EditInvoiceViewModel ViewEditInvoice(int id)
        {
            var invoice = dbContext.Documents.
                Include(a=>a.BankDetails)
               .Where(i => i.Id == id).FirstOrDefault();

            var fiscalAgent = dbContext.FiscalAgents
                .Where(i => i.Id == invoice.FiscalAgentId)
                .FirstOrDefault();

            var loggedUser = httpContextAccessor.ContextAccessUserId();
            var myCompany = dbContext.MyCompanies
              .Where(n => invoice.MyCompanyId == n.Id)
              .FirstOrDefault();

            if (loggedUser != myCompany.UserId) { return null; }

            if (id == 0)
            {
                return null;
            }
            var invoiceForEdit = new EditInvoiceViewModel
            {
                Currencies = currencyService.GetCurrencyList(),
                CurrencyExchangeRate = invoice.CurrencyExchangeRateUsdToBGN,
                Date = invoice.Date,
                GrossWeight = invoice.GrossWeight,
                NetWeight = invoice.NetWeight,
                DeliveryCost = invoice.DeliveryTrasnportCost,
                CurrencyId = invoice.CurrencyId,
                TruckNumber = invoice.TruckNumber,
                Products = new List<EditProductForCompanyInvoicesViewModel>(),
                DocumentType = invoice.DocumentType.ToString(),
                CompanyBankDetails = new List<BankDetailsViewModel>(),
                PaymentTerms = invoice.PaymentTerms,
                FiscalAgents = documentService.GetFiscalAgents(),
                FscTexts = fscTextService.GetAllFscTexts(),
                LoadingAddress = invoice.LoadingAddress,
                DeliveryAddress = invoice.DeliveryAddress,
                Images = imageService.ImageCollection(myCompany.Id),
                Factoring = invoice.Factoring,
                Comission = invoice.ProcentComission,
                MyCompanyId = invoice.MyCompanyId
              };

            if(fiscalAgent != null)
            {
                invoiceForEdit.FiscalAgentName = fiscalAgent.Name;
                invoiceForEdit.FiscalAgentId = fiscalAgent.Id;
            }
            
            if(invoice.FscTextId != null)
            {
                invoiceForEdit.FscText = dbContext.FscTexts
                    .Where(i => i.Id == invoice.FscTextId)
                    .Select(n => n.FscTextEng)
                    .FirstOrDefault();
            }

            foreach (var bank in dbContext.BankDetails.ToList())
            {
                invoiceForEdit.CompanyBankDetails.Add(new BankDetailsViewModel
                {
                    Id = bank.Id,
                    BankName = bank.BankName,
                    CurrencyId = bank.CurrencyId,
                    Iban = bank.Iban,
                    Swift = bank.Swift,
                    CurrencyName = dbContext.Currencies
                         .Where(i => i.Id == bank.CurrencyId).Select(n => n.Name).FirstOrDefault()
                });
            }

            var productsList = dbContext.InvoiceProductDetails
                   .Where(cd => cd.InvoiceId == id);

            foreach (var product in productsList)
            {
                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var customerProductDetail = dbContext.CustomerOrderProductDetails
                .Where(i => i.InvoiceProductDetails.Select(i => i.Id).Contains(product.Id))
                .Select(i => i.Id)
                .FirstOrDefault();

                var productForEdit = new EditProductForCompanyInvoicesViewModel
                {
                    Id = product.Id,
                    Description = productService.GetDescriptionName(mainProduct.DescriptionId),
                    DescriptionId = mainProduct.DescriptionId,
                    Grade = productService.GetGradeName(mainProduct.GradeId),
                    Size = productService.GetSizeName(mainProduct.SizeId),
                    Descriptions = productService.DescriptionIdAndNameList(),
                    Grades = productService.GradeIdAndNameList(),
                    Sizes = productService.SizeIdAndNameList(),
                    Unit = product.Unit,
                    FscClaim = product.FscClaim,
                    FscSertificate = product.FscCertificate,
                    Amount = product.Amount,
                    BgAmount = product.BgAmount,
                    BgPrice = product.BgPrice,                    
                    CustomerOrderId = product.CustomerOrderId ?? 0,                    
                    DeliveryCost = product.DeliveryCost,
                    InvoicedQuantity = product.InvoicedQuantity,
                    Pallets = product.Pallets,
                    SellPrice = product.SellPrice,
                    SheetsPerPallet = product.SheetsPerPallet,
                    PurchaseProductDetailsId = product.PurchaseProductDetailsId ?? 0,                   
                    GradeId = mainProduct.GradeId,
                    SizeId = mainProduct.SizeId,
                    VehicleNumber = product.VehicleNumber,
                    ProductId = product.ProductId,
                    TotalSheets = product.TotalSheets,
                    CustomerProductDetailId = customerProductDetail,
                    HsCode = mainProduct.HsCode
                };
                if (product.Unit == Data.Enums.Unit.pcs || product.Unit == Data.Enums.Unit.sheets)
                {
                    product.InvoicedQuantity = Math.Round(product.InvoicedQuantity, 0);
                }
                invoiceForEdit.Products.Add(productForEdit);
            }            
            return invoiceForEdit;
        }
        public async Task<bool> EditInvoice
            (int id, decimal currencyExchangeRate, DateTime date, decimal grossWeight, decimal netWeight,
            decimal deliveryCost, int orderConfirmationNumber, string truckNumber,
            ICollection<EditProductForCompanyInvoicesViewModel> products,
            string incoterms, string comment, List<int> banks, int? fiscalAgentId, 
            int? fscText, string paymentTerms, string deliveryAddress, 
            string loadingAddress, decimal? factoring, decimal? comission)
        {
            if (id == 0) return false;

            var invoice = dbContext.Documents               
               .Where(i => i.Id == id)
               .Include(s=>s.BankDetails)             
               .FirstOrDefault();

            var bankDetails = dbContext.BankDetails
               .Where(i => banks.Contains(i.Id))
               .ToList();

            invoice.CurrencyExchangeRateUsdToBGN = currencyExchangeRate;
            invoice.Date = date;
            invoice.GrossWeight = grossWeight;
            invoice.NetWeight = netWeight;
            invoice.DeliveryTrasnportCost = deliveryCost;
            invoice.TruckNumber = truckNumber;
            invoice.Amount = 0;
            invoice.Incoterms = incoterms; 
            invoice.Comment = comment;
            invoice.TotalQuantity= 0;         
            invoice.FiscalAgentId= fiscalAgentId;
            invoice.FscTextId = fscText;
            invoice.PaymentTerms= paymentTerms;
            invoice.DeliveryAddress = deliveryAddress;
            invoice.LoadingAddress = loadingAddress;
            invoice.Factoring = (decimal)factoring;
            invoice.ProcentComission = (decimal)comission;

            var oldBanks = invoice.BankDetails.ToList();

            oldBanks?.ForEach(i => invoice.BankDetails.Remove(i));

            bankDetails.ForEach(i => invoice.BankDetails.Add(i));

            foreach (var product in products)
                {                    
                var productForEdit = dbContext.InvoiceProductDetails.
                        Where(co => co.Id == product.Id)
                        .FirstOrDefault();

                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                var size = productService.GetSizeName(mainProduct.SizeId);
                    
                var customerOrderDetail = dbContext.CustomerOrderProductDetails
                        .Where(a => a.Id == productForEdit.CustomerOrderProductDetailsId)
                        .FirstOrDefault();

                if (customerOrderDetail != null)
                {
                    customerOrderDetail.AutstandingQuantity += productForEdit.InvoicedQuantity;
                    customerOrderDetail.AutstandingQuantity -= product.InvoicedQuantity;
                }

                productForEdit.VehicleNumber = product.VehicleNumber;
                productForEdit.Pallets = product.Pallets;
                productForEdit.SheetsPerPallet = product.SheetsPerPallet;
                productForEdit.TotalSheets = product.Pallets * product.SheetsPerPallet;
                productForEdit.InvoicedQuantity = product.InvoicedQuantity;
                productForEdit.Amount = product.InvoicedQuantity * product.SellPrice;
                productForEdit.BgPrice = product.SellPrice * currencyExchangeRate;
                productForEdit.BgAmount = product.BgPrice * product.InvoicedQuantity;

                productService.ClearProductQuantityWhenDealIsFinished
                    (product.ProductId, product.InvoicedQuantity, productForEdit.InvoicedQuantity);

                if(size != "-")
                {
                    productForEdit.QuantityM3ForCalc = productService.QuantityM3Conversion
                                        (productForEdit.Unit.ToString(), size, productForEdit.TotalSheets, product.InvoicedQuantity);
                }

                var customerOrder = dbContext.CustomerOrders
                    .Where(a => a.Id == productForEdit.CustomerOrderId)
                    .FirstOrDefault();
                //proveryavame dali vsichko po CO e veche invoiced i mu smenyame status
                if (productForEdit.CustomerOrderProductDetailsId != null)
                {
                    var customerOrderProductsSum = customerOrder.CustomerOrderProducts
                                        .Where(a => a.CustomerOrderId == customerOrder.Id)
                                        .Sum(a => a.AutstandingQuantity);

                    if (customerOrderProductsSum == 0)
                    {
                        customerOrder.StatusId = dbContext.Statuses
                        .Where(s => s.Name == "Finished")
                        .Select(i => i.Id)
                        .FirstOrDefault();
                    }
                    else
                    {
                        customerOrder.StatusId = dbContext.Statuses
                        .Where(s => s.Name == "Active")
                        .Select(i => i.Id)
                        .FirstOrDefault();
                    }
                }

                invoice.TotalQuantity += productForEdit.QuantityM3ForCalc;                   
                    
                invoice.Amount += productForEdit.Amount;                       
                 
                await dbContext.SaveChangesAsync();
            }

            foreach (var item in products)
            {
                var updatedProduct = dbContext.InvoiceProductDetails
                        .Where(co => co.Id == item.Id)
                        .FirstOrDefault();

                var mainProduct = productRepository.GetMainProduct(item.ProductId);

                var size = productService.GetSizeName(mainProduct.SizeId);

                if (updatedProduct.QuantityM3ForCalc > 0.001m)
                {
                    var purchaseProductDetail = dbContext.PurchaseProductDetails
                        .Where(a => a.Id == item.PurchaseProductDetailsId)
                        .FirstOrDefault(); 

                    updatedProduct.DeliveryCost = productService.CalculateDeliveryCostOfTheProductInCo
                        (updatedProduct.InvoicedQuantity, updatedProduct.QuantityM3ForCalc, invoice.TotalQuantity,
                        deliveryCost, updatedProduct.Unit, size);

                    var expensesFactoringPerProduct = (((factoring * invoice.TotalAmount) / 100)
                   / invoice.TotalQuantity) * updatedProduct.QuantityM3ForCalc;

                    var comissionExpenses = (comission / invoice.TotalQuantity) * updatedProduct.QuantityM3ForCalc;

                    updatedProduct.Profit = (decimal)((updatedProduct.Amount - (purchaseProductDetail.CostPrice * updatedProduct.InvoicedQuantity)) - updatedProduct.DeliveryCost - expensesFactoringPerProduct - comissionExpenses);
                }
            }
                invoice.VatAmount = invoice.Amount*invoice.Vat/100;
                invoice.TotalAmount = (decimal)(invoice.Amount + invoice.VatAmount);

                  await dbContext.SaveChangesAsync();  
           
            EditPackingList(invoice.Id);
            documentService.EditBgInvoice(invoice.DocumentNumber,invoice.MyCompanyId);
            return true;
        }
        public ICollection<CustomerCollectionForChoosingNewOrderForInvoiceEditViewModel> CustomersForeEditInvoice()
        {
           return dbContext.Customers
                .Select(i=> new CustomerCollectionForChoosingNewOrderForInvoiceEditViewModel
                {
                     Id= i.Id,
                     Name= i.Name,
                }).ToList();
        }

        public ICollection<PackingListJsonCollection> GetPackingListNumbers(int id)
        => dbContext.Documents
            .Where(type => type.DocumentType == Data.Enums.DocumentTypes.PackingList && type.MyCompanyId == id)
            .Select(num => new PackingListJsonCollection
            {
                PackingListId = num.Id,
                PackingListNumber = num.DocumentNumber
            })
            .ToList();       
    }
}
