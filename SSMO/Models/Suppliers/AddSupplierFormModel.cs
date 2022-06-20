
using System.ComponentModel.DataAnnotations;


namespace SSMO.Models.Suppliers
{
    public class AddSupplierFormModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [StringLength(9, MinimumLength = 9, ErrorMessage = "EIK number should be 9 symbols long.")]
        public string Eik { get; set; }

        [StringLength(11, MinimumLength = 9, ErrorMessage = "VAT number should be 11 symbols long.")]
        public string VAT { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public string RepresentativePerson { get; set; }
     //   public ICollection<ProductSpecificationFormModel> ProductList { get; set; }
    }
}
