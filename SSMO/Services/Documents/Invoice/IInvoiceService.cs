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
            int Number, string myCompanyName, string truckNumber, decimal deliveryCost,decimal grossWeight, decimal netWeight);
        public bool CheckFirstInvoice();
        public EditInvoicePaymentModel InvoiceForEditByNumber(int documentNumber);
        public bool EditInvoicePayment(int documentNumber, bool paidStatus, decimal paidAdvance, DateTime datePaidAmount);
        public ICollection<int> GetInvoiceDocumentNumbers();
        public BgInvoiceViewModel CreateBgInvoice(int documentNumber, decimal currencyExchangeRateUsdToBGN);
        public EditInvoiceViewModel ViewEditInvoice(int id);
        public bool EditInvoice
          (int id, decimal currencyExchangeRate, DateTime date, decimal grossWeight,
          decimal netWeight, decimal deliveryCost, int orderConfirmationNumber, string truckNumber);

        public void EditPackingList(int id);

    }
}
