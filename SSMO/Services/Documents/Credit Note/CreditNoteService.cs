using AutoMapper;
using DevExpress.Data.ODataLinq.Helpers;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using SSMO.Models.Documents.Invoice;
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
            (int invoiceId, DateTime date, bool quantityBack, List<AddProductsToCreditAndDebitNoteFormModel> products)
        { 
            var invoiceForCredit  = dbContext.Documents
                .Where(n=>n.Id== invoiceId && n.DocumentType == Data.Enums.DocumentTypes.Invoice)
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
                CustomerOrderId = invoiceForCredit.CustomerOrderId,
                MyCompanyId = invoiceForCredit.MyCompanyId,
                Vat = invoiceForCredit.Vat,
                CreditNoteProducts = new List<Product>()
            };

            dbContext.Documents.Add(creditNote);
            dbContext.SaveChanges();

            if (quantityBack == true) 
            {
                foreach (var product in products)
                {
                    var existProduct = dbContext.Products
                        .Where(d => d.DescriptionId == product.DescriptionId 
                        && d.GradeId == product.GradeId 
                        && d.SizeId == product.SizeId
                        && d.DocumentId == invoiceId)
                        .FirstOrDefault();

                    if(product.Quantity > existProduct.OrderedQuantity)
                    {
                        return null;
                    }

                    existProduct.CreditNoteId = creditNote.Id;                    
                    existProduct.CreditNotePallets = product.Pallets;
                    existProduct.CreditNotePrice= product.Price;
                    existProduct.CreditNoteSheetsPerPallet= product.SheetsPerPallet;
                    existProduct.CreditNoteQuantity= product.Quantity;
                    existProduct.CreditNoteProductAmount = product.Quantity * product.Price;
                    existProduct.Pallets -= product.Pallets;
                    existProduct.TotalSheets -= product.Pallets * product.SheetsPerPallet;
                    existProduct.LoadedQuantityM3 += product.Quantity;       
                    
                    creditNote.Amount += product.Price * product.Quantity;                    
                    creditNote.CreditNoteProducts.Add(existProduct);
                }
                creditNote.VatAmount = creditNote.Amount * creditNote.Vat / 100;
                creditNote.CreditNoteTotalAmount = creditNote.Amount + creditNote.VatAmount ?? 0;                
            }
            else
            {
                foreach (var product in products)
                {
                    var newProduct = new Product
                    {
                        DescriptionId = product.DescriptionId,
                        GradeId = product.GradeId,
                        SizeId = product.SizeId,                     
                        Unit = (Data.Enums.Unit) Enum.Parse(typeof(Data.Enums.Unit),product.Unit,true),
                        CreditNoteId= creditNote.Id,
                        CreditNotePallets = product.Pallets,
                        CreditNotePrice = product.Price,    
                        CreditNoteQuantity = product.Quantity,
                        CreditNoteSheetsPerPallet= product.SheetsPerPallet,
                        CustomerOrderId = invoiceForCredit.CustomerOrderId,
                        FSCClaim = product.FscClaim,
                        FSCSertificate = product.FscSertificate
                    };
                    newProduct.CreditNoteProductAmount = product.Price*product.Quantity;
                    creditNote.Amount += product.Price * product.Quantity;
                    dbContext.Products.Add(newProduct);
                }

                creditNote.VatAmount = creditNote.Amount * creditNote.Vat / 100;
                creditNote.CreditNoteTotalAmount = creditNote.Amount + creditNote.VatAmount ?? 0;
            }
            dbContext.SaveChanges();

            var customerOrderBalance = dbContext.CustomerOrders
                .Where(o=>o.Id == invoiceForCredit.CustomerOrderId)
                .Select(b=>b.Balance)
                .FirstOrDefault();

            var invoiceForCreditBalance = invoiceForCredit.Balance;
            invoiceForCredit.CreditNoteTotalAmount = creditNote.CreditNoteTotalAmount;

            if(customerOrderBalance > 0 || invoiceForCreditBalance > 0)
            {
                customerOrderBalance -= creditNote.CreditNoteTotalAmount;
                invoiceForCreditBalance-= creditNote.CreditNoteTotalAmount;
            }

            dbContext.SaveChanges();

            var creditNoteForPrint = documentService.PrintCreditAndDebitNote(creditNote.Id);
            documentService.CreateBgInvoice(creditNote.Id);

           return creditNoteForPrint;
        }


    }
}
