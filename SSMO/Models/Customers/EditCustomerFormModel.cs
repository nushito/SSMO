using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Customers
{
    public class EditCustomerFormModel
    {
   
        [Display(Name = "Customer")]
        public string Name { get; set; }
     
        [StringLength(11)]
        public string VAT { get; set; }

        [StringLength(11)]
        public string EIK { get; set; }
        [Display(Name = "Representative person")]
        public string RepresentativePerson { get; set; }

        public CustomerForEditAddressFormModel CustomerAddress { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
