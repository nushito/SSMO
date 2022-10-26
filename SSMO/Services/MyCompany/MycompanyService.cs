using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SSMO.Data;
using SSMO.Infrastructure;
using SSMO.Models.MyCompany;

namespace SSMO.Services.MyCompany
{
    public class MycompanyService : IMycompanyService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
       
        public MycompanyService(ApplicationDbContext dbContext, IMapper mapper)

        {
            this.dbContext = dbContext;
            this.mapper = mapper;
         }

        public ICollection<MyCompanyFormModel> GetAllCompanies()
        {
            var listDbCompanies = dbContext.MyCompanies.ToList();
            var allCompanies = mapper.Map<ICollection<MyCompanyFormModel>>(listDbCompanies); 

            return allCompanies;
        }

        public ICollection<string> GetCompany()
        {
            
                return dbContext.MyCompanies
                    .Select(x => x.Name)
                    .ToList();
         }

        public string GetUserIdMyCompanyByName(string name)
        {
            var userId = dbContext.MyCompanies
                    .Where(x => x.Name.ToLower() == name.ToLower())
                    .Select(id=>id.UserId)
                    .FirstOrDefault();
            return userId;
        }
        public string GetUserIdMyCompanyById(int id)
        {
            var userId = dbContext.MyCompanies
                    .Where(x =>x.Id == id)
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
    }
}
