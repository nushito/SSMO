
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SSMO.Data.Models
{
    public class Address
    {
        public int Id { get; init; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<MyCompany> MyCompanies { get; set; }

    }
}
