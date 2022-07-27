using SSMO.Models.Products;
using System;

namespace SSMO.Services.CustomerOrderService
{
    public interface ICustomerOrderService
    {
        public int CreateOrder
            (string num, DateTime date, int customer, 
            int company, string deliveryTerms, 
            string loadingAddress, string deliveryAddress, int currency);

        public SSMO.Data.Models.CustomerOrder OrderPerIndex(int id);

        public bool CheckOrderNumberExist(string number); 
      
        
    }
}
