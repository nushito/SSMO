using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SSMO.Data.Models
{
    public class Customer
    {
        public Customer()
        {
            
            Orders = new HashSet<CustomerOrder>();
        }
       
        public int Id { get; init; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your EIK is invalid!")]
        public string EIK { get; set; }
        [Required]
        [StringLength(11, ErrorMessage ="Your VAT is invalid")]
        public string VAT { get; set; }
        public string RepresentativePerson { get; set; }
        public int AddressId { get; set; }
        [Required]
        public Address ClientAddress { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public ICollection<CustomerOrder> Orders { get; set; }
        public ICollection<Document> Invoices { get; set; }
    }
}
