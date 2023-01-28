using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Documents.BgInvoice
{
    public class BgInvoiceForPrintChooseModel
    {
        public int DocumentNumber { get; set; }
        public ICollection<int> DocumentNumbers { get; set; }
    }
}
