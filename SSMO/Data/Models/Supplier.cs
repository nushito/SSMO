
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Data.Models
{
   
    public class Supplier
    {
        public Supplier()
        {
           
            BankDetails = new List<BankDetails>();
            
         }
     
        public int Id { get; init; }
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(9,ErrorMessage ="Your EIK is not valid")]
        public string Eik { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your EIK is not valid")]

        public string VAT { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public BankDetails BankDetail { get; set; }
        public ICollection<BankDetails> BankDetails { get; init; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
   
        public string FSCClaim { get; set; }
      
        public string FSCSertificate { get; set; }

        public string RepresentativePerson { get; set; }

        public IEnumerable<Product> Products { get; set; }
        // public int ProductId { get; set; }

    }
}