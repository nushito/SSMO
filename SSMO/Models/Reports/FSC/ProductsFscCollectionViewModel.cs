using SSMO.Models.Documents.Invoice;
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
        public int MyCompanyId { get; set; }
        public string MyCompany { get; set; }
        public string PurchaseOrSell { get; set; }
        public ICollection<MyCompanyViewModel> MyCompanies { get; set; }
        public string FSCClaim { get; set; }
        public ICollection<string> FscClaims { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<PurchaseProductFscCollectionViewModel> PurchaseProducts { get; set; }
        public ICollection<SoldProductsFscCollectionViewModel> SoldProducts { get; set; }
        public decimal PurchaseTotalQuantity { get; set; }
        public decimal SoldTotalQuantity { get; set; }
    }
}
