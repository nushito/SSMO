using SSMO.Models.Reports;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Documents.BgInvoice
{
    public class BgInvoiceForPrintChooseModel
    {
        public int DocumentNumber { get; set; }
        public ICollection<int> DocumentNumbers { get; set; }
        public int MyCompanyId { get; set; }
        public ICollection<MyCompaniesForReportViewModel> MyCompanies { get; set; }
    }
}
