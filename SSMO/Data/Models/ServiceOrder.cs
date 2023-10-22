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
        public string Etd { get; set; }
        public string Eta { get; set; }
        public string TruckNumber { get; set; }       
        public decimal AmountAfterVat { get; set; }        
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
