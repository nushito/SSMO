using System;
using System.Collections;
using System.Collections.Generic;

namespace SSMO.Models.Reports.FSC
{
    public class ProductsFscCollectionViewModel
    {
        public const int ProductsOnPage = 15;
        public int CurrentPage { get; set; } = 1;
        public int TotalProducts { get; set; }
        public string MyCompany { get; set; }
        public ICollection<string> MyCompanies { get; set; }
        public string FSCClaim { get; set; }
        public ICollection<string> FscClaims { get; set; }
        public string FSCCertificate { get; set; }
        public ICollection<string> FSCCertificates { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<ProductFscCollectionReportViewModel> Products { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
