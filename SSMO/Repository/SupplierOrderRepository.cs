using DevExpress.Data.ODataLinq.Helpers;
using SSMO.Data;
using System.Linq;

namespace SSMO.Repository
{
    public class SupplierOrderRepository : ISupplierOrderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SupplierOrderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string GetSupplierOrderNumberById(int? id)
        {
            return dbContext.SupplierOrders
                .Where(i=>i.Id == id)
                .Select(n=>n.Number)
                .FirstOrDefault();
        }
    }
}
