using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;

namespace SSMO.Models.Documents.DebitNote
{
    public class JsonProductsList
    {
      
        public string Description { get; set; }
   
        public string Size { get; set; }
   
        public string Grade { get; set; }

        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
