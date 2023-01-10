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
        public string BgCustomerName { get; set; }
        public string VAT { get; set; }
        public string EIK { get; set; }
        [Display(Name ="Representative person")]
        public string RepresentativePerson { get; set; }
        public string BgRepresentativePerson { get; set; }

        [Required]
        public string Country { get; set; }
        public string BgCountry { get; set; }
        [Required]
        public string City { get; set; }
        public string BgCity { get; set; }
        [Required]
        public string Street { get; set; }
        public string BgStreet { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
