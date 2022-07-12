using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.CustomerOrderService
{
    public interface ICustomerOrderService
    {
        public int CreateOrder
            (string num, DateTime date, int customer, 
            int company, string deliveryTerms, 
            string loadingAddress, string deliveryAddress, int currency);
        
    }
}
