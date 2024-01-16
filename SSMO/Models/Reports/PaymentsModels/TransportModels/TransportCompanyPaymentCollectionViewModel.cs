using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels.TransportModels
{
    public class TransportCompanyPaymentCollectionViewModel
    {
        public string Payer { get; set; }     
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal AmountAfterVat { get; set; }
        public decimal Balance { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public int? CustomerOrderId { get; set; }
        public int? OrderConfirmationNumber { get; set; }
        public int? SupplierOrderId { get; set; }
        public string SupplierOrderNumber { get; set; }
        public ICollection<TransportCompanyPaymentsDetailsViewModel> TransportPayments { get; set; }
    }
}
