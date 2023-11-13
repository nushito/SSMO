using SSMO.Data;
using SSMO.Models.ServiceOrders;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.FiscalAgent
{
    public class FiscalAgentService : IFiscalAgentService
    {
        private readonly ApplicationDbContext dbContext;
        public FiscalAgentService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //kolekciq ID & name na Fiscal Agent 
        public ICollection<FiscalAgentsForServiceOrderVireModel> FiscalAgentsCollection()
        {
            return dbContext.FiscalAgents               
                .Select(c=> new FiscalAgentsForServiceOrderVireModel
                {
                    FiscalAgentId = c.Id,
                    FiscalAgentName= c.Name,
                })
                .ToList();
        }
    }
}
