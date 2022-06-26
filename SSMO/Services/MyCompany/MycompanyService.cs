
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

        public MycompanyService(ApplicationDbContext dbContext, IMapper mapper
            )
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

        
    }
}
