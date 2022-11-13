using System.Collections.Generic;

namespace SSMO.Models.Documents.Packing_List
{
    public class ChoosePackingListFromInvoicesViewModel
    {
        public int PackingListNumber { get; set; }
        public ICollection<int> PckingListNumbers { get; set; }
        public PackingListForPrintViewModel PackingListForPrint { get; set; }
        
    }
}
