using AutoMapper;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Suppliers;
using SSMO.Services.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SSMO.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public SupplierService(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;       
        }

        public bool AddNewSupplier
            (string name, string vat, string eik, string email, string city, 
            string street, string country, string manager, string fscCertificate)
        {
            var newSupplier = new SSMO.Data.Models.Supplier
            {
                Name = name,
                VAT = vat,
                Eik = eik,
                Email = email,
                Address = new Address
                {
                    City = city,
                    Street = street,
                    Country = country
                },
                RepresentativePerson = manager,
                FSCSertificate = fscCertificate
            };

            //var bankDetail = new BankDetails
            //{
            //    BankName = model.BankName,
            //    Iban = model.Iban,
            //    Address = model.BankAddress,
            //    Swift = model.Swift,
            //    Currency = new InvoiceAndStockModels.Currency { AccountCurrency = (AccountCurrency)Enum.Parse(typeof(AccountCurrency), model.Currency) }  //(AccountCurrency)Enum.Parse(typeof(AccountCurrency),model.Currency)              
            //};

            //supplier.BankDetails.Add(bankDetail);

            if (newSupplier == null) return false;

            this.dbContext.Suppliers.Add(newSupplier);
            this.dbContext.SaveChanges();

            return true;
        }

        public bool EditSupplier
            (string supplierName, string vat, string eik, string representativePerson,
            string country, string city, string street, string email, string fscCertificate)
        {
            var supplier = dbContext.Customers
                .Where(a => a.Name.ToLower() == supplierName.ToLower())
                .FirstOrDefault();
            if (supplier == null) return false;
            supplier.Name = supplierName;
            supplier.VAT = vat;
            supplier.EIK = eik;
            supplier.RepresentativePerson = representativePerson;
            supplier.Email = email;

            var address = dbContext.Addresses
                .Where(c => c.Id == supplier.AddressId)
                .FirstOrDefault();

            address.Country = country;
            address.City = city;
            address.Street = street;

            dbContext.SaveChanges();
            return true;
        }

        public EditSupplierFormModel GetSupplierForEdit(string supplierName)
        {
            if (String.IsNullOrEmpty(supplierName))
            {
                return null;
            }
            var suuplier = dbContext.Suppliers.Where(a => a.Name.ToLower() == supplierName.ToLower()).FirstOrDefault();
            if (suuplier == null)
            {
                return null;
            }
            var address = dbContext.Addresses.Where(a => a.Id == suuplier.AddressId).FirstOrDefault();
            var addressForEdit = mapper.Map<EditSupplierAddressFormModel>(address);
            var getSupplier = mapper.Map<EditSupplierFormModel>(suuplier);
            getSupplier.SupplierAddress = new EditSupplierAddressFormModel
            {
                City = addressForEdit.City,
                Country = addressForEdit.Country,
                Street = addressForEdit.Street
            };

            return getSupplier;
        }

        public string GetSupplierFscCertificateByOrderNumber(string orderNumber)
        {
            var supplierId = dbContext.SupplierOrders
                .Where(num => num.Number.ToLower() == orderNumber.ToLower())
                .Select(s => s.SupplierId)
                .FirstOrDefault();

            var supplierFsc = dbContext.Suppliers
                .Where(id => id.Id == supplierId)
                .Select(fs => fs.FSCSertificate)
                .FirstOrDefault();

            if(supplierFsc == null)
            {
                return null;
            }

            return supplierFsc;
        }

        public string GetSupplierFscCertificateByOrderId(int orderId)
        {
            var supplierId = dbContext.SupplierOrders
                .Where(i => i.Id == orderId)
                .Select(s => s.SupplierId)
                .FirstOrDefault();

            var supplierFsc = dbContext.Suppliers
                .Where(id => id.Id == supplierId)
                .Select(fs => fs.FSCSertificate)
                .FirstOrDefault();

            if (supplierFsc == null)
            {
                return null;
            }

            return supplierFsc;

        }

        public IEnumerable<string> GetSupplierNames()
        {
            return dbContext
                 .Suppliers
                 .Select(n => n.Name
                 ).ToList();
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

        public IEnumerable<SupplierDetailsList> GetSuppliersIdAndNames(int id)
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

        public string SupplierNameById(int id)
        {
            var supplierId = dbContext.SupplierOrders
                .Where(i => i.Id == id)
                .Select(s => s.SupplierId)
                .FirstOrDefault();

            return dbContext.Suppliers
                  .Where(i => i.Id == supplierId)
                  .Select(n => n.Name)
                  .FirstOrDefault();
        }
    }
}
