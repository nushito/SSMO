using AutoMapper;
using DevExpress.Data.ODataLinq.Helpers;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Enums;
using SSMO.Data.Migrations;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.CreditNote;
using SSMO.Services.Documents.Invoice;
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
        public CreditNoteService(ApplicationDbContext dbContex, IDocumentService documentService, IProductService productService)
        {
            this.dbContext = dbContex;
            this.documentService = documentService;
            this.productService = productService;
        }

       
        public CreditAndDebitNoteViewModel CreateCreditNote
            (int invoiceId, DateTime date, bool quantityBack, 
            string deliveryAddress, List<AddProductsToCreditAndDebitNoteFormModel> products)
        {
            var invoiceForCredit = dbContext.Documents
                .Where(n => n.Id == invoiceId && n.DocumentType == Data.Enums.DocumentTypes.Invoice)
                .FirstOrDefault();            

            var creditNote = new Document
            {
                DocumentNumber = dbContext.Documents
                .OrderByDescending(n => n.DocumentNumber).Select(num => num.DocumentNumber).First() + 1,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.CreditNote,
                CreditToInvoiceDate = invoiceForCredit.Date,
                CreditToInvoiceNumber = invoiceForCredit.DocumentNumber,
                SupplierId = invoiceForCredit.SupplierId,
                SupplierOrderId = invoiceForCredit.SupplierOrderId,
                CurrencyId = invoiceForCredit.CurrencyId,
                CurrencyExchangeRateUsdToBGN = invoiceForCredit.CurrencyExchangeRateUsdToBGN,
                CustomerId = invoiceForCredit.CustomerId,
                MyCompanyId = invoiceForCredit.MyCompanyId,
                Vat = invoiceForCredit.Vat,
                InvoiceProducts = new List<InvoiceProductDetails>(),
                CreditNoteDeliveryAddress = deliveryAddress,               
                CreditAndDebitNoteProducts = new List<Product>()
            };

            var customerOrdersByInvoice = dbContext.CustomerOrders
                .Where(a => a.Documents
                .Select(a => a.Id).Contains(invoiceForCredit.Id)).ToList();

            creditNote.CustomerOrders= customerOrdersByInvoice;

            dbContext.Documents.Add(creditNote);
            dbContext.SaveChanges();
            
            foreach (var product in products)
            {
                var existProduct = dbContext.Products
                    .Where(d => d.DescriptionId == product.DescriptionId
                    && d.GradeId == product.GradeId
                    && d.SizeId == product.SizeId 
                    && d.LoadedQuantityM3 >0
                    && d.SoldQuantity > 0
                    && d.InvoiceProductDetails.Select(a => a.InvoiceId).ToList().Contains(invoiceForCredit.Id))
                    .FirstOrDefault();

                if(existProduct != null)
                {
                    var invoicedProductForBack = dbContext.InvoiceProductDetails
                    .Where(i => i.ProductId == existProduct.Id)
                    .FirstOrDefault();

                    if (product.Quantity > invoicedProductForBack.InvoicedQuantity)
                    {
                        return null;
                    }
                    invoicedProductForBack.CreditNoteId = creditNote.Id;
                    invoicedProductForBack.CreditNotePallets = product.Pallets;
                    invoicedProductForBack.CreditNotePrice = product.Price;
                    invoicedProductForBack.CreditNoteSheetsPerPallet = product.SheetsPerPallet;
                    invoicedProductForBack.CreditNoteQuantity = product.Quantity;
                    invoicedProductForBack.CreditNoteProductAmount = product.Quantity * product.Price;
                    invoicedProductForBack.CreditNoteBgPrice = product.Price * creditNote.CurrencyExchangeRateUsdToBGN;
                    invoicedProductForBack.CreditNoteBgAmount = product.Price * creditNote.CurrencyExchangeRateUsdToBGN * product.Quantity;
                    invoicedProductForBack.TotalSheets = product.Pallets * product.SheetsPerPallet;
                    creditNote.Amount += product.Price * product.Quantity;
                    creditNote.TotalQuantity += product.Quantity;

                    if (quantityBack == true)
                    {  
                        existProduct.QuantityAvailableForCustomerOrder += product.Quantity;
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
                        InvoiceId = creditNote.Id
                    };

                    invoiceProduct.TotalSheets = product.Pallets * product.SheetsPerPallet;
                    newProduct.InvoiceProductDetails.Add(invoiceProduct);

                    creditNote.Amount += product.Price * product.Quantity;
                    creditNote.CreditAndDebitNoteProducts.Add(newProduct);
                    creditNote.TotalQuantity += product.Quantity;
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
            documentService.CreateBgInvoice(creditNote.Id);

            return creditNoteForPrint;
        }

        public bool EditCreditNote
            (int id, DateTime date, string incoterms, string truckNumber, decimal netWeight, 
            decimal grossWeight, decimal deliveryCost, decimal currencyExchangeRate, string comment, List<EditProductForCreditNoteViewModel> products)
        {
            if(id == 0) { return false; }

            var creditNote = dbContext.Documents
                .Find(id);

            creditNote.Date = date;
            creditNote.Incoterms= incoterms;
            creditNote.TruckNumber= truckNumber;
            creditNote.NetWeight= netWeight;
            creditNote.GrossWeight= grossWeight;
            creditNote.DeliveryTrasnportCost= deliveryCost;
            creditNote.CurrencyExchangeRateUsdToBGN = currencyExchangeRate;
            creditNote.Comment= comment;

            foreach (var product in products)
            {
                var productForEdit = dbContext.InvoiceProductDetails
                    .Where(a => a.Id == product.Id)
                    .FirstOrDefault();

                productForEdit.CreditNotePallets = product.CreditNotePallets;
                productForEdit.CreditNoteSheetsPerPallet = product.CreditNoteSheetsPerPallet;
                productForEdit.CreditNoteQuantity = product.CreditNoteQuantity;
                productForEdit.Unit = product.Unit;
                productForEdit.FscCertificate = product.FscSertificate;
                productForEdit.FscClaim = product.FscClaim;
                productForEdit.TotalSheets = product.CreditNotePallets * product.CreditNoteSheetsPerPallet;
                productForEdit.CreditNotePrice = product.CreditNotePrice;
                productForEdit.CreditNoteBgPrice = product.CreditNotePrice * currencyExchangeRate;
                productForEdit.CreditNoteProductAmount = product.CreditNotePrice * product.CreditNoteQuantity;
                productForEdit.CreditNoteBgAmount = productForEdit.CreditNoteProductAmount * currencyExchangeRate;

                creditNote.Amount += product.CreditNotePrice * product.CreditNoteQuantity;
                creditNote.TotalQuantity += product.CreditNoteQuantity;               
            }

            dbContext.SaveChanges();
            return true;
        }
        public bool AddNewProductsToCreditNoteWhenEdit
            (int id, int invoiceId, 
            List<ProductForCreditNoteViewModelPerInvoice> productsFromInvoice, 
            List<NewProductsForCreditNoteViewModel> newPoducts)
        {
            if (id == 0) { return false; }

            var creditNote = dbContext.Documents
                .Find(id);

            if(productsFromInvoice.Count != 0)
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
                        productFromInvoice.CreditNotePrice = product.SellPrice;
                        productFromInvoice.FscCertificate= product.FscCertificate;
                        productFromInvoice.FscClaim = product.FscClaim;
                        productFromInvoice.TotalSheets = product.Pallets * product.SheetsPerPallet;
                        productFromInvoice.CreditNoteProductAmount = productFromInvoice.CreditNotePrice * productFromInvoice.CreditNoteQuantity;
                        productFromInvoice.CreditNoteBgPrice = productFromInvoice.CreditNotePrice * creditNote.CurrencyExchangeRateUsdToBGN;
                        productFromInvoice.CreditNoteBgAmount = productFromInvoice.CreditNoteProductAmount* creditNote.CurrencyExchangeRateUsdToBGN;
                        creditNote.Amount += productFromInvoice.CreditNoteProductAmount;
                        creditNote.TotalQuantity += productFromInvoice.CreditNoteQuantity;
                    }                    
                }
            };

            if(newPoducts.Count > 0)
            {
                foreach (var product in newPoducts)
                {

                }
            }

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

            var invoiceId = dbContext.Documents
                .Where(i=>i.DocumentNumber == creditNote.CreditToInvoiceNumber)
                .Select(i=>i.Id)
                .FirstOrDefault();  

            var creditNoteForEdit = new EditCreditNoteViewModel
            {
                Comment = creditNote.Comment,
                CreditToInvoiceNumber = creditNote.CreditToInvoiceNumber,    
                InvoiceNumberId = invoiceId,
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
                InvoiceNumbers = new List<InvoiceNumbersForEditedCreditNoteViewModel>()
            };

            var productsListFromEditInvoice = dbContext.InvoiceProductDetails
                  .Where(cd => cd.CreditNoteId == id ).ToList();

            //var productsList = dbContext.Products
            //    .Where(d=>d.DocumentId == id)
            //    .ToList();

            if (productsListFromEditInvoice.Any())
            {
                foreach (var product in productsListFromEditInvoice)
                {
                    var mainProduct = dbContext.Products
                 .Where(a => a.Id == product.ProductId)
                 .FirstOrDefault();

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
                        CustomerOrderId = product.CustomerOrderId,
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

            //if(productsList.Any())
            //{
            //    foreach (var product in productsList)
            //    {
            //        var productCreditNote = new EditProductForCreditNoteViewModel
            //        {
            //            DescriptionId= product.Id,
            //            SizeId= product.SizeId,
            //            GradeId= product.GradeId,
            //            Description = productService.GetDescriptionName(product.DescriptionId),
            //            Grade = productService.GetGradeName(product.GradeId),
            //            Size = productService.GetSizeName(product.SizeId),
            //            Descriptions = productService.DescriptionIdAndNameList(),
            //            Grades = productService.GradeIdAndNameList(),
            //            Sizes = productService.SizeIdAndNameList(),
            //            Unit = product.Unit,
            //            CreditNoteAmount = product.Amount,
            //            CreditNoteBgAmount = product.BgAmount,
            //            CreditNoteBgPrice = product.BgPrice,
            //            CreditNoteId = product.DocumentId,
            //            CreditNotePrice = product.Price,
            //            CreditNoteQuantity = product.OrderedQuantity,
            //            CustomerOrderId = product.CustomerOrderId ?? 0,                        
            //            FscClaim = product.FscClaim,
            //            FscSertificate = product.FscSertificate,                      
            //            ProductId = product.Id,
            //            CreditNotePallets = product.Pallets,
            //            CreditNoteSheetsPerPallet = product.SheetsPerPallet,                        
            //        };
            //        productCreditNote.TotalSheets = product.Pallets * product.SheetsPerPallet;
            //        creditNoteForEdit.Products.Add(productCreditNote);
            //    }
            //}
            return creditNoteForEdit;
        }
    }
}
