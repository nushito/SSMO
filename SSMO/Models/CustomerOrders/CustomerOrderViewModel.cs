using SSMO.Models.Currency;
using SSMO.Models.Customers;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Services.Curruncies;
using SSMO.Services.Supplier;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderViewModel
    {
        public int Id { get; init; }
        public string Number { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        [Required]
        public string DeliveryTerms { get; set; }
        public string DeliveryAddress { get; set; }
        public int ClientId { get; set; }
        public AddCustomerFormModel Customer { get; set; }

        public IEnumerable<AddCustomerFormModel> Customers { get; set; }
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<MyCompanyFormModel> MyCompanies { get; set; }
        public string Status { get; set; } = "active";
        public IEnumerable<StatusModel> Statuses { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
        public string FscClaim { get; set; }
        public string FscCertificate { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Vat { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public bool PaidAmountStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public int SupplierOrderId { get; set; }
        public string Supplier { get; set; }
        public IEnumerable<AllSuppliers>  Suppliers { get; set; }
    }
}
