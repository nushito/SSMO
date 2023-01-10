using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceCustomerDetailsModel
    {
        public string Name { get; set; }
        public string EIK { get; set; }
        public string VAT { get; set; }
        public string RepresentativePerson { get; set; }
        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
