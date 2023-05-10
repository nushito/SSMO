using SSMO.Models.Documents.CreditNote;
using System.Collections.Generic;

namespace SSMO.Models.Reports.CreditNote
{
    public class EditedCreditNoteNewProductViewModel
    {
        public int CreditNoteId { get; set; }
        public int InvoiceNumberId { get; set; }
        public List<ProductForCreditNoteViewModelPerInvoice> Products { get; set; }
        public List<NewProductsForCreditNoteViewModel> NewProducts { get; set; }
    }
}
