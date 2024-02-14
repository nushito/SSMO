using System;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels
{
    public class SupplierOrdersPaymentReportViewModel
    {
        public const int supplierOrderPerPage = 15;
        public string SupplierName { get; set; }
        public int CurrentPage { get; init; } = 1;
        public int TotalSupplierOrders { get; set; }
        public int MyCompanyId { get; init; }
        public IEnumerable<MyCompaniesForReportViewModel> MyCompanyNames { get; set; }
        public IEnumerable<string> SupplierNames { get; set; }
        public IEnumerable<SupplierOrdersPaymentDetailsModel> SupplierOrderPaymentCollection { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
