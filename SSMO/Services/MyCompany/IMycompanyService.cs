﻿using DocumentFormat.OpenXml.Spreadsheet;
using SSMO.Models.Documents.Invoice;
using SSMO.Models.MyCompany;
using SSMO.Models.Reports;
using SSMO.Models.ServiceOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.MyCompany
{
    public interface IMycompanyService
    {
      public ICollection<string> GetCompaniesNames();
        public ICollection<string> GetCompaniesUserId();
        public ICollection<MyCompaniesForReportViewModel> GetAllCompanies();
        public ICollection<MyCompaniesForTrasnportOrderViewModel> GetCompaniesForTransportOrder();
        public string GetUserIdMyCompanyByName(string name);
        public string GetUserIdMyCompanyById(int id); 
        public string GetUserIdMyCompanyBySupplierOrdreNum(string supplierOrder);
        public List<string> MyCompaniesNamePerCustomer(string name);
        public List<string> MyCompaniesNamePerSupplier(string name);
        public bool RegisterMyCompany(
            string name, string eik, string vat, string fsc,string userId, string city, string addres, string country, string representativePerson,
            string bgName, string bgCity, string bgAddress, string bgCountry, string bgRepresentative,
            string correspondCountry, string correspondCity, string correspondStreet,
            string correspondBgCountry, string correspondBgCity,string correspondBgStreet);

        public int GetMyCompanyId(string name);
        public ICollection<string> MyCompaniesFscList();

        public string GetCompanyName(int id);
        public ICollection<MyCompanyViewModel> GetCompaniesNameAndId();

        public MyCompanyEditFormModel CompanyForEditById(int id);

        public bool EditCompany(int id, string name, string bgname, string eik, string vat,string fscClaim, string fscCertificate, 
            string representativeName, string representativeNameBg, string street, string bgStreet, 
            string city, string bgCity, string country, string bgCountry,
            string correspondCountry, string correspondCity, string correspondStreet,
            string correspondBgCountry, string correspondBgCity, string correspondBgStreet);
    }
}
