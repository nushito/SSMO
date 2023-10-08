using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.MyCompany
{
    public class MyCompanyEditFormModel
    {
        public string BgName { get; set; }
        public string Name { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "EIK number should be 9 symbols long.")]
        public string EIK { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "VAT number should be 11 symbols long.")]
        public string VAT { get; set; }
        public string BgCountry { get; set; }
        public string BgCity { get; set; }
        public string BgStreet { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string RepresentativePerson { get; set; }
        public string BgRepresentativePerson { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int AddressId { get; set; }
    }
}
