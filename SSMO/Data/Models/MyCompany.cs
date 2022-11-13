using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SSMO.Data;

namespace SSMO.Data.Models
{
    using static ConstantsValidation;
    public class MyCompany 
    {
        public MyCompany()
        {
           
            BankDetails = new HashSet<BankDetails>();
          
            Orders = new HashSet<CustomerOrder>();

        }
        public int Id { get; init; } 
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(EikLength, MinimumLength = EikLength, ErrorMessage = "EIK number should be 9 symbols long.")]
        public string Eik { get; set; }
        [Required]
        [StringLength(VatLength, MinimumLength = VatLength, ErrorMessage = "VAT number should be 11 symbols long.")]
        public string VAT { get; set; }
        public int AddressId { get; set; }
        [Required]
        public Address Address { get; set; }
        [Required]
        public string RepresentativePerson { get; set; }
        public string UserId { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }

        public ICollection<CustomerOrder> Orders { get; set; }
        public ICollection<SupplierOrder> SupplierOrders { get; set; }
        public ICollection<BankDetails> BankDetails { get; set; }
        public ICollection<Document> Documents { get; set; }





    }
}
