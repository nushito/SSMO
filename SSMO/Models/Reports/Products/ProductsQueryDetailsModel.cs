using System.Collections.Generic;

namespace SSMO.Models.Reports.Products
{
    public class ProductsQueryDetailsModel
    {
        public int TotalProducts { get; set; }       
        public ICollection<ProductPurchaseDetails> Products { get; set; }
    }
}
