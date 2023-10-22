using SSMO.Data;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace SSMO.Services.TransportService
{
    public class TransportService : ITransportService
    {
        private readonly ApplicationDbContext dbContext;
        public TransportService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool FirstTransport()
        {
            var check = dbContext.ServiceOrders.Any() == true;
            if (!check)
            {
                return true;

            }
            return false;
        }
    }
}
