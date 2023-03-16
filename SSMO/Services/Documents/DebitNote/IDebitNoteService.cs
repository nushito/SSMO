using SSMO.Models.Documents;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.DebitNote
{
    public interface IDebitNoteService
    {
        public CreditAndDebitNoteViewModel CreateDebitNote
           (int invoiceId, DateTime date, bool moreQuantity, List<AddProductsToCreditAndDebitNoteFormModel> products);
        
    }
}
