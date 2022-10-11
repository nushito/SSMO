using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Data.Models
{
    public class ServiceOrder
    {
        public int Id { get; init; }
        public string TransportCompany { get; set; }
        public decimal Cost { get; set; }
        public int Vat { get; set; }
        public bool Paid { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string TruckNumber { get; set; }
        public decimal AmountAfterVat { get; set; }
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public int SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
    }
}
