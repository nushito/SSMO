using SSMO.Models.Customers;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Services.Curruncies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderPrintViewModel
    {
        public int OrderConfirmationNumber { get; set; }
        public string Type { get; set; }
        public string CustomerPoNumber { get; set; }     
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }     
        public string DeliveryTerms { get; set; }
        public string DeliveryAddress { get; set; }      
        public AddCustomerFormModel Customer { get; set; }  
        public MyCompanyDetailsPrintViewModel MyCompany { get; set; }
        public string Currency { get; set; }    
        public IList<ProductCustomerFormModel> Products { get; set; }
        public string Comment { get; set; }
        public string Origin { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Vat { get; set; }      
        public decimal Amount { get; set; }      
        public decimal SubTotal { get; set; }
        public string DealType { get; set; }
        public string DealDescription { get; set; }
        public ICollection<BankDetailsViewModel> BankDetails { get; set; }
        public FiscalAgentPrintViewModel FiscalAgent { get; set; }
    }
}
