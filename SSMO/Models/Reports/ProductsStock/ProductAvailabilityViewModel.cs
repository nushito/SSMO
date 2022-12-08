using System.Collections.Generic;

namespace SSMO.Models.Reports.ProductsStock
{
    public class ProductAvailabilityViewModel
    {
        public const int ProductsPerPage = 15;
        public int CurrentPage { get; set; }
        public int? TotalProducts { get; set; }
        public int DescriptionId { get; set; }
        public IEnumerable<DescriptionForProductSearchModel> Descriptions { get; set; }
        public int SizeId { get; set; }
        public IEnumerable<SizeForProductSearchModel> Sizes { get; set; }
        public int GradeId { get; set; }
        public IEnumerable<GradeForProductSearchModel> Grades { get; set; }
        public IEnumerable<ProductAvailabilityDetailsViewModel> ProductsDetails { get; set; }

    }
}
