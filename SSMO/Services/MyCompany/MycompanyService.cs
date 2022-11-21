
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SSMO.Data;
using SSMO.Models.MyCompany;
using SSMO.Infrastructure;
using SSMO.Models.Reports;

namespace SSMO.Services.MyCompany
{
    public class MycompanyService : IMycompanyService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly HttpContextUserIdExtension _httpContextAccessor;
        public MycompanyService
            (ApplicationDbContext dbContext, IMapper mapper,
             HttpContextUserIdExtension httpContextAccessor)

        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public ICollection<MyCompaniesForReportViewModel> GetAllCompanies()
        {
            var loggedUserId = _httpContextAccessor.ContextAccessUserId();
            var listDbCompanies = dbContext.MyCompanies
                .Where(idvalue=>idvalue.UserId == loggedUserId).ToList();
            var allCompanies = mapper.Map<ICollection<MyCompaniesForReportViewModel>>(listDbCompanies);

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


    }
}
