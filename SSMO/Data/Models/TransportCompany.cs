using System.Collections;
using System.Collections.Generic;

namespace SSMO.Data.Models
{
    public class TransportCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Eik { get; set; }
        public string Vat { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public string ContactPerson { get; set; }
        public string UserId { get; set; }
        public ICollection<ServiceOrder> ServiceOrders { get; set; }
    }
}
