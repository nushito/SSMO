using SSMO.Models.Documents.Invoice;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.MyCompany
{
    public class EditCompanyViewModel
    {
        public int Id { get; set; }
        public ICollection<MyCompanyViewModel> MyCompanies { get; set; }
        public MyCompanyEditFormModel Company { get; set; }
    }
}
