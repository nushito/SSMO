using SSMO.Data;
using SSMO.Services.Curruncies;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SSMO.Services
{
    public class Currency : ICurrency
    {
        private readonly ApplicationDbContext dbContext;

        public Currency(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<GetCurrencyModel> AllCurrency()
        {
            return this.dbContext
                 .Currencys
                 .Select(a => new GetCurrencyModel
                 {
                     Name = a.Name,
                     Id = a.Id
                 })
                 .ToList();
        }

        ICollection<string> ICurrency.GetCurrency()
        {
            return this.dbContext
                .Currencys
                .Select(a => a.Name)
                .ToList();

        }


    }
}
