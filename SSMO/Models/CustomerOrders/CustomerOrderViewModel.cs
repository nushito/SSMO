using SSMO.Models.Customers;
using SSMO.Models.MyCompany;
using SSMO.Models.Products;
using SSMO.Models.Reports;
using SSMO.Services.Curruncies;
using SSMO.Services.Suppliers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderViewModel
    {
        public CustomerOrderViewModel()
        {
            
        }

        public int Id { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public string Type { get; set; }
        public string CustomerPoNumber { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public string LoadingPlace { get; set; }
        [Required]
        public string DeliveryTerms { get; set; }
        public string DeliveryAddress { get; set; }
        public int CustomerId { get; set; }
        public AddCustomerFormModel Customer { get; set; }
        public IEnumerable<AddCustomerFormModel> Customers { get; set; }
        public int MyCompanyId { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<MyCompaniesForReportViewModel> MyCompanies { get; set; }
        public int StatusId { get; set; }
        public StatusModel Status { get; set; } 
        public IEnumerable<StatusModel> Statuses { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public IList<ProductCustomerFormModel> Products { get; set; }
        public string Comment { get; set; }
        public string Origin { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public int? Vat { get; set; }
        public decimal PaidAvance { get; set; }
        public decimal Balance { get; set; }
        public bool PaidAmountStatus { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public int SupplierOrderId { get; set; }
        public int SupplierId { get; set; }    
        public ICollection<int> SelectedSupplierOrders { get; set; }
        public List<int> ChosenBanks { get; set; }
        public ICollection<BankDetailsViewModel> BankDetails { get; set; }
        public int ProductsCount { get; set; }
        public IEnumerable<SupplierOrdersBySupplier> SupplierOrdersBySupplier { get; set; }
        public int FiscalAgentId { get; set; }
        public ICollection<FiscalAgentViewModel> FiscalAgents { get; set; }
        public string DealType { get; set; }
        public string DealDescription { get; set; }
    }
}
