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

        public ICollection<GetCurrencyModel> AllCurrency()
        {
            return this.dbContext
                 .Currencies
                 .Select(a => new GetCurrencyModel
                 {
                     Name = a.Name,
                     Id = a.Id
                 })
                 .ToList();
        }

       public ICollection<string> GetCurrencyList()
        {
            return this.dbContext
                .Currencies
                .Select(a => a.Name)
                .ToList();

        }

        public string GetCurrency(int id)
        {
            var currencyId = dbContext.SupplierOrders
                .Where(i => i.Id == id)
                .Select(c => c.CurrencyId)
                .FirstOrDefault();

            return dbContext.Currencies
                .Where(a => a.Id == currencyId)
                .Select(n => n.Name)
                .FirstOrDefault();
        }


    }
}
