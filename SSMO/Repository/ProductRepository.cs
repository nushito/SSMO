using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using System.Linq;

namespace SSMO.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext dbContext;
        public ProductRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Product GetMainProduct(int id)
        {
            var mainProduct = dbContext.Products
                 .Where(a => a.Id == id)
                 .FirstOrDefault();

            return mainProduct;          
        }
    }
}
