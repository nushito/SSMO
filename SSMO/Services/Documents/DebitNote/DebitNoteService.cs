using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Migrations;
using SSMO.Data.Models;
using SSMO.Models.Documents;
using SSMO.Models.Documents.DebitNote;
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
                        .Where(a => a.DescriptionId == product.DescriptionId 
                        && a.GradeId == product.GradeId 
                        && a.SizeId == product.SizeId 
                        && a.InvoiceProductDetails.Select(i => i.InvoiceId)
                                                  .ToList()
                                                  .Contains(invoiceForDebit.Id))
                        .FirstOrDefault();

                    if(mainProduct != null)
                    {
                        var productForDebit = dbContext.InvoiceProductDetails
                             .Where(co => co.ProductId == mainProduct.Id)
                             .FirstOrDefault();

                        productForDebit.DebitNoteId = debitNote.Id;
                        productForDebit.DebitNotePrice = product.Price;
                        productForDebit.DebitNoteQuantity = product.Quantity;
                        productForDebit.DebitNoteAmount = product.Quantity * product.Price;
                        productForDebit.DebitNoteBgAmount = product.Price * debitNote.CurrencyExchangeRateUsdToBGN * product.Quantity;
                        productForDebit.DebitNoteBgPrice = product.Price * debitNote.CurrencyExchangeRateUsdToBGN;                       
                        productForDebit.TotalSheets = product.Pallets * product.SheetsPerPallet;

                        if(moreQuantity == true)
                        {

                        }
                    }
                    else
                    {
                        var newProduct = dbContext.Products
                        .Where(a => a.DescriptionId == product.DescriptionId
                        && a.GradeId == product.GradeId
                        && a.SizeId == product.SizeId)                       
                        .FirstOrDefault();
                     
                        if(newProduct == null) 
                        {
                            newProduct = new Product
                            {
                                DescriptionId = product.DescriptionId,
                                GradeId = product.GradeId,
                                SizeId = product.SizeId,
                                InvoiceProductDetails = new List<InvoiceProductDetails>(),
                                DocumentId = debitNote.Id
                            };
                        }
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

                            debitProduct.TotalSheets = product.Pallets * product.SheetsPerPallet;
                            newProduct.InvoiceProductDetails.Add(debitProduct);
                            dbContext.Products.Add(newProduct);

                        }
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

        public EditDebitNoteViewModel ViewDebitNoteForEdit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
