using DevExpress.Data.ODataLinq.Helpers;
using SSMO.Data;
using System.Linq;

namespace SSMO.Repository
{
    public class CustomerOrderRepository : ICustomerOrderRepository
    {
        private readonly ApplicationDbContext dbContext;
        public CustomerOrderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    
        public int? GetCustomerOrderNumberById(int? id)
        {
            return dbContext.CustomerOrders
                .Where(i=>i.Id == id)
                .Select(n=>n.OrderConfirmationNumber)
                .FirstOrDefault();
        }

        public string GetCustomerPoNumberById(int? id)
        {
            return dbContext.CustomerOrders
                .Where(i => i.Id == id)
                .Select(n => n.CustomerPoNumber)
                .FirstOrDefault();
        }
    }
}
