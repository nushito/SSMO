using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Suppliers
{
    public class EditSupplierFormModel
    {
       
        public string Name { get; set; }
        public string Eik { get; set; }
        public string VAT { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public EditSupplierAddressFormModel SupplierAddress { get; set; }
        public string FSCSertificate { get; set; }
        public string RepresentativePerson { get; set; }
    }
}