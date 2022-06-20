using SSMO.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Customers
{
    public class AddCustomerFormModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public string Name { get; set; }
        [Required]
        [StringLength(11)]
        public string VAT { get; set; }
        [Required]
        [StringLength(11)]
        public string EIK { get; set; }
        [Display(Name ="Representative person")]
        public string RepresentativePerson { get; set; }

        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
       
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
