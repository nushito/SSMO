using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Data.Models
{
    public class Size
    {
        public int Id { get; init; }
        public string Name { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
