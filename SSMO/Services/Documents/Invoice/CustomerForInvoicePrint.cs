using System.ComponentModel.DataAnnotations;

namespace SSMO.Services.Documents.Invoice
{
    public class CustomerForInvoicePrint
    {
        public string Name { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your EIK is invalid!")]
        public string EIK { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your VAT is invalid")]
        public string VAT { get; set; }
        public string RepresentativePerson { get; set; }
      
        public AddressCustomerForInvoicePrint ClientAddress { get; set; }
    }
}
