using SSMO.Models.Reports;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Packing_List
{
    public class ChoosePackingListFromInvoicesViewModel
    {
        public int MyCompanyId { get; set; }
        public ICollection<MyCompaniesForReportViewModel> Companies { get; set; }
        public int PackingListNumber { get; set; }       
        public PackingListForPrintViewModel PackingListForPrint { get; set; }
        
    }
}
