using System.ComponentModel.DataAnnotations;

namespace SSMO.Services.Documents.Invoice
{
    public class BGCustomerForInvoicePrint
    {
        public string BgName { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your EIK is invalid!")]
        public string EIK { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your VAT is invalid")]
        public string VAT { get; set; }
        public string BgRepresentativePerson { get; set; }

        public BGAddressCustomerForInvoicePrint ClientAddress { get; set; }
    }
}
