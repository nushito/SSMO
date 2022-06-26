using AutoMapper;
using SSMO.Data.Models;
using SSMO.Models.Customers;
using SSMO.Models.MyCompany;

namespace SSMO.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Customer, AddCustomerFormModel>();
            this.CreateMap<MyCompany, MyCompanyFormModel>();
        }
    }
}
