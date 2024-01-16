using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Services.Curruncies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Reports.SupplierOrderReportForEdit
{
    public class SupplierOrderForEditModel
    {
       
        public string Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int CustomerOrderId { get; set; }
        public int CustomerOrderNumber { get; set; }
        public int MyCompanyId { get; set; }
        public IEnumerable<MyCompaniesForReportViewModel> MyCompanies { get; set; }
        public int StatusId { get; set; }
        public IEnumerable<StatusModel> Statuses { get; set; }
        public int CurrencyId { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public decimal Amount { get; set; }       
        public decimal TotalAmount { get; set; }      
        public int? VAT { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string  DeliveryTerms { get; set; }
        public string LoadingAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalQuantity { get; set; }
        public List<ProductsForEditSupplierOrder> Products { get; set; }
        public List<NewProductsForSupplierOrderModel> NewProducts { get; set; }
    }
}

