using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace SSMO.Models.TransportCompany
{
    public class EditTransportCompanyModel
    {
        public int Id { get; set; }
        public List<TransportCompanyListViewModel>  TransportCompanies { get; set; }
        public EditTransportCompanyFormModel TransportCompany { get; set; }
    }
}
