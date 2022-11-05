using SSMO.Models.Documents.Invoice;
using SSMO.Models.Reports.PaymentsModels;
using System;

namespace SSMO.Services.Documents.Invoice
{
    public interface IInvoiceService
    {
        public InvoicePrintViewModel CreateInvoice(
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN, int Number, string myCompanyName, string truckNumber);
        public bool CheckFirstInvoice();
        public EditInvoicePaymentModel InvoiceForEditById(int id);
    }
}
