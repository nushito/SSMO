using System;

namespace SSMO.Data.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal? NewAmountPerExchangeRate { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int? CurrencyForCalculationsId { get; set; }
        public Currency CurrencyForCalculations { get; set; }
        public decimal? CurruncyRateExchange { get; set; }
        public int? SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public int? DocumentId { get; set; }
        public Document Document { get; set; }
        public int? CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
    }
}
