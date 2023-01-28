
using System.Collections.Generic;


namespace SSMO.Data.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<BankDetails> BankDetails { get; set; } = new List<BankDetails>();
        public IEnumerable<CustomerOrder> CustomerOrders { get; set; }
        public IEnumerable<SupplierOrder> SupplierOrders { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
