using SSMO.Data.Models;

namespace SSMO.Models.TransportCompany
{
    public class TransportCompanyFormModel
    {       
        public string Name { get; set; }
        public string Eik { get; set; }
        public string Vat { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string  Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string ContactPerson { get; set; }
    }
}
