using SSMO.Models.Documents.Invoice;
using SSMO.Services.Curruncies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderNumbersForInvoiceListView
    {
        public int OrderConfirmationNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<string> MyCompanyNames { get; set; }
        public int? Number { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string Incoterms { get; set; }
        public int Vat { get; set; }
        public string Comment { get; set; }
        public List<int> SelectedCustomerOrders { get; set; }
        public List<ProductsForInvoiceViewModel> Products { get; set; }
        public List<ServiceProductForInvoiceFormModel> ServiceProducts { get; set; }
        public int CustomerId { get; set; }
    }
}
