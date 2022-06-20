
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SSMO.Models.Suppliers
{
    public class AddBankAccountFormModel
    {
        public AddBankAccountFormModel()
        {
            Currencies = new List<string>();
        }
        public string Currency { get; init; }
        [Required]
        public string BankName { get; set; }
       
        public string Iban { get; set; }
        [Required]
        public string Swift { get; set; }
        [Required]
        public string Address { get; set; }
        public int CompanyId { get; init; }

     public IEnumerable<string> Currencies { get; set; } 
    }
}
