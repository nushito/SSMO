using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Suppliers
{
    public class EditSupplierFormModel
    {
       
        public string Name { get; set; }

        [StringLength(9, MinimumLength = 9, ErrorMessage = "EIK number should be 9 symbols long.")]
        public string Eik { get; set; }

        [StringLength(11, MinimumLength = 9, ErrorMessage = "VAT number should be 11 symbols long.")]
        public string VAT { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public EditSupplierAddressFormModel SupplierAddress { get; set; }
        public string FSCSertificate { get; set; }
    }
}