using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SSMO.Models.MyCompany
{
    using static SSMO.Data.ConstantsValidation;
    public class AddBankDetailsFormModel
    {
        public AddBankDetailsFormModel()
        {
           Currency = new List<string>();
          CompanyNames = new List<string>();
        }
        public int Id { get; init; }

        [Required]      
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }

        [Required]
        public string BankName { get; set; }

        [StringLength(IbanLength, ErrorMessage = "Your IBAN is invalid!")]
        public string Iban { get; set; }
        [Required]
        public string Swift { get; set; }
        [Required]
        public string Address { get; set; }
      
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
      public ICollection<string> CompanyNames { get; set; }
      public ICollection<string> Currency { get; set; }


        //  public ICollection<MyCompanyGetNameModel> Company { get; set; } = new List<MyCompanyGetNameModel>();
    }
}
