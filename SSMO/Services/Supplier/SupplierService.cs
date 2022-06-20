
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Services.Supplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext dbContext;

        public SupplierService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ICollection<AllSuppliers> GetSuppliers()
        {
            return dbContext
                 .Suppliers
                 .Select(a => 
                 new AllSuppliers 
                 {
                    Id = a.Id,
                    Name = a.Name
                 })
                 .ToList();

        }
    }
}
