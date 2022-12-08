using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Documents.Invoice
{
    public interface IInvoiceService
    {
        public InvoicePrintViewModel CreateInvoice(
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN,
            int Number, string myCompanyName, string truckNumber, decimal deliveryCost);
        public bool CheckFirstInvoice();
        public EditInvoicePaymentModel InvoiceForEditByNumber(int documentNumber);

        public bool EditInvoicePayment(int documentNumber, bool paidStatus, decimal paidAdvance, DateTime datePaidAmount);
        public ICollection<int> GetInvoiceDocumentNumbers();

        public BgInvoiceViewModel CreateBgInvoice(int documentNumber);    
    }
}
