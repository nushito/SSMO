using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.DebitNote;
using SSMO.Models.Reports.DebitNote;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.DebitNote
{
    public interface IDebitNoteService
    {
        public CreditAndDebitNoteViewModel CreateDebitNote
           (int invoiceId, DateTime date,string deliveryAddress,
            List<AddProductsToCreditAndDebitNoteFormModel> products, 
            List<PurchaseProductsForDebitNoteViewModel> availableProducts,string paymentTerms);

        public EditDebitNoteViewModel ViewDebitNoteForEdit(int id);

        public bool EditDebitNote
            (int id, DateTime date, string incoterms, string comment, 
            List<EditProductForDebitNoteViewModel> products, string paymentTerms);

        public ICollection<InvoiceNumbersForEditedDebitNoteViewModel> GetInvoiceNumbers();

        public bool AddNewProductsToDebitNoteWhenEdit(int id, int invoiceId,
            List<NewProductsFromOrderEditedDebitNoteViewModel> products,
             List<NewProductsForEditedDebitNoteFormModel> newProducts,
             IList<PurchaseProductsForDebitNoteViewModel> availableProducts);
    }
}
