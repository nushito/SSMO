using SSMO.Models.Documents.CreditNote;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Invoice
{
    public interface IInvoiceService
    {
        public InvoicePrintViewModel CreateInvoice(
            List<int> selectedCustomerOrderNumbers, List<ProductsForInvoiceViewModel> products, 
            DateTime date, decimal currencyExchangeRateUsdToBGN,
            int number,string myCompanyName, string truckNumber, decimal deliveryCost, string swb, 
            decimal netWeight, decimal grossWeight,string incoterms, int customerId, int currencyId, int vat, int myCompanyId);
        public bool CheckFirstInvoice(int myCompanyId);
        public EditInvoicePaymentModel InvoiceForEditByNumber(int documentNumber);
        public bool EditInvoicePayment(int documentNumber, bool paidStatus, decimal paidAdvance, DateTime datePaidAmount);
        public ICollection<InvoiceNumbersJsonList> GetInvoiceDocumentNumbers(int id);
        public BgInvoiceViewModel CreateBgInvoiceForPrint
            (int documentNumber);       
        public EditInvoiceViewModel ViewEditInvoice(int id);
        public bool EditInvoice
          (int id, decimal currencyExchangeRate, DateTime date, decimal grossWeight,
          decimal netWeight, decimal deliveryCost, int orderConfirmationNumber, string truckNumber, 
          int invoiceForCreditNote, int invoiceForDebitNote, ICollection<EditProductForCreditAndDebitViewModel> products);

        public void EditPackingList(int id);

    }
}
