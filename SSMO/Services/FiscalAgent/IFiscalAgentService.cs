using SSMO.Models.ServiceOrders;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Services.FiscalAgent
{
    public interface IFiscalAgentService
    {
        public ICollection<FiscalAgentsForServiceOrderVireModel> FiscalAgentsCollection();//kolekciq ID & name na Fiscal Agent
    }
}
