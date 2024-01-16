using AutoMapper;
using DevExpress.Data.ODataLinq.Helpers;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Migrations;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.CreditNote;
using SSMO.Repository;
using SSMO.Services.Documents.Invoice;
using SSMO.Services.Images;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Documents.Credit_Note
{
    public class CreditNoteService : ICreditNoteService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IProductService productService;
        private readonly IDocumentService documentService;
        private readonly IProductRepository productRepository;
        private readonly IImageService  imageService;
        public CreditNoteService(ApplicationDbContext dbContex, IDocumentService documentService, 
            IProductService productService, IProductRepository productRepository,IImageService imageService)
        {
            this.dbContext = dbContex;
            this.documentService = documentService;
            this.productService = productService;
            this.productRepository = productRepository; 
            this.imageService = imageService;
        }       
        public CreditAndDebitNoteViewModel CreateCreditNote
            (int invoiceId, DateTime date, bool quantityBack, 
            string deliveryAddress, List<AddProductsToCreditAndDebitNoteFormModel> products, string paymentTerms)
        {
            var invoiceForCredit = dbContext.Documents
                .Include(a=>a.BankDetails)
                .Where(n => n.Id == invoiceId && n.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();  

            var creditNote = new Document
            {
                DocumentNumber = dbContext.Documents
                .OrderByDescending(n => n.DocumentNumber).Select(num => num.DocumentNumber).First() + 1,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.CreditNote,
                CreditToInvoiceDate = invoiceForCredit.Date,
                CreditToInvoiceId = invoiceForCredit.Id,
                SupplierId = invoiceForCredit.SupplierId,               
                CurrencyId = invoiceForCredit.CurrencyId,
                CurrencyExchangeRateUsdToBGN = invoiceForCredit.CurrencyExchangeRateUsdToBGN,
                CustomerId = invoiceForCredit.CustomerId,
                MyCompanyId = invoiceForCredit.MyCompanyId,
                Vat = invoiceForCredit.Vat,
                InvoiceProducts = new List<InvoiceProductDetails>(),
                CreditNoteDeliveryAddress = deliveryAddress,               
                CreditAndDebitNoteProducts = new List<Product>(),
                BankDetails = invoiceForCredit.BankDetails,
                PaymentTerms = paymentTerms,
                HeaderId= invoiceForCredit.HeaderId,
                FooterId = invoiceForCredit.FooterId
            };

            var customerOrdersByInvoice = dbContext.CustomerOrders
                .Where(a => a.Documents
                .Select(a => a.Id).Contains(invoiceForCredit.Id)).ToList();

            creditNote.CustomerOrders= customerOrdersByInvoice;

            dbContext.Documents.Add(creditNote);
            dbContext.SaveChanges();
            
            foreach (var product in products)
            {
                var mainProductList = dbContext.Products
                    .Where(d => d.DescriptionId == product.DescriptionId
                    && d.GradeId == product.GradeId
                    && d.SizeId == product.SizeId 
                    && d.LoadedQuantity > 0 && d.LoadedQuantity >= product.Quantity)
                    .ToList();

               var existProduct = dbContext.InvoiceProductDetails
                    .Where(a=> mainProductList.Select(a=>a.Id)
                    .Contains(a.ProductId) && a.InvoiceId == invoiceForCredit.Id)
                    .FirstOrDefault();

                if(existProduct != null)
                {
                    var mainProduct = productRepository.GetMainProduct(existProduct.ProductId);
                    //unity na credit note tryabva da e edin i sasht s fakturiraniya
                    if (product.Quantity > existProduct.InvoicedQuantity)
                    {
                        return null;
                    }

                    existProduct.CreditNoteId = creditNote.Id;
                    existProduct.CreditNotePallets = product.Pallets;
                    existProduct.CreditNotePrice = product.Price;
                    existProduct.CreditNoteSheetsPerPallet = product.SheetsPerPallet;
                    existProduct.CreditNoteQuantity = product.Quantity;
                    existProduct.CreditNoteProductAmount = product.Quantity * product.Price;
                    existProduct.CreditNoteBgPrice = product.Price * creditNote.CurrencyExchangeRateUsdToBGN;
                    existProduct.CreditNoteBgAmount = product.Price * creditNote.CurrencyExchangeRateUsdToBGN * product.Quantity;
                    existProduct.CreditNoteTotalSheets = product.Pallets * product.SheetsPerPallet;
                    creditNote.Amount += product.Price * product.Quantity;
                    //creditNote.TotalQuantity += product.Quantity;

                    if (quantityBack == true)
                    {  if(mainProduct.Unit.ToString() != product.Unit)
                        {
                            var size = productService.GetSizeName(mainProduct.SizeId);

                            product.Quantity =productService.ConvertUnitQuantityToDiffUnitQuantity
                                (mainProduct.Unit.ToString(), product.Unit, product.Quantity, size, existProduct.TotalSheets);
                        }
                        mainProduct.QuantityAvailableForCustomerOrder += product.Quantity;
                        //todo soldquantity unit ako e razlichno
                        mainProduct.SoldQuantity -= product.Quantity;
                        var customerOrderProduct = dbContext.CustomerOrderProductDetails
                            .Where(i => i.ProductId == mainProduct.Id && i.CustomerOrderId == existProduct.CustomerOrderId)
                            .FirstOrDefault();

                        //TODO dali se vrashta v austanding ili nie rachno pravim edit na CO. moje da ne se vrashta nishto ???
                         customerOrderProduct.AutstandingQuantity += product.Quantity;
                    }                    
                }
                else
                {
                    var newProduct = dbContext.Products
                           .Where(d => d.DescriptionId == product.DescriptionId
                           && d.GradeId == product.GradeId
                           && d.SizeId == product.SizeId)
                           .FirstOrDefault();

                    if (newProduct == null)
                    {
                        newProduct = new Product
                        {
                            DescriptionId = product.DescriptionId,
                            GradeId = product.GradeId,
                            SizeId = product.SizeId,
                            DocumentId = creditNote.Id,
                            InvoiceProductDetails = new List<InvoiceProductDetails>(),
                            SupplierOrderId = null
                        };
                        dbContext.Products.Add(newProduct);
                        dbContext.SaveChanges();    
                    }

                    var invoiceProduct = new InvoiceProductDetails
                    {
                        Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), product.Unit, true),
                        CreditNotePallets = product.Pallets,
                        CreditNotePrice = product.Price,
                        CreditNoteQuantity = product.Quantity,
                        CreditNoteSheetsPerPallet = product.SheetsPerPallet,
                        FscClaim = product.FscClaim,
                        FscCertificate = product.FscSertificate,
                        CreditNoteProductAmount = product.Price * product.Quantity,
                        CreditNoteBgAmount = product.Price * creditNote.CurrencyExchangeRateUsdToBGN * product.Quantity,
                        CreditNoteBgPrice = product.Price * creditNote.CurrencyExchangeRateUsdToBGN,
                        CreditNoteId = creditNote.Id,
                        CustomerOrderId = creditNote.CustomerOrders.Select(a => a.Id).First(),
                        InvoiceId = creditNote.Id,
                        CreditNoteTotalSheets = product.Pallets * product.SheetsPerPallet
                };
                  
                    newProduct.InvoiceProductDetails.Add(invoiceProduct);

                    creditNote.Amount += product.Price * product.Quantity;
                    creditNote.CreditAndDebitNoteProducts.Add(newProduct);
                   // creditNote.TotalQuantity += product.Quantity;
                }
            }                
            
            creditNote.VatAmount = creditNote.Amount * creditNote.Vat / 100;
            creditNote.CreditNoteTotalAmount = creditNote.Amount + creditNote.VatAmount ?? 0;

            dbContext.SaveChanges();
           
            var invoiceForCreditBalance = invoiceForCredit.Balance;
            invoiceForCredit.CreditNoteTotalAmount = creditNote.CreditNoteTotalAmount;

            if (invoiceForCreditBalance > 0)
            {               
                invoiceForCreditBalance -= creditNote.CreditNoteTotalAmount;
            }

            dbContext.SaveChanges();

            var creditNoteForPrint = documentService.PrintCreditAndDebitNote(creditNote.Id);
            documentService.CreateBgInvoice(creditNote.Id,creditNote.MyCompanyId);

            return creditNoteForPrint;
        }

        public bool EditCreditNote
            (int id, DateTime date, string incoterms, string truckNumber, decimal netWeight, 
            decimal grossWeight, decimal deliveryCost, decimal currencyExchangeRate, string comment, 
            IList<EditProductForCreditNoteViewModel> products, string paymentTerms, int invoiceId)
        {
            if(id == 0) { return false; }

            var creditNote = dbContext.Documents
                .Include(b => b.BankDetails)
                .Where(i => i.Id == id)
                .FirstOrDefault();               

            creditNote.Date = date;
            creditNote.Incoterms= incoterms;
            creditNote.TruckNumber= truckNumber;
            creditNote.NetWeight= netWeight;
            creditNote.GrossWeight= grossWeight;
            creditNote.DeliveryTrasnportCost= deliveryCost;
            creditNote.CurrencyExchangeRateUsdToBGN = currencyExchangeRate;
            creditNote.Comment= comment;
            creditNote.Amount = 0m;
            creditNote.PaymentTerms= paymentTerms;  
            creditNote.CreditToInvoiceId= invoiceId;

            foreach (var product in products)
            {
                var productForEdit = dbContext.InvoiceProductDetails
                    .Where(a => a.Id == product.Id)
                    .FirstOrDefault();

                var mainProduct = productRepository.GetMainProduct(product.ProductId);

                mainProduct.QuantityAvailableForCustomerOrder += product.CreditNoteQuantity-productForEdit.CreditNoteQuantity;
                mainProduct.SoldQuantity -= product.CreditNoteQuantity + productForEdit.CreditNoteQuantity;
                //TODO sold q ne pokazva vqrno ako stava vapros za usluga , a ne produkt
                productForEdit.CreditNotePallets = product.CreditNotePallets;
                productForEdit.CreditNoteSheetsPerPallet = product.CreditNoteSheetsPerPallet;
                productForEdit.CreditNoteQuantity = product.CreditNoteQuantity;
                productForEdit.Unit = product.Unit;
                productForEdit.FscCertificate = product.FscSertificate;
                productForEdit.FscClaim = product.FscClaim;
                productForEdit.CreditNoteTotalSheets = product.CreditNotePallets * product.CreditNoteSheetsPerPallet;
                productForEdit.CreditNotePrice = product.CreditNotePrice;
                productForEdit.CreditNoteBgPrice = product.CreditNotePrice * currencyExchangeRate;
                productForEdit.CreditNoteProductAmount = product.CreditNotePrice * product.CreditNoteQuantity;
                productForEdit.CreditNoteBgAmount = productForEdit.CreditNoteProductAmount * currencyExchangeRate;
                
                creditNote.Amount += product.CreditNotePrice * product.CreditNoteQuantity;

                    if (mainProduct.Unit != productForEdit.Unit && !mainProduct.Size.ToString().Equals("-"))
                    {
                        var size = productService.GetSizeName(mainProduct.SizeId);

                    productForEdit.CreditNoteQuantity = productService.ConvertUnitQuantityToDiffUnitQuantity
                            (mainProduct.Unit.ToString(), productForEdit.Unit.ToString(), product.CreditNoteQuantity, size, product.TotalSheets);
                    }
                    
                    var customerOrderProduct = dbContext.CustomerOrderProductDetails
                        .Where(i => i.ProductId == mainProduct.Id && i.CustomerOrderId == productForEdit.CustomerOrderId)
                        .FirstOrDefault();

                    //TODO dali se vrashta v austanding ili nie rachno pravim edit na CO. moje da ne se vrashta nishto ???
                     customerOrderProduct.AutstandingQuantity += product.CreditNoteQuantity;
              
                // creditNote.TotalQuantity += product.CreditNoteQuantity;               
            }

            creditNote.VatAmount = creditNote.Amount * creditNote.Vat / 100;
            creditNote.CreditNoteTotalAmount = creditNote.Amount + creditNote.VatAmount ?? 0;
            documentService.EditBgInvoice(creditNote.DocumentNumber,creditNote.MyCompanyId);

            dbContext.SaveChanges();
            return true;
        }
        public bool AddNewProductsToCreditNoteWhenEdit
            (int id, int invoiceId, 
            List<ProductForCreditNoteViewModelPerInvoice> productsFromInvoice, 
            List<NewProductsForCreditNoteViewModel> newPoducts)
        {
            var creditNote = dbContext.Documents
                .Find(id);

            if (creditNote == null) {return false; }

            var invoice = dbContext.Documents
                .Where(i=>i.Id== invoiceId)
                .FirstOrDefault();

           if(creditNote.CustomerId != invoice.CustomerId)
            {
                creditNote.CustomerId = invoice.CustomerId;
            }
            if (creditNote.CreditToInvoiceId != invoiceId)
            {
                creditNote.CreditToInvoiceId = invoiceId;
            }
            creditNote.CreditToInvoiceId= invoiceId;

            var customerOrders = dbContext.CustomerOrders
                .Where(a=>a.Documents.Select(i=>i.Id).Contains(invoiceId)).ToList();

            if(productsFromInvoice != null)
            {
                foreach (var product in productsFromInvoice)
                {
                    if(product.InvoicedQuantity > 0)
                    {
                        var productFromInvoice = dbContext.InvoiceProductDetails
                   .Where(a => a.InvoiceId == invoiceId && a.Id == product.Id)
                   .FirstOrDefault();

                        productFromInvoice.CreditNoteId = id;
                        productFromInvoice.CreditNoteQuantity = product.InvoicedQuantity;
                        productFromInvoice.CreditNotePallets = product.Pallets;
                        productFromInvoice.CreditNoteSheetsPerPallet = product.SheetsPerPallet;
                        productFromInvoice.CreditNoteTotalSheets = product.Pallets * product.SheetsPerPallet;
                        productFromInvoice.CreditNotePrice = product.SellPrice;
                        productFromInvoice.FscCertificate= product.FscCertificate;
                        productFromInvoice.FscClaim = product.FscClaim;                      
                        productFromInvoice.CreditNoteProductAmount = productFromInvoice.CreditNotePrice * productFromInvoice.CreditNoteQuantity;
                        productFromInvoice.CreditNoteBgPrice = productFromInvoice.CreditNotePrice * creditNote.CurrencyExchangeRateUsdToBGN;
                        productFromInvoice.CreditNoteBgAmount = productFromInvoice.CreditNoteProductAmount* creditNote.CurrencyExchangeRateUsdToBGN;
                        creditNote.Amount += productFromInvoice.CreditNoteProductAmount;                       
                    }                    
                }
            };

            if(newPoducts.Count > 0)
            {
                foreach (var product in newPoducts)
                {
                  var  newProduct = new Product
                    {
                        DescriptionId = product.DescriptionId,
                        GradeId = product.GradeId,
                        SizeId = product.SizeId,
                        DocumentId = creditNote.Id,
                        InvoiceProductDetails = new List<InvoiceProductDetails>(),
                        SupplierOrderId = null
                    };

                    var invoiceProduct = new InvoiceProductDetails
                    {
                        Unit = product.Unit,
                        CreditNotePallets = product.CreditNotePallets,
                        CreditNotePrice = product.CreditNotePrice,
                        CreditNoteQuantity = product.CreditNoteQuantity,
                        CreditNoteSheetsPerPallet = product.CreditNoteSheetsPerPallet,
                        FscClaim = product.FscClaim,
                        FscCertificate = product.FscCertificate,
                        CreditNoteProductAmount = product.CreditNotePrice * product.CreditNoteQuantity,
                        CreditNoteBgAmount = product.CreditNotePrice * creditNote.CurrencyExchangeRateUsdToBGN * product.CreditNoteQuantity,
                        CreditNoteBgPrice = product.CreditNotePrice * creditNote.CurrencyExchangeRateUsdToBGN,
                        CreditNoteId = creditNote.Id,
                        CustomerOrderId = customerOrders.Select(a => a.Id).First(),
                        InvoiceId = creditNote.Id,
                        CreditNoteTotalSheets = product.CreditNotePallets*product.CreditNoteSheetsPerPallet
                    };

                    invoiceProduct.TotalSheets = product.CreditNotePallets * product.CreditNoteSheetsPerPallet;                  
                    creditNote.Amount += invoiceProduct.CreditNoteProductAmount;
                    if(newPoducts.All(i=>i.Unit == Unit.m3) || newPoducts.All(i => i.Unit == Unit.m2)
                        || newPoducts.All(i => i.Unit == Unit.pcs) || newPoducts.All(i => i.Unit == Unit.sheets)
                        || newPoducts.All(i => i.Unit == Unit.m))
                    {
                        creditNote.TotalQuantity += invoiceProduct.CreditNoteQuantity;
                    }
                    
                    dbContext.Products.Add(newProduct);
                    dbContext.SaveChanges();
                    newProduct.InvoiceProductDetails.Add(invoiceProduct);
                }
            }
            creditNote.VatAmount = creditNote.Amount * creditNote.Vat;
            creditNote.TotalAmount = creditNote.Amount + creditNote.VatAmount ?? 0;         

            invoice.CreditNoteTotalAmount = creditNote.TotalAmount;
            
            dbContext.SaveChanges();
            documentService.EditBgInvoice(creditNote.DocumentNumber, creditNote.MyCompanyId);

            return true;
        }

        public List<InvoiceNumbersForEditedCreditNoteViewModel> InvoiceNumbers()
        {
            var invoiceNumbers = dbContext.Documents
                .Where(x => x.DocumentType == DocumentTypes.Invoice)
                .Select(n=> new InvoiceNumbersForEditedCreditNoteViewModel
                {
                    Id= n.Id,
                    DocumentNumber= n.DocumentNumber
                })
                .ToList();
           return invoiceNumbers;
        }
                
        public EditCreditNoteViewModel ViewCreditNoteForEdit(int id)
        {
            if (id == 0)
            {
                return null;
            }

            var creditNote = dbContext.Documents
                .Where(i=>i.Id == id)
                .FirstOrDefault();

            var invoice = dbContext.Documents
                .Where(i=>i.Id == creditNote.CreditToInvoiceId)                
                .FirstOrDefault();  

            var creditNoteForEdit = new EditCreditNoteViewModel
            {
                Comment = creditNote.Comment,
                CreditToInvoiceNumber =invoice.DocumentNumber,    
                InvoiceNumberId = invoice.Id,
                CurrencyId = creditNote.CurrencyId,
                Date = creditNote.Date,
                //  CustomerOrders = creditNote.CustomerOrders
                DeliveryCost = creditNote.DeliveryTrasnportCost,
                Incoterms = creditNote.Incoterms,
                TruckNumber = creditNote.TruckNumber,
                CurrencyExchangeRate = creditNote.CurrencyExchangeRateUsdToBGN,
                GrossWeight = creditNote.GrossWeight,
                NetWeight = creditNote.NetWeight,
                Products = new List<EditProductForCreditNoteViewModel>(),
                InvoiceNumbers = new List<InvoiceNumbersForEditedCreditNoteViewModel>(),
                PaymentTerms = creditNote.PaymentTerms,
                HeaderUrl = imageService.HeaderUrl(invoice.HeaderId??0),
                FooterUrl = imageService.FooterUrl(invoice.FooterId??0)
            };

            var productsListFromEditInvoice = dbContext.InvoiceProductDetails
                  .Where(cd => cd.CreditNoteId == id && cd.CreditNoteQuantity > 0)
                  .ToList();

            if (productsListFromEditInvoice.Any())
            {
                foreach (var product in productsListFromEditInvoice)
                {
                    var mainProduct = productRepository.GetMainProduct(product.ProductId);

                    var productCreditNote = new EditProductForCreditNoteViewModel
                    {
                        Id= product.Id,
                        DescriptionId= mainProduct.DescriptionId,
                        SizeId= mainProduct.SizeId,
                        GradeId = mainProduct.GradeId,
                        Description = productService.GetDescriptionName(mainProduct.DescriptionId),
                        Grade = productService.GetGradeName(mainProduct.GradeId),
                        Size = productService.GetSizeName(mainProduct.SizeId),
                        Descriptions = productService.DescriptionIdAndNameList(),
                        Grades = productService.GradeIdAndNameList(),
                        Sizes = productService.SizeIdAndNameList(),
                        Unit = product.Unit,                        
                        CreditNoteAmount = product.CreditNoteProductAmount,
                        CreditNoteBgAmount = product.CreditNoteBgAmount,
                        CreditNoteBgPrice = product.CreditNoteBgPrice,
                        CreditNoteId = product.CreditNoteId,
                        CreditNotePrice = product.CreditNotePrice,
                        CreditNoteQuantity = product.CreditNoteQuantity,
                        CustomerOrderId = product.CustomerOrderId ?? 0,
                        CustomerProductDetailId = product.CustomerOrderProductDetailsId ?? 0,
                        FscClaim = product.FscClaim,
                        FscSertificate = product.FscCertificate,
                        Profit = product.Profit,
                        ProductId = product.ProductId,
                        CreditNotePallets = product.CreditNotePallets,
                        CreditNoteSheetsPerPallet = product.CreditNoteSheetsPerPallet 
                    };
                    productCreditNote.TotalSheets = product.CreditNotePallets*product.CreditNoteSheetsPerPallet;
                    creditNoteForEdit.Products.Add(productCreditNote);
                }
            }
            return creditNoteForEdit;
        }
    }
}
