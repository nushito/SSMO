using SSMO.Models.Documents.Invoice;
using SSMO.Models.ServiceOrders;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Products
{
    public class ProductsDetailsViewModel
    {
        public const int productsPerPage = 15;
        public ICollection<MyCompanyViewModel> MyCompanies { get; set; }
        public int CurrentPage { get; init; } = 1;
        public int TotalProducts { get; set; }
        public int MyCompanyId { get; set; }
        public ICollection<AllProductsFullNameModel> ProductsName { get; set; }
        public string ProductsNums { get; set; }
        public ProductsQueryDetailsModel Products { get; set; }
    }
}
