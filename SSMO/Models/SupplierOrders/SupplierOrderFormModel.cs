﻿using SSMO.Models.CustomerOrders;
using SSMO.Models.Customers;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Services.Curruncies;
using SSMO.Services.Supplier;
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
        public string Supplier { get; set; }
        public IEnumerable<AllSuppliers> Suppliers { get; set; }
        public int CustomerOrderId { get; set; }
        public string CustomerOrderNumber { get; set; }
        public CustomerOrderViewModel CustomerOrder { get; set; }
        public IEnumerable<CustomerOrderViewModel> CustomerOrders { get; set; }
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<MyCompanyFormModel> MyCompanies { get; set; }
        public int StatusId { get; set; }
        public StatusModel Status { get; set; }
        public IEnumerable<StatusModel> Statuses { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }

        public decimal Amount { get; set; }
        public decimal TotalAmount {
            get {
                if(Products == null)
                {
                    return 0;
                }

                var vatAmount = Products.Sum(a => a.PurchaseAmount) * VAT / 100??0;
                return Products.Sum(a => a.PurchaseAmount) + vatAmount;
                    } 
        }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public string DatePaidAmount { get; set; }
        public bool PaidStatus { get; set; }
        public int? VAT { get; set; }

        public IList<ProductViewModel> Products { get; set; }
        public decimal TotalQuantity { get; set; }




    }
}
