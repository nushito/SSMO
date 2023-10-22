using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.FscTexts;
using System.Collections.Generic;
using System.Linq;

namespace SSMO.Services.FscTextDocuments
{
    public class FscTextService : IFscTextService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IConfigurationProvider mapper;
        public FscTextService(ApplicationDbContext dbContext, IConfigurationProvider configurationProvider)
        {
            this.dbContext = dbContext;
            mapper = configurationProvider;
        }

        public void AddFscText(string textEng, string textBg)
        {
            var text = new FscText
            {
                FscTextEng = textEng,
                FscTextBg = textBg
            };

            dbContext.FscTexts.Add(text);
            dbContext.SaveChanges();
            
        }

        public ICollection<FscTextViewModel> GetAllFscTexts()
        {
            return dbContext.FscTexts
                .ProjectTo<FscTextViewModel>(mapper)
                .ToList();
        }

    }
}
