using System;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Packing_List
{
    public class PackingListForPrintViewModel
    {
        public string DocumentType { get; set; }
        public DateTime Date { get; set; }
        public int DocumentNumber { get; set; }
        public int? CustomerId { get; set; }
        public string Incoterms { get; set; }
        public string FSCClaim { get; set; }
        public string FSCSertificate { get; set; }
        public int? MyCompanyId { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string TruckNumber { get; set; }
        public MyCompanyForPackingPrint MyCompanyForPl { get; set; }
        public ICollection<ProductsForPackingListPrint> Products { get; set; }
        public CustomerForPackingListPrint Customer { get; set; }

    }
}
