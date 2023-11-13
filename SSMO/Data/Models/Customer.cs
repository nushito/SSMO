﻿using System;
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
        public string BgCustomerName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string EIK { get; set; }
        [Required]
        public string VAT { get; set; }
        public string BgCustomerRepresentativePerson { get; set; }
        public string RepresentativePerson { get; set; }
        public int AddressId { get; set; }       
        public Address Address { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public ICollection<CustomerOrder> Orders { get; set; }
        public ICollection<Document> Invoices { get; set; }
    }
}
