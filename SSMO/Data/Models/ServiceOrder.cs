using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Data.Models
{
    public class ServiceOrder
    {
        public int Id { get; init; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public int TransportCompanyId { get; set; }        
        public TransportCompany TransportCompany { get; set; }
        public int MyCompanyId { get; set; }
        public MyCompany MyCompany { get; set; }
        public decimal Cost { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int Vat { get; set; }
        public bool Paid { get; set; }
        public string LoadingAddress { get; set; }      
        public string DeliveryAddress { get; set; }
        public string Etd { get; set; } //Estimate Time of Departure
        public string Eta { get; set; } //Estmate time of arrival
        public string TruckNumber { get; set; }       
        public decimal AmountAfterVat { get; set; }

        //left amount to be paid if payments are > 1
        public decimal Balance { get; set; }
        public int? DocumentId { get; set; }
        public Document Document { get; set; }
        public int? CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int? SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public ICollection<Payment> Payments { get; set; }        
        public int? FiscalAgentId { get; set; }
        public FiscalAgent FiscalAgent { get; set; }
        public string Comment { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public string GrossWeight { get; set; }
        public string PaymentTerms { get; set; }
        public string PaymentMethod { get; set; }
        public string Payer { get; set; }
    }
}
