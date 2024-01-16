using SSMO.Data.Models;
using SSMO.Models.Reports.Products;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSMO.Repository
{
    public interface IProductRepository
    {
        public Product GetMainProduct(int id);
        Task<ICollection<AllProductsFullNameModel>> ProductsFullName();
        public string GetDescriptionName(int id);
        public string GetGradeName(int id);
        public string GetSizeName(int id);
        Task<ICollection<ProductPurchaseDetails>> ProductFullReport
            (int companyId, ICollection<int> productId, int currentpage, int productsPerPage);        
    }
}
