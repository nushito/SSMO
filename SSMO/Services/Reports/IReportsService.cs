using SSMO.Models.CustomerOrders;
using System;
using System.Collections.Generic;

namespace SSMO.Services.Reports
{
    public interface IReportsService
    {
        IEnumerable<CustomerOrderViewModel> AllCustomerOrders(string name);
        CustomerOrderViewModel Details(int id);

        bool Edit(int id,string number,
                System.DateTime date,
                int clientId,
                int myCompanyId,
                string deliveryTerms,
                string loadingPlace,
                string deliveryAddress,
               int currencyId, string status, string fscClaim,string fscCertificate);
        
    }
}
