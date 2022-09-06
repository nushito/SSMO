
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Services.Supplier;
using System;
using System.Collections;
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

        //public List<SelectListItem> GetSuppliersByCustomerId(int id)
        //{
            
        //    var listCustomerOrders = dbContext.CustomerOrders.Where(a => a.CustomerId == id).Select(a => a.Id).ToList();

        //    var proba = dbContext.SupplierOrders.Where(a => listCustomerOrders.Contains(a.CustomerOrderId)).ToList()
        //        .GroupBy(a => a.SupplierId).Select(a => a.First()).ToList();
              
        //    var listItems = new List<SelectListItem>();

        //    foreach (var item in proba)
        //    {
        //        var name = dbContext.Suppliers.Where(a=>a.Id == item.SupplierId).Select(a => a.Name).FirstOrDefault();

        //        listItems.Add(new SelectListItem { Text = name, Value = item.SupplierId.ToString() });
        //    }

        //    return listItems;
        //}

        public IEnumerable<SupplierDetailsList> GetSuppliersNames(int id)
        {

            var supplierDetailList = dbContext.CustomerOrders.
                Where(a => a.CustomerId == id)
                .SelectMany(a => a.SupplierOrder
                         .Select(a => new SupplierDetailsList
                         {
                             SupplierId = a.SupplierId,
                             SupplierName = a.Supplier.Name
                         }).Distinct())
                .ToList();

           var distinctSupplierDetailsList = supplierDetailList.GroupBy(a=>a.SupplierId).Select(a=>a.First()).ToList();   
           
            return distinctSupplierDetailsList;
        }
    }
}
