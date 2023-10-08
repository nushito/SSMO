namespace SSMO.Models.Documents.Invoice
{
    public class InvoiceBankDetailsViewModel
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; init; }
        public string BankName { get; set; }
        public string Iban { get; set; }
        public string Swift { get; set; }
    }
}
