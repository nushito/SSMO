using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Migrations;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Reports.DebitNote;
using SSMO.Repository;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SSMO.Services.Documents.DebitNote
{
    public class DebitNoteService : IDebitNoteService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IDocumentService documentService;  
        private readonly IProductRepository productRepository;
        private readonly IProductService productService;
        public DebitNoteService(ApplicationDbContext dbContext, IDocumentService documentService, 
            IProductRepository productRepository, IProductService productService)
        {
            this.dbContext = dbContext;
            this.documentService = documentService; 
            this.productRepository= productRepository;
            this.productService = productService;   
        }

        public bool AddNewProductsToDebitNoteWhenEdit(int id, int invoiceId,
            List<NewProductsFromOrderEditedDebitNoteViewModel> products,
             List<NewProductsForEditedDebitNoteFormModel> newProducts,
             IList<PurchaseProductsForDebitNoteViewModel> availableProducts)
        {
            var debitNote = dbContext.Documents.Find(id);

            if (debitNote == null) { return false; }

            if(debitNote.DebitToInvoiceId != invoiceId)
            {
                debitNote.DebitToInvoiceId = invoiceId;
            }

            var invoice = dbContext.Documents
                .Include(b=>b.BankDetails)
               .Where(i => i.Id == invoiceId)
               .FirstOrDefault();

            var customerOrders = dbContext.CustomerOrders
               .Where(a => a.Documents.Select(i => i.Id).Contains(invoiceId)).ToList();

            if (debitNote.CustomerId != invoice.CustomerId)
            {
                debitNote.CustomerId = invoice.CustomerId;
            }

            if (products.Any())
            {
                foreach (var product in products)
                {
                    if (product.InvoicedQuantity == 0) { continue; };

                    var invoiceProduct = dbContext.InvoiceProductDetails.Find(product.Id);

                    invoiceProduct.DebitNoteQuantity = product.InvoicedQuantity;
                    invoiceProduct.Unit = product.Unit;
                    invoiceProduct.DebitNotePallets = product.Pallets;
                    invoiceProduct.DebitNoteSheetsPerPallet = product.SheetsPerPallet;
                    invoiceProduct.DebitNotePrice = product.SellPrice;
                    invoiceProduct.DebitNoteId = id;
                    invoiceProduct.DebitNoteAmount = product.SellPrice * product.InvoicedQuantity;
                    debitNote.Amount += invoiceProduct.DebitNoteAmount;
                    invoiceProduct.DebitNoteBgPrice = product.SellPrice * debitNote.CurrencyExchangeRateUsdToBGN;
                    invoiceProduct.DebitNoteBgAmount = product.InvoicedQuantity * invoiceProduct.DebitNoteBgPrice;
                }
            }

            if (newProducts.Any())
            {
                foreach (var product in newProducts)
                {
                    var newProduct = new Product
                    {
                        DescriptionId = product.DescriptionId,
                        GradeId = product.GradeId,
                        SizeId = product.SizeId,
                        DocumentId = debitNote.Id,
                        InvoiceProductDetails = new List<InvoiceProductDetails>(),
                        SupplierOrderId = null
                    };

                    var invoiceProduct = new InvoiceProductDetails
                    {
                        Unit = product.Unit,                      
                        DebitNotePrice = product.DebitNotePrice,
                        DebitNoteQuantity = product.DebitNoteQuantity,     
                        DebitNotePallets = product.DebitNotePallets,
                        DebitNoteSheetsPerPallet = product.DebitNoteSheetsPerPallet,
                        FscClaim = product.FscClaim,
                        FscCertificate = product.FscCertificate,
                        DebitNoteAmount = product.DebitNotePrice * product.DebitNoteQuantity,
                        DebitNoteBgAmount = product.DebitNotePrice * debitNote.CurrencyExchangeRateUsdToBGN * product.DebitNoteQuantity,
                        DebitNoteBgPrice = product.DebitNotePrice * debitNote.CurrencyExchangeRateUsdToBGN,
                        DebitNoteId = debitNote.Id,
                       // CustomerOrderId = customerOrders.Select(a => a.Id).First(),
                        InvoiceId = debitNote.Id,
                        DebitNoteTotalSheets = product.DebitNotePallets* product.DebitNoteSheetsPerPallet
                    };

                    invoiceProduct.TotalSheets = product.DebitNotePallets * product.DebitNoteSheetsPerPallet;
                    debitNote.Amount += invoiceProduct.CreditNoteProductAmount;
                    if (newProducts.All(i => i.Unit == Unit.m3) || newProducts.All(i => i.Unit == Unit.m2)
                        || newProducts.All(i => i.Unit == Unit.pcs) || newProducts.All(i => i.Unit == Unit.sheets)
                        || newProducts.All(i => i.Unit == Unit.m))
                    {
                        debitNote.TotalQuantity += invoiceProduct.CreditNoteQuantity;
                    }

                    dbContext.Products.Add(newProduct);
                    dbContext.SaveChanges();
                    newProduct.InvoiceProductDetails.Add(invoiceProduct);
                }
            }

            if (availableProducts.Count > 0)
            {
                foreach (var product in availableProducts)
                {
                    if (!product.Checked) { continue; }

                    var mainProduct = productRepository.GetMainProduct(product.ProductId);
                    if (mainProduct == null) { continue; }

                    if(product.DebitNoteQuantity <= 0)
                    {
                        continue;
                    }

                    var debitNoteDetails = new InvoiceProductDetails
                    {
                        Unit = product.Unit,
                        DebitNotePrice = product.Price,
                        DebitNoteQuantity = product.DebitNoteQuantity,
                        FscClaim = product.FscClaim,
                        FscCertificate = product.FscSertificate,
                        DebitNoteAmount = product.Price * product.DebitNoteQuantity,
                        DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN,
                        DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.DebitNoteQuantity,
                        DebitNoteId = debitNote.Id,
                        InvoiceId = debitNote.Id,
                        ProductId = mainProduct.Id,
                        DebitNoteTotalSheets = product.Pallets * product.SheetsPerPallet
                    };

                    if (product.ChoosenCustomerOrderProductDetailsId != null)
                    {
                        debitNoteDetails.CustomerOrderId = dbContext.CustomerOrderProductDetails
                            .Where(x => x.Id == product.ChoosenCustomerOrderProductDetailsId)
                            .Select(a => a.CustomerOrderId)
                            .FirstOrDefault();
                        debitNoteDetails.CustomerOrderProductDetailsId = product.ChoosenCustomerOrderProductDetailsId;
                        var customerProductDetail = dbContext.CustomerOrderProductDetails
                            .Where(x => x.Id == product.ChoosenCustomerOrderProductDetailsId)
                            .FirstOrDefault();
                        customerProductDetail.AutstandingQuantity -= product.DebitNoteQuantity;
                        productService.ReviseAutstandingQuantity(product.ChoosenCustomerOrderProductDetailsId ?? 0, product.DebitNoteQuantity);
                    }
                    else
                    {
                        mainProduct.QuantityAvailableForCustomerOrder -= product.DebitNoteQuantity;
                    };

                    mainProduct.SoldQuantity += product.DebitNoteQuantity;

                    debitNote.Amount += product.Price * product.DebitNoteQuantity;
                    debitNote.DebitNoteProducts.Add(debitNoteDetails);
                    mainProduct.InvoiceProductDetails.Add(debitNoteDetails);
                }
            }

            debitNote.VatAmount = debitNote.Amount * debitNote.Vat/100;
            debitNote.DebitNoteTotalAmount = debitNote.Amount + debitNote.VatAmount ?? 0;
            documentService.EditBgInvoice(debitNote.DocumentNumber);
            dbContext.SaveChanges();
            return true;
        }

        public CreditAndDebitNoteViewModel CreateDebitNote
            (int invoiceId, DateTime date,bool moreQuantity,string deliveryAddress, 
            List<AddProductsToCreditAndDebitNoteFormModel> products, 
            List<PurchaseProductsForDebitNoteViewModel> availableProducts, string paymentTerms)
        {
            var invoiceForDebit = dbContext.Documents
               .Where(n => n.Id == invoiceId && n.DocumentType == Data.Enums.DocumentTypes.Invoice)
               .FirstOrDefault();
           
            var debitNote = new Document
            {
                DocumentNumber = dbContext.Documents
                .OrderByDescending(n => n.DocumentNumber).Select(num => num.DocumentNumber).First() + 1,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.DebitNote,
                DebitToInvoiceDate = invoiceForDebit.Date,
                DebitToInvoiceId = invoiceForDebit.Id,
                DeliveryAddress= deliveryAddress,
                SupplierId = invoiceForDebit.SupplierId,
                SupplierOrderId = invoiceForDebit.SupplierOrderId,
                CurrencyId = invoiceForDebit.CurrencyId,
                CurrencyExchangeRateUsdToBGN = invoiceForDebit.CurrencyExchangeRateUsdToBGN,
                CustomerId = invoiceForDebit.CustomerId,
                MyCompanyId = invoiceForDebit.MyCompanyId,
                Vat = invoiceForDebit.Vat,
                DebitNoteProducts = new List<InvoiceProductDetails>(),
                CreditAndDebitNoteProducts = new List<Product>(),
                InvoiceProducts = new List<InvoiceProductDetails>(),
                PaymentTerms = paymentTerms
            };

            dbContext.Documents.Add(debitNote);
            dbContext.SaveChanges();

            if(products.Count > 0)
            {
                foreach (var product in products)
                {
                    var mainProduct = dbContext.Products
                        .Where(a => a.DescriptionId == product.DescriptionId
                        && a.GradeId == product.GradeId
                        && a.SizeId == product.SizeId).FirstOrDefault(); 

                    if (mainProduct != null)  
                    {
                        if (mainProduct.InvoiceProductDetails.Select(i => i.InvoiceId).Contains(invoiceForDebit.Id))
                        {
                            var productForDebit = dbContext.InvoiceProductDetails
                             .Where(co => co.ProductId == mainProduct.Id)
                             .FirstOrDefault();

                            productForDebit.DebitNoteId = debitNote.Id;
                            productForDebit.DebitNotePrice = product.Price;

                            if (moreQuantity == true)
                            { productForDebit.DebitNoteQuantity = product.Quantity; }
                            else
                            { productForDebit.DebitNoteQuantity = 0; }
                          
                            productForDebit.DebitNoteAmount = product.Quantity * product.Price;
                            productForDebit.DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.Quantity;
                            productForDebit.DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN;
                            productForDebit.DebitNoteTotalSheets = product.Pallets * product.SheetsPerPallet;
                        }
                        else
                        {
                            var debitProduct = new InvoiceProductDetails
                            {
                                Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), product.Unit, true),
                                DebitNotePrice = product.Price,
                                DebitNoteQuantity = product.Quantity,
                                FscClaim = product.FscClaim,
                                FscCertificate = product.FscSertificate,
                                DebitNoteAmount = product.Price * product.Quantity,
                                DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN,
                                DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.Quantity,
                                DebitNoteId = debitNote.Id,
                               // CustomerOrderId = invoiceForDebit.CustomerOrders.Select(i => i.Id).First()
                            };

                            debitProduct.DebitNoteTotalSheets = product.Pallets * product.SheetsPerPallet;
                            debitNote.InvoiceProducts.Add(debitProduct);   
                        }                        
                    }
                    else
                    {
                       var  newProduct = new Product
                            {
                                DescriptionId = product.DescriptionId,
                                GradeId = product.GradeId,
                                SizeId = product.SizeId,
                                InvoiceProductDetails = new List<InvoiceProductDetails>(),
                                DocumentId = debitNote.Id
                            };

                        var debitProduct = new InvoiceProductDetails
                        {
                            Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), product.Unit, true),
                            DebitNotePrice = product.Price,
                            DebitNoteQuantity = product.Quantity,
                            FscClaim = product.FscClaim,
                            FscCertificate = product.FscSertificate,
                            DebitNoteAmount = product.Price * product.Quantity,
                            DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN,
                            DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.Quantity,
                            DebitNoteId = debitNote.Id
                        };

                        debitProduct.DebitNoteTotalSheets = product.Pallets * product.SheetsPerPallet;
                        debitNote.DebitNoteProducts.Add(debitProduct);
                        debitNote.CreditAndDebitNoteProducts.Add(newProduct);
                    }
                    debitNote.Amount += product.Price * product.Quantity;
                }
            }

            if(availableProducts.Count > 0)
            {
                foreach (var product in availableProducts)
                {
                    var mainProduct = productRepository.GetMainProduct(product.ProductId); 
                    if (mainProduct == null) { continue; }

                    var debitNoteDetails = new InvoiceProductDetails
                    {
                        Unit =  product.Unit,
                        DebitNotePrice = product.Price,
                        DebitNoteQuantity = product.DebitNoteQuantity,
                        FscClaim = product.FscClaim,
                        FscCertificate = product.FscSertificate,
                        DebitNoteAmount = product.Price * product.DebitNoteQuantity,
                        DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN,
                        DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.DebitNoteQuantity,
                        DebitNoteId = debitNote.Id,
                        InvoiceId = debitNote.Id,
                        ProductId = mainProduct.Id                        
                    };

                    if(product.ChoosenCustomerOrderProductDetailsId != null)
                    {
                        debitNoteDetails.CustomerOrderId = dbContext.CustomerOrderProductDetails
                            .Where(x => x.Id == product.ChoosenCustomerOrderProductDetailsId)
                            .Select(a=>a.CustomerOrderId)
                            .FirstOrDefault();
                        debitNoteDetails.CustomerOrderProductDetailsId = product.ChoosenCustomerOrderProductDetailsId;
                        var customerProductDetail = dbContext.CustomerOrderProductDetails
                            .Where(x => x.Id == product.ChoosenCustomerOrderProductDetailsId)
                            .FirstOrDefault();
                        customerProductDetail.AutstandingQuantity -= product.DebitNoteQuantity;
                        productService.ReviseAutstandingQuantity(product.ChoosenCustomerOrderProductDetailsId ??0, product.DebitNoteQuantity);
                    }
                    else
                    {
                        mainProduct.QuantityAvailableForCustomerOrder -= product.DebitNoteQuantity;
                    };

                    mainProduct.SoldQuantity += product.DebitNoteQuantity;                    

                    debitNote.Amount += product.Price * product.DebitNoteQuantity;
                    debitNote.DebitNoteProducts.Add(debitNoteDetails);             
                    mainProduct.InvoiceProductDetails.Add(debitNoteDetails);
                }
            }
            
            debitNote.VatAmount = debitNote.Amount * debitNote.Vat / 100;
            debitNote.DebitNoteTotalAmount = debitNote.Amount + debitNote.VatAmount ?? 0;           
        
            invoiceForDebit.Balance += debitNote.DebitNoteTotalAmount;           

            dbContext.SaveChanges();

            var debitNoteForPrint = documentService.PrintCreditAndDebitNote(debitNote.Id);
            documentService.CreateBgInvoice(debitNote.Id);

            return debitNoteForPrint;
        }

        public bool EditDebitNote
            (int id, DateTime date, string incoterms,string comment, 
            List<EditProductForDebitNoteViewModel> products, string paymentTerms)
        {
            var debitNoteForEdit = dbContext.Documents.Find(id);

            debitNoteForEdit.Date = date;
            debitNoteForEdit.Incoterms= incoterms;
            debitNoteForEdit.Amount = 0; //TODO dali e dobre da zanulim?
            debitNoteForEdit.PaymentTerms= paymentTerms;

        if(products != null)
            {
                foreach (var product in products)
                {
                    var invoicedProduct = dbContext.InvoiceProductDetails
                        .Where(i=>i.Id == product.Id)
                        .FirstOrDefault();

                    var mainProduct = productRepository.GetMainProduct(invoicedProduct.ProductId);

                    mainProduct.SoldQuantity += product.DebitNoteQuantity - invoicedProduct.DebitNoteQuantity;

                    if (product.ServiceOrProductQuantity == true)
                    {
                        mainProduct.QuantityLeftForPurchaseLoading += invoicedProduct.DebitNoteQuantity - product.DebitNoteQuantity;                        
                    }

                    invoicedProduct.DebitNoteQuantity = product.DebitNoteQuantity;
                    invoicedProduct.Unit = product.Unit;
                    invoicedProduct.DebitNotePrice= product.DebitNotePrice;
                    invoicedProduct.DebitNoteAmount= product.DebitNoteQuantity * product.DebitNotePrice;
                    invoicedProduct.DebitNoteBgPrice = product.DebitNoteBgPrice * debitNoteForEdit.CurrencyExchangeRateUsdToBGN;
                    invoicedProduct.DebitNoteBgAmount = invoicedProduct.DebitNoteBgPrice * product.DebitNoteQuantity;
                    invoicedProduct.DebitNotePallets = product.DebitNotePallets;
                    invoicedProduct.DebitNoteSheetsPerPallet = product.DebitNoteSheetsPerPallet;
                    invoicedProduct.DebitNoteTotalSheets = product.DebitNotePallets * product.DebitNoteSheetsPerPallet;
                    debitNoteForEdit.Amount += invoicedProduct.DebitNoteAmount;
                }
            }

            debitNoteForEdit.VatAmount = debitNoteForEdit.Amount * debitNoteForEdit.Vat / 100;
            debitNoteForEdit.DebitNoteTotalAmount = debitNoteForEdit.Amount + debitNoteForEdit.VatAmount ?? 0;
            documentService.EditBgInvoice(debitNoteForEdit.DocumentNumber);

            dbContext.SaveChanges();
            return true;
        }

        public ICollection<InvoiceNumbersForEditedDebitNoteViewModel> GetInvoiceNumbers()
        {
            var invoiceNumbers = dbContext.Documents
                 .Where(x => x.DocumentType == DocumentTypes.Invoice)
                 .Select(n => new InvoiceNumbersForEditedDebitNoteViewModel
                 {
                     Id = n.Id,
                     DocumentNumber = n.DocumentNumber
                 })
                 .ToList();
            return invoiceNumbers;
        }

        public EditDebitNoteViewModel ViewDebitNoteForEdit(int id)
        {
            if (id == 0)
            {
                return new EditDebitNoteViewModel();
            }

            var debitNote = dbContext.Documents.Find(id);

            var products = dbContext.InvoiceProductDetails
                .Where(d=>d.DebitNoteId == debitNote.Id)
                .ToList();

            var invoice = dbContext.Documents
                .Where(d=>d.Id == debitNote.DebitToInvoiceId)
                .FirstOrDefault();

            var viewDebitNoteForEdit = new EditDebitNoteViewModel()
            {
                Comment = debitNote.Comment,                     
                CurrencyId = debitNote.CurrencyId,
                Date = debitNote.Date,
                Vat = debitNote.Vat,
                //  CustomerOrders = creditNote.CustomerOrders
             //   DeliveryCost = debitNote.DeliveryTrasnportCost,
                Incoterms =debitNote.Incoterms,
              //  TruckNumber = debitNote.TruckNumber,
                CurrencyExchangeRate = debitNote.CurrencyExchangeRateUsdToBGN,
              //  GrossWeight = debitNote.GrossWeight,
              //  NetWeight = debitNote.NetWeight,
                Products = new List<EditProductForDebitNoteViewModel>(),
                DebitToInvoiceNumberId = invoice.Id,
                InvoiceNumber = invoice.DocumentNumber,
                DebitNoteInvoicenumbers = GetInvoiceNumbers(),
                PaymentTerms = debitNote.PaymentTerms
            };

            if (products.Any())
            {
                foreach (var product in products)
                {
                    var mainProduct = productRepository.GetMainProduct(product.ProductId);

                    var productCreditNote = new EditProductForDebitNoteViewModel
                    {
                        Id = product.Id,
                        DescriptionId = mainProduct.DescriptionId,
                        SizeId = mainProduct.SizeId,
                        GradeId = mainProduct.GradeId,
                        Description = productService.GetDescriptionName(mainProduct.DescriptionId),
                        Grade = productService.GetGradeName(mainProduct.GradeId),
                        Size = productService.GetSizeName(mainProduct.SizeId),
                        Descriptions = productService.DescriptionIdAndNameList(),
                        Grades = productService.GradeIdAndNameList(),
                        Sizes = productService.SizeIdAndNameList(),
                        Unit = product.Unit,
                        DebitNoteAmount = product.DebitNoteAmount,
                        DebitNoteBgAmount = product.DebitNoteBgAmount,
                        DebitNoteBgPrice = product.DebitNoteBgPrice,
                        DebitNoteId = product.DebitNoteId,
                        DebitNotePrice = product.DebitNotePrice,
                        DebitNoteQuantity = product.DebitNoteQuantity,
                      //  CustomerOrderId = product.CustomerOrderId,
                        CustomerProductDetailId = product.CustomerOrderProductDetailsId ?? 0,
                        FscClaim = product.FscClaim,
                        FscSertificate = product.FscCertificate,
                        Profit = product.Profit,
                        ProductId = product.ProductId,
                        DebitNotePallets = product.DebitNotePallets,
                        DebitNoteSheetsPerPallet = product.DebitNoteSheetsPerPallet
                       
                    };
                    productCreditNote.TotalSheets = product.CreditNotePallets * product.CreditNoteSheetsPerPallet;
                    viewDebitNoteForEdit.Products.Add(productCreditNote);
                }
            }

            return viewDebitNoteForEdit;
        }
    }
}
