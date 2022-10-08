using SSMO.Models.Products;
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
               int customerOrderNumber ,
                int statusId, int currencyId, int vat );
        void TotalAmountAndQuantitySum(int supplierOrderId);

        public IEnumerable<string> GetSuppliers();

       

    }
}
