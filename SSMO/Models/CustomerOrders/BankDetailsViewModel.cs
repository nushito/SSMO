using System.ComponentModel.DataAnnotations;

namespace SSMO.Models.CustomerOrders
{
    public class BankDetailsViewModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Iban { get; set; }
        public int CurrencyId { get; set; }
        public string Swift { get; set; }
        public string CurrencyName { get; set; }       
    }
}
