using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Models.MyCompany
{
    public class AddBankViewModel
    {
        public AddBankViewModel()
        {
            Currency = new List<string>();
            CompanyNames = new List<string>();
        }
        public AddBankDetailsFormModel AddBankModel { get; set; }
        public ICollection<string> CompanyNames { get; set; }
         public ICollection<string> Currency { get; set; }
    }
}
