using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.Documents;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Services.Curruncies;
using SSMO.Services.Suppliers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSMO.Models.SupplierOrders
{
    public class SupplierOrderFormModel
    {
        public string Number { get; set; }
       
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int SupplierId { get; set; }
        
        public IEnumerable<AllSuppliers> Suppliers { get; set; }       
        public string FscClaim { get; set; }        
        public int MyCompanyId { get; set; }
        public IEnumerable<MyCompaniesForReportViewModel> MyCompanies { get; set; }
        public int StatusId { get; set; }
        public StatusModel Status { get; set; }
        public IEnumerable<StatusModel> Statuses { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public string DeliveryTerms { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }

        [Range(0.0, 9999999999999.99999)]
        public decimal PaidAvance { get; set; }
        [Range(0.0, 9999999999999.99999)]
        public decimal Balance { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public int? VAT { get; set; }
      
        [Range(0.0, 9999999999999.99999)]      
        public ICollection<ProductSupplierFormModel> ProductList { get; set; }
        public List<string> SupplierFscCertificate { get; set; }
    }
}
