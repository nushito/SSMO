using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierInvoiceCollectionViewModel
    {
        public int TotalPurchaseInvoices { get; set; }
        public IEnumerable<SupplierInvoicePaymentDetailsModel> PurchaseInvoices { get; set; }
    }
}
