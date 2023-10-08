using System.Collections;
using System.Collections.Generic;

namespace SSMO.Data.Models
{
    public class FiscalAgent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string BgName { get; set; }
        public string BgDetails { get; set; }
        public ICollection<Document>  Documents { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
    }
}
