
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
        public string CorrespondBgCountry { get; set; }
        public string CorrespondBgCity { get; set; }
        public string CorrespondBgStreet { get; set; }
        public string CorrespondCountry { get; set; }
        public string CorrespondCity { get; set; }
        public string CorrespondStreet { get; set; }
        public MyCompany MyCompany { get; set; }       
        public Supplier Suppliers { get; set; }       
        public Customer Customers { get; set; }
        public TransportCompany TransportCompany { get; set; }

    }
}
