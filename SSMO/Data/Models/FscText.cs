using System.Collections;
using System.Collections.Generic;

namespace SSMO.Data.Models
{
    public class FscText
    {
        public int Id { get; set; }
        public string FscTextEng { get; set; }
        public string FscTextBg { get; set; }
        public string UserId { get; set; }
        public ICollection<Document>  Documents { get; set; }
        public ICollection<CustomerOrder> CustomerOrders { get; set; }
    }
}
