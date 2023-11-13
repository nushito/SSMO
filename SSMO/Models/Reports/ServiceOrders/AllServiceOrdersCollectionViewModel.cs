using SSMO.Models.Reports.SupplierOrderReportForEdit;
using SSMO.Models.ServiceOrders;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.ServiceOrders
{
    public class AllServiceOrdersCollectionViewModel
    {
        public const int serviceOrdersPerPage = 15;
        public ICollection<MyCompaniesForTrasnportOrderViewModel> MyCompanies { get; set; }
        public int CurrentPage { get; init; } = 1;
        public int TotalServiceOrders { get; set; }
        public int MyCompanyId { get; set; }
        public int TransportCompanyId { get; set; }      
        public IEnumerable<ServiceOrderCollectionDetailViewModel> ServiceOrders { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
