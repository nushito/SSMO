using SSMO.Models.MyCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.MyCompany
{
    public interface IMycompanyService
    {
      public ICollection<string> GetCompany();
        public ICollection<MyCompanyFormModel> GetAllCompanies();
     
    }
}
