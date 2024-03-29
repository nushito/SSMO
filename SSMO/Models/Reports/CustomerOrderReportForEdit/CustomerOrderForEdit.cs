﻿using SSMO.Models.Customers;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Services.Curruncies;
using SSMO.Services.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.Reports.CustomerOrderReportForEdit
{
    public class CustomerOrderForEdit
    {
        public int OrderConfirmationNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerPoNumber { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        [Required]
        public string DeliveryTerms { get; set; }
        public string DeliveryAddress { get; set; }
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<MyCompanyFormModel> MyCompanies { get; set; }
        public int StatusId { get; set; }
        public StatusModel Status { get; set; }
        public IEnumerable<StatusModel> Statuses { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public List<ProductCustomerFormModel> Products { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public string Origin { get; set; }
        public int? Vat { get; set; }
        public decimal PaidAdvance { get; set; }
        public bool PaidAmountStatus { get; set; }
        public int SupplierOrderId { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public IEnumerable<AllSuppliers> Suppliers { get; set; }
        
    }
}
