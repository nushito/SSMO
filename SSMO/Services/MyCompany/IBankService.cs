
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using SSMO.Models.CustomerOrders;
using System.Collections.Generic;

namespace SSMO.Services.MyCompany
{
   public interface IBankService
    {
        public int Create(
            int currencyId,            
            string bankName, 
            string iban, 
            string swift, 
            string address, 
            string companyName, 
            int companyId);

        public ICollection<BankDetailsViewModel> GetMyBanks(int customerOrderId);

    }
}
