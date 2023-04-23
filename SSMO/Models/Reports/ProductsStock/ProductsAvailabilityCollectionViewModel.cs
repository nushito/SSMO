using System.Collections.Generic;

namespace SSMO.Models.Reports.ProductsStock
{
    public class ProductsAvailabilityCollectionViewModel
    {
        public IEnumerable<ProductAvailabilityDetailsViewModel> Products { get; set; }
        public int TotalProducts { get; set; }
    }
}
