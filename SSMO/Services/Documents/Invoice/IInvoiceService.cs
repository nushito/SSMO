using SSMO.Models.Documents.Invoice;
using System;

namespace SSMO.Services.Documents.Invoice
{
    public interface IInvoiceService
    {
        public InvoicePrintViewModel CreateInvoice(
            int orderConfirmationNumber, DateTime date, decimal currencyExchangeRateUsdToBGN, int Number, string myCompanyName, string truckNumber);

        public bool CheckFirstInvoice();
    }
}
