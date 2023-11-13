using SSMO.Models.Reports.ServiceOrders.ModelsForPrint;
using System;

namespace SSMO.Models.Reports.ServiceOrders
{
    public class ServiceOrderDetailsPrintViewModel
    {
        public TransportCompanyDetailsOrderPrintViewModel TransportCompany { get; set; }
        public MyCompanyDetailsTransportOrderPrintViewModel MyCompany { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string Etd { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string TruckNumber { get; set; }
        public decimal Cost { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string ProductType { get; set; }
        public string GrossWeight { get; set; }
        public string Comment { get; set; }
        public string Currency { get; set; }
        public string PaymentTerms { get; set; }
        public string PaymentMethod { get; set; }
        public string Payer { get; set; }
        public FiscalAgentDetailsForPrintViewModel FiscalAgent { get; set; } //Customs or fiscal
    }
}
