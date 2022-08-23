using SSMO.Services.Documents.Purchase;
using System.Collections.Generic;

namespace SSMO.Services.Documents
{
    public interface IPurchaseService
    {
        public IEnumerable<PurchaseModelAsPerSpec> GetSupplierOrders(string
            supplierName,int currentpage, int supplierOrdersPerPage);
    }
}
