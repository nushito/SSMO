using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.DebitNote;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.DebitNote
{
    public interface IDebitNoteService
    {
        public CreditAndDebitNoteViewModel CreateDebitNote
           (int invoiceId, DateTime date,bool moreQuantity,string deliveryAddress,
            List<AddProductsToCreditAndDebitNoteFormModel> products, List<PurchaseProductsForDebitNoteViewModel> availableProducts);

        public EditDebitNoteViewModel ViewDebitNoteForEdit(int id);

        public ICollection<InvoiceNumbersForEditedDebitNoteViewModel> GetInvoiceNumbers();
    }
}
