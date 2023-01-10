namespace SSMO.Models.Reports.Invoice
{
    public class InvoiceBankDetailsModel
    {
       
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string BankName { get; set; }
        public string Iban { get; set; }      
        public string Swift { get; set; }   
        public string Address { get; set; }
        public int CompanyId { get; set; }

    }
}
