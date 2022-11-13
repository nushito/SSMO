using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Packing_List
{
    public class CustomerForPackingListPrint
    {
        public string Name { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your EIK is invalid!")]
        public string EIK { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Your VAT is invalid")]
        public string VAT { get; set; }
        public string RepresentativePerson { get; set; }
        public AddressCustomerForPackingList ClientAddress { get; set; }
    }
}
