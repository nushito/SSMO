using SSMO.Models.Documents.Invoice;
using SSMO.Models.FscTexts;
using SSMO.Models.Image;
using SSMO.Services.Curruncies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.CustomerOrders
{
    public class CustomerOrderNumbersForInvoiceListView
    {
        public int OrderConfirmationNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public IEnumerable<GetCurrencyModel> Currencies { get; set; }
        public decimal CurrencyExchangeRateUsdToBGN { get; set; }
        public string MyCompanyName { get; set; }
        public IEnumerable<string> MyCompanyNames { get; set; }
        public int? Number { get; set; }
        public string TruckNumber { get; set; }
        public string Swb { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal DeliveryCost { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string Incoterms { get; set; }
        public int Vat { get; set; }
        public string Comment { get; set; }
        public List<int> SelectedCustomerOrders { get; set; }
        public List<ProductsForInvoiceViewModel> Products { get; set; }
        public List<ServiceProductForInvoiceFormModel> ServiceProducts { get; set; }
        public int CustomerId { get; set; }
        public string DealTypeEng { get; set; }
        public string DealDescriptionEng { get; set; }
        public string DealTypeBg { get; set; }
        public string DealDescriptionBg { get; set; }
        public List<int> ChoosenBanks { get; set; }
        public List<int> SelectedBanks { get; set; }
        public ICollection<InvoiceBankDetailsViewModel> CompanyBankDetails { get; set; }
        public ICollection<FiscalAgentViewModel> FiscalAgents { get; set; }        
        public int? FiscalAgent { get; set; }
        public bool IsEur { get; set; }
        public int? FscTextEng { get; set; }
        public ICollection<FscTextViewModel> FscTexts { get; set; }
        public string PaymentTerms { get; set; }
        public string LoadingAddress { get; set; }
        public ICollection<ImageModelViewForAllDocuments> Images { get; set; }
        public int Footer { get; set; }
        public int Header { get; set; }
        public string PlaceOfIssue { get; set; }
        public decimal? Factoring { get; set; }
        public decimal? Comission { get; set; }
    }
}
