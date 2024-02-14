
using SSMO.Models.CustomerOrders;
using SSMO.Models.FscTexts;
using SSMO.Models.Image;
using SSMO.Models.Reports.Invoice;
using System;
using System.Collections.Generic;

namespace SSMO.Models.Documents.Invoice
{
    public class EditInvoiceViewModel
    {
        public string DocumentType { get; set; }
        public int OrderConfirmationNumber { get; set; }
        public IEnumerable<int> OrderConfirmationNumbers { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<string> Currencies { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string TruckNumber { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal? BankExpenses { get; set; }
        public decimal? Duty { get; set; }
        public decimal? CustomsExpenses { get; set; }       
        public decimal? FiscalAgentExpenses { get; set; }      
        public decimal? OtherExpenses { get; set; }
        public decimal CurrencyExchangeRate { get; set; }
        public string Incoterms { get; set; }
        public string Comment { get; set; }
        public int? FiscalAgentId { get; set; }
        public string FiscalAgentName { get; set; }
        public ICollection<FiscalAgentViewModel> FiscalAgents { get; set; }
        public int? FscTextEng { get; set; }
        public string FscText { get; set; }
        public ICollection<FscTextViewModel>  FscTexts { get; set; }
        public List<EditProductForCompanyInvoicesViewModel> Products { get; set; }
        public int? CustomerId { get; set; }      
        public ICollection<CustomerCollectionForChoosingNewOrderForInvoiceEditViewModel> Customers { get; set; }
        public List<int> SelectedCustomerOrders { get; set; }
        public List<int> ChosenBanks { get; set; }
        public ICollection<BankDetailsViewModel> CompanyBankDetails { get; set; }
        public string PaymentTerms { get; set; }
        public string DealType { get; set; }
        public string DealDescription { get; set; }
        public string DeliveryAddress { get; set; }
        public string LoadingAddress { get; set; }
        public int Header { get; set; }
        public int Footer { get; set; }
        public ICollection<ImageModelViewForAllDocuments> Images { get; set; }
        public string PlaceOfIssue { get; set; }
        public decimal? Factoring { get; set; }
        public decimal? Comission { get; set; }
        public int MyCompanyId { get; set; }
        public string CustomsExportDeclaration { get; set; }
    }
}
