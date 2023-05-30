using SSMO.Data.Models;

namespace SSMO.Repository
{
    public interface IProductRepository
    {
        public Product GetMainProduct(int id);
    }
}
