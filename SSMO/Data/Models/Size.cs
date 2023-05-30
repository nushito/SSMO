using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Data.Models
{
    public class Size
    {
        public int Id { get; init; }

        [RegularExpression(@"^\\d+[\\.|,]?\\d+\\/\\d+\\/\\d+\\s?mm$")]
        public string Name { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
