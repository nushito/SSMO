
using SSMO.Models.Suppliers;
using SSMO.Services.Suppliers;
using System.Collections.Generic;


namespace SSMO.Services
{
    public interface ISupplierService
    {
        public bool AddNewSupplier
            (string name, string vat,string eik, string email, string city, 
            string street, string country,string manager, string fscCertificate);
        public ICollection<AllSuppliers> GetSuppliers();
       
        public IEnumerable<SupplierDetailsList> GetSuppliersIdAndNames(int id);
        //public List<SelectListItem> GetSuppliersByCustomerId(int id);
        public IEnumerable<string> GetSupplierNames();
        public EditSupplierFormModel GetSupplierForEdit(string supplierName);

        public bool EditSupplier(string supplierName, string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string fscCertificate);

        public string GetSupplierFscCertificateByOrderNumber(string orderNumber);
        public string GetSupplierFscCertificateByOrderId(int orderId);
        public string SupplierNameById(int id);
        public string SupplierNameBySupplierOrderId(int id);
        public List<string> SuppliersFscCertificates();
    }
}
