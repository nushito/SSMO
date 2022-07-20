using AutoMapper;
using SSMO.Data;
using SSMO.Models;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.Status
{
    public class StatusService : IStatusService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public StatusService(ApplicationDbContext dbContext, IMapper provider)
        {
            _context = dbContext;   
            mapper = provider;
        }
        public ICollection<StatusModel> GetAllStatus()
        {
            var listStatus = _context.Statuses.ToList();

            var statuses = this.mapper.Map<ICollection<StatusModel>>(listStatus);

            return statuses;

           
        }
    }
}
