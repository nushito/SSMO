using SSMO.Models.Documents;
using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Reports.CreditNote;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Credit_Note
{
    public interface ICreditNoteService
    {
        public CreditAndDebitNoteViewModel CreateCreditNote
            (int invoiceId, DateTime date, bool quantityBack, string deliveryAddress, List<AddProductsToCreditAndDebitNoteFormModel> products);

        public EditCreditNoteViewModel ViewCreditNoteForEdit(int id);
        public List<InvoiceNumbersForEditedCreditNoteViewModel> InvoiceNumbers();
        public bool EditCreditNote(int id, DateTime date, string incoterms, string truckNumber, decimal netWeight,
            decimal grossWeight, decimal deliveryCost, decimal currencyExchangeRate, string comment, 
            IList<EditProductForCreditNoteViewModel> products);

        public bool AddNewProductsToCreditNoteWhenEdit(int id, int invoiceId,
            List<ProductForCreditNoteViewModelPerInvoice> productsFromInvoice, 
            List<NewProductsForCreditNoteViewModel> newPoducts);
                   
    }
}
