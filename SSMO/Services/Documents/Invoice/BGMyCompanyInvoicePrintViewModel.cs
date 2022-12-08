using System.ComponentModel.DataAnnotations;

namespace SSMO.Services.Documents.Invoice
{
    public class BGMyCompanyInvoicePrintViewModel
    {
        [Required]
        public string BgName { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "EIK number should be 9 symbols long.")]
        public string EIK { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "VAT number should be 11 symbols long.")]
        public string VAT { get; set; }

        [Required]
        public string BgCountry { get; set; }
        [Required]
        public string BgCity { get; set; }
        [Required]
        public string BgStreet { get; set; }

        public string BgRepresentativePerson { get; set; }

        public string FSCClaim { get; set; }

        public string FSCSertificate { get; set; }
    }
}
