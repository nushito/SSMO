using System.Collections.Generic;

namespace SSMO.Models.Reports.Purchase
{
    public class PurchaseCollectionQueryModel
    {
        public int TotalPurchaseInvoices { get; set; }
        public IEnumerable<PurchaseInvoicesViewModel> PurchaseInvoiceCollection { get; set; }
    }
}
