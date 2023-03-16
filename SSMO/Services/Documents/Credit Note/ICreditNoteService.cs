using SSMO.Models.Documents;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Credit_Note
{
    public interface ICreditNoteService
    {
        public CreditAndDebitNoteViewModel CreateCreditNote
            (int invoiceId, DateTime date, bool quantityBack, List<AddProductsToCreditAndDebitNoteFormModel> products);

       
    }
}
