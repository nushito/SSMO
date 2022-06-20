
using SSMO.Data.Models;
using SSMO.Services.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services
{
    public interface ISupplierService
    {
        public ICollection<AllSuppliers> GetSuppliers();
       
       
    }
}
