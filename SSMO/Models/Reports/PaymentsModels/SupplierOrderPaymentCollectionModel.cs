using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierOrderPaymentCollectionModel
    {
        public int TotalSupplierOrders { get; set; }
        public IEnumerable<SupplierOrdersPaymentDetailsModel> SupplierOrderPaymentCollection { get; set; }
        
    }
}
