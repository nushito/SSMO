using SSMO.Models.Reports.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.Purchase
{
    public class PurchaseInvoisecBySupplierModel
    {
        public const int PurchaseInvoicesPerPage = 15;
        public int CurrentPage { get; init; } = 1;
        public int? TotalPurchaseInvoices { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Supplier { get; set; }
        public IEnumerable<string> Suppliers { get; set; }
        public IEnumerable<PurchaseInvoicesViewModel> InvoiceCollection { get; set; }
    }
}
