using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Data.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CustomerOrder> CustomerOrders { get; set; }
    }
}
