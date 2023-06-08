using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Migrations;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.DebitNote;
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
    
        public CreditAndDebitNoteViewModel CreateDebitNote
            (int invoiceId, DateTime date,bool moreQuantity,string deliveryAddress, 
            List<AddProductsToCreditAndDebitNoteFormModel> products, 
            List<PurchaseProductsForDebitNoteViewModel> availableProducts)
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
                DebitToInvoiceNumber = invoiceForDebit.Id,
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
                InvoiceProducts = new List<InvoiceProductDetails>()
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
                            productForDebit.TotalSheets = product.Pallets * product.SheetsPerPallet;
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
                                CustomerOrderId = invoiceForDebit.CustomerOrders.Select(i => i.Id).First()
                            };

                            debitProduct.TotalSheets = product.Pallets * product.SheetsPerPallet;
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
                            DebitNoteId = debitNote.Id,
                            CustomerOrderId = invoiceForDebit.CustomerOrders.Select(i=>i.Id).First()
                        };

                        debitProduct.TotalSheets = product.Pallets * product.SheetsPerPallet;
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
                        ProductId = mainProduct.Id,
                        CustomerOrderProductDetailsId = product.CustomerOrderDetailsId
                    };

                    if (product.CustomerOrderDetail != null)
                    {
                        debitNoteDetails.CustomerOrderId = product.CustomerOrderDetail.Id;
                    }

                   // productService.ReviseAutstandingQuantity(product.CustomerOrderDetailsId, product.DebitNoteQuantity);

                    mainProduct.SoldQuantity += product.DebitNoteQuantity;
                    mainProduct.QuantityAvailableForCustomerOrder-= product.DebitNoteQuantity;

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
            (int id, DateTime date, string incoterms,string comment, List<EditProductForDebitNoteViewModel> products)
        {
            var debitNoteForEdit = dbContext.Documents.Find(id);

            debitNoteForEdit.Date = date;
            debitNoteForEdit.Incoterms= incoterms;
          
        if(products != null)
            {
                foreach (var product in products)
                {
                    if(product.DebitNoteQuantity == 0) { continue; }

                    var invoicedProduct = dbContext.InvoiceProductDetails
                        .Where(i=>i.Id == product.Id)
                        .FirstOrDefault();

                    invoicedProduct.DebitNoteQuantity = product.DebitNoteQuantity;
                    invoicedProduct.DebitNotePrice= product.DebitNotePrice;
                    invoicedProduct.DebitNoteAmount= product.DebitNoteQuantity * product.DebitNotePrice;
                    invoicedProduct.DebitNoteBgPrice = product.DebitNoteBgPrice * debitNoteForEdit.CurrencyExchangeRateUsdToBGN;
                    invoicedProduct.DebitNoteBgAmount = invoicedProduct.DebitNoteBgPrice * product.DebitNoteQuantity;
                    invoicedProduct.DeditNotePallets = product.DebitNotePallets;
                    invoicedProduct.DeditNoteSheetsPerPallet = product.DebitNoteSheetsPerPallet;
                    invoicedProduct.TotalSheets = product.DebitNotePallets * product.DebitNoteSheetsPerPallet;
                    debitNoteForEdit.Amount += invoicedProduct.DebitNoteAmount;
                }
            }

            debitNoteForEdit.VatAmount = debitNoteForEdit.Amount * debitNoteForEdit.Vat / 100;
            debitNoteForEdit.DebitNoteTotalAmount = debitNoteForEdit.Amount + debitNoteForEdit.VatAmount ?? 0;

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
                .Where(d=>d.Id == debitNote.DebitToInvoiceNumber)
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
                DebitNoteInvoicenumbers = GetInvoiceNumbers()
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
                        DebitNotePallets = product.DeditNotePallets,
                        DebitNoteSheetsPerPallet = product.DeditNoteSheetsPerPallet
                       
                    };
                    productCreditNote.TotalSheets = product.CreditNotePallets * product.CreditNoteSheetsPerPallet;
                    viewDebitNoteForEdit.Products.Add(productCreditNote);
                }
            }

            return viewDebitNoteForEdit;
        }
    }
}
