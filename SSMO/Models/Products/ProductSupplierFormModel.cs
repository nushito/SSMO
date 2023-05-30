﻿
using SSMO.Models.Reports.ProductsStock;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SSMO.Models.Products
{
    public class ProductSupplierFormModel 
    {
        public int Id { get; init; }
        public int DescriptionId { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> DescriptionNames { get; set; }
        public int SizeId { get; set; }
        public string Size { get; set; }
        public IEnumerable<string> SizeNames { get; set; }
        public int GradeId { get; set; }       
        public string Grade { get; set; }
        public IEnumerable<string> GradeNames { get; set; }
        public string FscClaim { get; set; }        
        public string PurchaseFscCertificate { get; set; }
        public string Unit { get; set; }
        public ICollection<string> Units { get; set; }
        public int CustomerOrderId { get; set; }
        public int SupplierOrderId { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal PurchasePrice { get; set; }
        public decimal PurchaseAmount { get; set; }
        public int Pallets { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Quantity { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal OrderedQuantity { get; set; }
        public int SheetsPerPallet { get; set; }
      
        public IEnumerable<DescriptionForProductSearchModel> Descriptions { get; set; }
       
        public IEnumerable<SizeForProductSearchModel> Sizes { get; set; }
      
        public IEnumerable<GradeForProductSearchModel> Grades { get; set; }
    }
    }
