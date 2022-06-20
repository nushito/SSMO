using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Models.Documents
{
    public class DocumentFormModel
    {
        public string Type { get; set; }
        public int Number { get; set; }
        public ICollection<string> Types { get; set; }
        public IDocument Document { get; set; }
        public SupplierModel Seller { get; set; }
        public string Customer { get; set; }
        public ICollection<string> Customers { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int VatPercent { get; set; }

    }
}
