using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using System;
using System.Collections.Generic;
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
            (int invoiceId, DateTime date, bool moreQuantity, List<AddProductsToCreditAndDebitNoteFormModel> products)
        {
            var invoiceForCredit = dbContext.Documents
               .Where(n => n.Id == invoiceId && n.DocumentType == Data.Enums.DocumentTypes.Invoice)
               .FirstOrDefault();

            var debitNote = new Document
            {
                DocumentNumber = dbContext.Documents
                .OrderByDescending(n => n.DocumentNumber).Select(num => num.DocumentNumber).First() + 1,
                Date = date,
                DocumentType = Data.Enums.DocumentTypes.DebitNote,
                DebitToInvoiceDate = invoiceForCredit.Date,
                DebitToInvoiceNumber = invoiceForCredit.DocumentNumber,
                SupplierId = invoiceForCredit.SupplierId,
                SupplierOrderId = invoiceForCredit.SupplierOrderId,
                CurrencyId = invoiceForCredit.CurrencyId,
                CurrencyExchangeRateUsdToBGN = invoiceForCredit.CurrencyExchangeRateUsdToBGN,
                CustomerId = invoiceForCredit.CustomerId,
                CustomerOrderId = invoiceForCredit.CustomerOrderId,
                MyCompanyId = invoiceForCredit.MyCompanyId,
                Vat = invoiceForCredit.Vat,
                DebitNoteProducts = new List<Product>()
            };

            dbContext.Documents.Add(debitNote);
            dbContext.SaveChanges();

            if (moreQuantity == true)
            {
                foreach (var product in products)
                {
                    var existProduct = dbContext.Products
                        .Where(d => d.DescriptionId == product.DescriptionId && d.GradeId == product.GradeId && d.SizeId == product.SizeId)
                        .FirstOrDefault();

                    if(existProduct != null)
                    {
                        existProduct.DebitNoteId = debitNote.Id;
                        existProduct.DebitNotePrice = product.Price;
                        existProduct.DebitNoteQuantity = product.Quantity;
                        existProduct.DebitNoteAmount = product.Quantity * product.Price;
                        existProduct.OrderedQuantity += product.Quantity;

                    }
                    else
                    {
                        var newProduct = new Product
                        {
                            DescriptionId = product.DescriptionId,
                            GradeId = product.GradeId,
                            SizeId = product.SizeId,
                            Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), product.Unit, true),
                            DebitNoteId = debitNote.Id,                           
                            DebitNotePrice = product.Price,
                            DebitNoteQuantity = product.Quantity,
                            CustomerOrderId = invoiceForCredit.CustomerOrderId,
                            FSCClaim = product.FscClaim,
                            FSCSertificate = product.FscSertificate
                        };
                        newProduct.DebitNoteAmount = product.Price * product.Quantity;                        
                        dbContext.Products.Add(newProduct);
                    }

                    debitNote.Amount += product.Price * product.Quantity;
                    debitNote.DebitNoteProducts.Add(existProduct);
                }
                debitNote.VatAmount = debitNote.Amount * debitNote.Vat / 100;
                debitNote.TotalAmount = debitNote.Amount + debitNote.VatAmount ?? 0;
            }
            else
            {
                foreach (var product in products)
                {
                    var existProduct = dbContext.Products
                       .Where(d => d.DescriptionId == product.DescriptionId && d.GradeId == product.GradeId && d.SizeId == product.SizeId)
                       .FirstOrDefault();
                }
            }

            dbContext.SaveChanges();

            var customerOrderBalance = dbContext.CustomerOrders
                .Where(o => o.Id == invoiceForCredit.CustomerOrderId)
                .Select(b => b.Balance)
                .FirstOrDefault();

            customerOrderBalance += debitNote.TotalAmount;
            invoiceForCredit.Balance += debitNote.TotalAmount;           

            dbContext.SaveChanges();

            var debitNoteForPrint = documentService.PrintCreditAndDebitNote(debitNote.Id);
            documentService.CreateBgInvoice(debitNote.Id);

            return debitNoteForPrint;
        }
               
    }
}
