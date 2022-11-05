using SSMO.Models.MyCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.MyCompany
{
    public interface IMycompanyService
    {
      public ICollection<string> GetCompaniesNames();
        public ICollection<MyCompanyFormModel> GetAllCompanies();
        public string GetUserIdMyCompanyByName(string name);
        public string GetUserIdMyCompanyById(int id); 
        public string GetUserIdMyCompanyBySupplierOrdreNum(string supplierOrder);
        public List<string> MyCompaniesNamePerCustomer(string name);
        public List<string> MyCompaniesNamePerSupplier(string name);
    }
}
