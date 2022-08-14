using System;

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

       
    }
}
