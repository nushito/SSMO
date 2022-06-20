using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using SSMO.Data;
using SSMO.Infrastructure;
using SSMO.Models.MyCompany;

namespace SSMO.Services.MyCompany
{
    public class MycompanyService : IMycompanyService
    {
        private readonly ApplicationDbContext dbContext;
        
        public MycompanyService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ICollection<MyCompanyFormModel> GetAllCompanies()
        {
            return (ICollection<MyCompanyFormModel>)dbContext.MyCompanies.ToList();
        }

        public ICollection<string> GetCompany()
        {
            
                return dbContext.MyCompanies
                    .Select(x => x.Name)
                    .ToList();
         }

        
    }
}
