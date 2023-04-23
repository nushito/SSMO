using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents;
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
        public DebitNoteService(ApplicationDbContext dbContext, IDocumentService documentService)
        {
            this.dbContext = dbContext;
            this.documentService = documentService; 
        }
    
        public CreditAndDebitNoteViewModel CreateDebitNote
            (int invoiceId, DateTime date,bool moreQuantity, List<AddProductsToCreditAndDebitNoteFormModel> products)
        {
            var invoiceForDebit = dbContext.Documents
               .Where(n => n.Id == invoiceId && n.DocumentType == Data.Enums.DocumentTypes.Invoice)
               .FirstOrDefault();

            var invoiceProductDetails = dbContext.InvoiceProductDetails
                .Where(a => a.InvoiceId == invoiceForDebit.Id)
                .FirstOrDefault();

            var debitNote = new Document
            {
                DocumentNumber = dbContext.Documents
                .OrderByDescending(n => n.DocumentNumber).Select(num => num.DocumentNumber).First() + 1,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.DebitNote,
                DebitToInvoiceDate = invoiceForDebit.Date,
                DebitToInvoiceNumber = invoiceForDebit.DocumentNumber,
                SupplierId = invoiceForDebit.SupplierId,
                SupplierOrderId = invoiceForDebit.SupplierOrderId,
                CurrencyId = invoiceForDebit.CurrencyId,
                CurrencyExchangeRateUsdToBGN = invoiceForDebit.CurrencyExchangeRateUsdToBGN,
                CustomerId = invoiceForDebit.CustomerId,
                MyCompanyId = invoiceForDebit.MyCompanyId,
                Vat = invoiceForDebit.Vat,
                DebitNoteProducts = new List<InvoiceProductDetails>(),
                CreditAndDebitNoteProducts = new List<Product>()
            };

            dbContext.Documents.Add(debitNote);
            dbContext.SaveChanges();

            if (moreQuantity == true)
            {
                foreach (var product in products)
                {
                    var mainProduct = dbContext.Products
                        .Where(a => a.DescriptionId == product.DescriptionId && a.GradeId == product.GradeId &&
                        a.SizeId == product.SizeId &&
                        a.InvoiceProductDetails.Contains(invoiceProductDetails))
                        .FirstOrDefault();

                    var productForDebit = dbContext.InvoiceProductDetails
                   .Where(co => co.ProductId == mainProduct.Id)
                   .FirstOrDefault();

                    productForDebit.DebitNoteId = debitNote.Id;
                    productForDebit.DebitNotePrice = product.Price;
                    productForDebit.DebitNoteQuantity = product.Quantity;
                    productForDebit.DebitNoteAmount = product.Quantity * product.Price;
                    productForDebit.DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.Quantity;
                    productForDebit.DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN;
                    debitNote.Amount += product.Price * product.Quantity;
                    debitNote.TotalQuantity += product.Quantity;
                }
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
                        Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), product.Unit, true),
                        DocumentId = debitNote.Id,
                        Price = product.Price,
                        OrderedQuantity = product.Quantity,                       
                        FscClaim = product.FscClaim,
                        FscSertificate = product.FscSertificate,
                        BgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN,
                        BgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.Quantity
                    };
                    newProduct.Amount = product.Price * product.Quantity;
                    dbContext.Products.Add(newProduct);

                    debitNote.Amount += product.Price * product.Quantity;
                    debitNote.TotalQuantity += product.Quantity;
                }
            }

            debitNote.VatAmount = debitNote.Amount * debitNote.Vat / 100;
            debitNote.TotalAmount = debitNote.Amount + debitNote.VatAmount ?? 0;
           
        
            invoiceForDebit.Balance += debitNote.TotalAmount;           

            dbContext.SaveChanges();

            var debitNoteForPrint = documentService.PrintCreditAndDebitNote(debitNote.Id);
            documentService.CreateBgInvoice(debitNote.Id);

            return debitNoteForPrint;
        } 
    }
}
