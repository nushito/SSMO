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
                CustomerOrders = invoiceForCredit.CustomerOrders,
                CreditAndDebitNoteProducts = new List<Product>()
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

                    var invoicedProductForBack = dbContext.InvoiceProductDetails
                        .Where(i=>i.ProductId == existProduct.Id)
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
                    existProduct.QuantityAvailableForCustomerOrder += product.Quantity;
                    invoicedProductForBack.CreditNoteBgPrice = product.Price * creditNote.CurrencyExchangeRateUsdToBGN;
                    invoicedProductForBack.CreditNoteBgAmount = product.Price * creditNote.CurrencyExchangeRateUsdToBGN * product.Quantity;

                    creditNote.Amount += product.Price * product.Quantity;
                    creditNote.InvoiceProducts.Add(invoicedProductForBack);
                    creditNote.TotalQuantity += product.Quantity;
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
                        Unit = (Data.Enums.Unit)Enum.Parse(typeof(Data.Enums.Unit), product.Unit, true),
                        DocumentId = creditNote.Id,
                        Pallets = product.Pallets,
                        Price = product.Price,
                        OrderedQuantity = product.Quantity,
                        SheetsPerPallet = product.SheetsPerPallet,                       
                        FscClaim = product.FscClaim,
                        FscSertificate = product.FscSertificate,
                        BgAmount = product.Price * creditNote.CurrencyExchangeRateUsdToBGN * product.Quantity,
                        BgPrice = product.Price * creditNote.CurrencyExchangeRateUsdToBGN,
                        SupplierOrderId = null
                    };
                    newProduct.Amount = product.Price * product.Quantity;
                    creditNote.Amount += product.Price * product.Quantity;
                    creditNote.CreditAndDebitNoteProducts.Add(newProduct);
                    creditNote.TotalQuantity += product.Quantity;
                }
                creditNote.VatAmount = creditNote.Amount * creditNote.Vat / 100;
                creditNote.CreditNoteTotalAmount = creditNote.Amount + creditNote.VatAmount ?? 0;
            }
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
    }
}
