
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SSMO.Data;
using SSMO.Models.MyCompany;
using SSMO.Infrastructure;
using SSMO.Models.Reports;
using SSMO.Data.Models;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.ServiceOrders;
using SSMO.Services.Addresses;

namespace SSMO.Services.MyCompany
{
    public class MycompanyService : IMycompanyService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly HttpContextUserIdExtension _httpContextAccessor;
        private readonly IAddressService addressService;
        public MycompanyService
            (ApplicationDbContext dbContext, IMapper mapper,
             HttpContextUserIdExtension httpContextAccessor, IAddressService addressService)

        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            this.addressService = addressService;
        }

        private List<Data.Models.MyCompany> UserCompanies()
        {
            var loggedUserId = _httpContextAccessor.ContextAccessUserId();
            var listDbCompanies = dbContext.MyCompanies
                .Where(idvalue => idvalue.UserId == loggedUserId).ToList();

            return listDbCompanies;
        }

        public ICollection<MyCompaniesForReportViewModel> GetAllCompanies()
        {  
            var allCompanies = mapper.Map<ICollection<MyCompaniesForReportViewModel>>(UserCompanies());

            return allCompanies;
        }

        public ICollection<MyCompaniesForTrasnportOrderViewModel> GetCompaniesForTransportOrder()
        {
            var allCompanies = mapper.Map<ICollection<MyCompaniesForTrasnportOrderViewModel>>(UserCompanies());

            return allCompanies;
        }
        public ICollection<string> GetCompaniesNames()
        {
            var loggedUserId = _httpContextAccessor.ContextAccessUserId();
            return dbContext.MyCompanies
                     .Where(userId => userId.UserId == loggedUserId)
                     .Select(x => x.Name)
                     .ToList();
        }
        public string GetUserIdMyCompanyByName(string name)
        {
            var userId = dbContext.MyCompanies
                    .Where(x => x.Name.ToLower() == name.ToLower())
                    .Select(id => id.UserId)
                    .FirstOrDefault();
            return userId;
        }
        public string GetUserIdMyCompanyById(int id)
        {
            var userId = dbContext.MyCompanies
                    .Where(x => x.Id == id)
                    .Select(id => id.UserId)
                    .FirstOrDefault();
            return userId;
        }

        public string GetUserIdMyCompanyBySupplierOrdreNum(string supplierOrder)
        {
            var myCompanyId = dbContext.SupplierOrders
                   .Where(x => x.Number.ToLower() == supplierOrder.ToLower())
                   .Select(id => id.MyCompanyId)
                   .FirstOrDefault();

            return dbContext.MyCompanies
                    .Where(x => x.Id == myCompanyId)
                    .Select(id => id.UserId)
                    .FirstOrDefault();
        }

        public List<string> MyCompaniesNamePerCustomer(string name)
        {
            var customerId = dbContext.Customers
                .Where(c => c.Name.ToLower() == name.ToLower())
                .Select(id => id.Id)
                .FirstOrDefault();

            var mycompanyIdList = dbContext.CustomerOrders.
                Where(cus => cus.CustomerId == customerId)
                .Select(comp => comp.MyCompanyId)
                .ToList();

            var listMycompaniesUserId = dbContext.MyCompanies.
                Where(i => mycompanyIdList.Contains(i.Id)).
                Select(u => u.UserId).
                ToList();

            return listMycompaniesUserId;
        }
        public List<string> MyCompaniesNamePerSupplier(string name)
        {
            var supplierId = dbContext.Suppliers
                .Where(c => c.Name.ToLower() == name.ToLower())
                .Select(id => id.Id)
                .FirstOrDefault();

            var mycompanyIdList = dbContext.SupplierOrders
                .Where(cus => cus.SupplierId == supplierId)
                .Select(comp => comp.MyCompanyId)
                .ToList();

            var listMycompaniesUserId = dbContext.MyCompanies.
                Where(i => mycompanyIdList.Contains(i.Id)).
                Select(u => u.UserId).
                ToList();

            return listMycompaniesUserId;
        }

        public bool RegisterMyCompany(
             string name, string eik, string vat, string fsc, string userId, string city, string addres, string country, string representativePerson,
            string bgName, string bgCity, string bgAddress, string bgCountry, string bgRepresentative,
            string correspondCountry, string correspondCity, string correspondStreet,
            string correspondBgCountry, string correspondBgCity, string correspondBgStreet)
        {
            if (userId == null) return false;

            var addressNum = addressService.CreateAddress
                (addres,city,country,bgAddress,bgCity,bgCountry, correspondStreet, 
                correspondCity, correspondCountry, correspondBgStreet, correspondBgCity, correspondBgCountry);

            var myCompany = new Data.Models.MyCompany
            {
                Name = name,
                BgName = bgName,
                Eik = eik,
                VAT = vat,
                RepresentativePerson = representativePerson,
                BgRepresentativePerson = bgRepresentative,
                FscSertificate = fsc,
                UserId = userId,
                AddressId = addressNum               
            };          

            dbContext.MyCompanies.Add(myCompany);   
            dbContext.SaveChanges();

            return true;
        }

        public ICollection<string> GetCompaniesUserId()
        {
            var loggedUserId = _httpContextAccessor.ContextAccessUserId();
            return dbContext.MyCompanies
                     .Where(userId => userId.UserId == loggedUserId)
                     .Select(x => x.UserId)
                     .ToList();
        }

        public int GetMyCompanyId(string name)
        {
            return dbContext.MyCompanies
                .Where(n=>n.Name.ToLower() == name.ToLower())
                .Select(id=>id.Id)
                .FirstOrDefault();
        }

        public ICollection<string> MyCompaniesFscList()
        {
            var userId = _httpContextAccessor.ContextAccessUserId();
            return dbContext.MyCompanies
                .Where(u => u.UserId == userId)
                .Select(f => f.FscSertificate)
                .ToList();            
        }

            
        public ICollection<MyCompanyViewModel> GetCompaniesNameAndId()
        {
            return dbContext.MyCompanies
                .Select(a=> new MyCompanyViewModel
                {
                    Id= a.Id,
                    Name= a.Name,
                }).ToList();
        }

        public string GetCompanyName(int id)
        {
            return dbContext.MyCompanies
                .Where(i=>i.Id == id)
                .Select(n=>n.Name) 
                .FirstOrDefault();
        }

        public MyCompanyEditFormModel CompanyForEditById(int id)
        {
            var company = dbContext.MyCompanies
                .Where(i=>i.Id == id);
           
            if (company.FirstOrDefault() == null) 
            {
                return null;
            }

            var companyForEdit = mapper.ProjectTo<MyCompanyEditFormModel>(company).FirstOrDefault();

            var address = dbContext.Addresses.
                Where(i=>i.Id == companyForEdit.AddressId)
                .FirstOrDefault();

            companyForEdit.Street = address.Street;
            companyForEdit.City = address.City;
            companyForEdit.BgCity = address.BgCity;
            companyForEdit.BgStreet = address.BgStreet; 
            companyForEdit.Country = address.Country;
            companyForEdit.BgCountry = address.Country;          

            return companyForEdit;
        }

        public bool EditCompany(int id, string name, string bgname, string eik, string vat, string fscClaim, 
            string fscCertificate, string representativeName, string representativeNameBg, string street, string bgStreet, 
            string city, string bgCity, string country, string bgCountry,
            string correspondCountry, string correspondCity, string correspondStreet,
            string correspondBgCountry, string correspondBgCity, string correspondBgStreet)
        {
            var company = dbContext.MyCompanies.Find(id);
            if (company == null) { return false; }

            company.Name = name;    
            company.BgName= bgname; 
            company.Eik = eik;
            company.VAT= vat;
            company.FscClaim= fscClaim; 
            company.FscSertificate = fscCertificate;
            company.RepresentativePerson= representativeName;
            company.BgRepresentativePerson = representativeNameBg;

            addressService.ЕditAddress
                (company.AddressId, street, city, country, bgStreet, bgCity, bgCountry,
                correspondStreet, correspondCity, correspondCountry, correspondBgStreet, 
                correspondBgCity, correspondBgCountry);
          
            return true;
        }
    }
}
