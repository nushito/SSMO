using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.PaymentsModels.TransportModels
{
    public class TransportCompanyOrdersCollectionForPaymentsViewModel
    {
        public ICollection<TransportCompanySelectViewModel> TransportCompanies { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.Date;
        public int Company { get; set; }
        public ICollection<TransportCompanyPaymentCollectionViewModel> CompanyCollectionDetails { get; set; }
    }
}
