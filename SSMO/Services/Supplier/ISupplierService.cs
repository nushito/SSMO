
using Microsoft.AspNetCore.Mvc.Rendering;
using SSMO.Data.Models;
using SSMO.Models.Suppliers;
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
       
        public IEnumerable<SupplierDetailsList> GetSuppliersIdAndNames(int id);
        //public List<SelectListItem> GetSuppliersByCustomerId(int id);
        public IEnumerable<string> GetSupplierNames();
        public EditSupplierFormModel GetSupplierForEdit(string supplierName);

        public bool EditSupplier(string supplierName, string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string fscCertificate);

        public string GetSupplierFscCertificateByOrderNumber(string orderNumber);

    }
}
