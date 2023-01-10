
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Suppliers
{
    public class AddSupplierModel
    {
        [Required]
        public string Name { get; set; }
        public string Eik { get; set; }
        public string VAT { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string SupplierAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string RepresentativePerson { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        //   public ICollection<PurchaseProductFormModel> ProductList { get; set; }
        //  public string Currency { get; init; }
        //  [Required]
        //  public string BankName { get; set; }

        ////  [Index(IsUnique = true)]
        //  public string Iban { get; set; }
        //  [Required]
        //  public string Swift { get; set; }
        //  [Required]
        //  public string BankAddress { get; set; }

        //  public IEnumerable<string> Currency { get; set; }
    }
}
