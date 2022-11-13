using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Documents.Packing_List
{
    public class MyCompanyForPackingPrint
    {
        public string Name { get; set; }

        [StringLength(9, MinimumLength = 9, ErrorMessage = "EIK number should be 9 symbols long.")]
        public string EIK { get; set; }
 
        [StringLength(11, MinimumLength = 11, ErrorMessage = "VAT number should be 11 symbols long.")]
        public string VAT { get; set; }

        public string Country { get; set; }
       
        public string City { get; set; }
        public string Street { get; set; }

        public string RepresentativePerson { get; set; }

        public string FSCClaim { get; set; }

        public string FSCSertificate { get; set; }
    }
}
