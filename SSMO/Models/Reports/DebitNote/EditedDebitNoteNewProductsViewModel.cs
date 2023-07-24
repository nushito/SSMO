using SSMO.Models.Documents.DebitNote;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.DebitNote
{
    public class EditedDebitNoteNewProductsViewModel
    {
        public int DebitNoteId { get; set; }
        public int InvoiceId { get; set; }
        public List<NewProductsFromOrderEditedDebitNoteViewModel> Products { get; set; }
        public List<NewProductsForEditedDebitNoteFormModel> NewProducts { get; set; }
        public IList<PurchaseProductsForDebitNoteViewModel> PurchaseProducts { get; set; }
    }
}
