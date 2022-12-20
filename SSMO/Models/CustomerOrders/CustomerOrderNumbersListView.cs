using SSMO.Models.Documents.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderNumbersListView
    {
        public int OrderConfirmationNumber { get; set; }
        public DateTime Date { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<string> MyCompanyNames { get; set; }
        public int? Number { get; set; }
        public string TruckNumber { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public IEnumerable<int> OrderConfirmationNumberList { get; set; }
    }
}
