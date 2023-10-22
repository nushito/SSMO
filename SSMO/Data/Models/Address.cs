
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SSMO.Data.Models
{
    public class Address
    {
        public int Id { get; init; }
        public string BgStreet { get; set; }
        [Required]
        public string Street { get; set; }
        public string BgCity { get; set; }
        [Required]
        public string City { get; set; }
        public string Bgcountry { get; set; }
        [Required]
        public string Country { get; set; }      
        public MyCompany MyCompany { get; set; }       
        public Supplier Suppliers { get; set; }       
        public Customer Customers { get; set; }
        public TransportCompany TransportCompany { get; set; }

    }
}
