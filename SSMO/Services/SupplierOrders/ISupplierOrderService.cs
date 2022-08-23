using SSMO.Services.Documents.Purchase;
using System;
using System.Collections.Generic;

namespace SSMO.Services.SupplierOrders
{
    public interface ISupplierOrderService
    {
        public int CreateSupplierOrder(int myCompanyId,
                int supplierId,
                DateTime Date,
                string number,
               string customerOrderNumber ,
                int statusId, int currencyId, int vat );
        void TotalAmountSum(int supplierOrderId);

        public IEnumerable<string> GetSuppliers();
      
    }
}
